using System.IO;
using Teigha.DatabaseServices;
using Teigha.Geometry;
using NanoCAD.API.Helpers;
using NanoCAD.API.Models;

namespace NanoCAD.API.Services
{
    /// <summary>
    /// Сервис для вставки блоков из файла-контейнера blocks.dwg
    /// </summary>
    public class BlockService
    {
        // Вставка блоков с атрибутами
        public BlockInsertResult InsertBlock(Database db, BlockInsertOptions options, Point3d insertionPoint)
        {
            // Создаём объект результата и заполняем входными данными
            var result = new BlockInsertResult
            {
                BlockName = options.BlockName,
                TypeDesignation = options.TypeDesignation,
                Position = options.Position
            };

            // Проверяем существование файла blocks.dwg
            string blocksPath = PathHelper.GetBlocksFilePath();
            if (!File.Exists(blocksPath))
            {
                result.Success = false;
                result.Message = $"Файл blocks.dwg не найден: {blocksPath}";
                return result;
            }

            // Начинаем транзакцию с чертежом nanoCAD
            using (var tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    // Получаем таблицу блоков чертежа
                    var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

                    // Получаем пространство модели (то, что видно на экране)
                    var modelSpace = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    // Если блок ещё не импортирован в чертёж - импортируем его
                    if (!bt.Has(options.BlockName))
                    {
                        ImportBlockFromContainer(db, tr, bt, options.BlockName, blocksPath);

                        // Обновляем ссылку на таблицу блоков
                        bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                    }

                    // Проверяем, что блок успешно импортирован
                    if (!bt.Has(options.BlockName))
                    {
                        result.Success = false;
                        result.Message = $"Не удалось импортировать блок '{options.BlockName}'";
                        return result;
                    }

                    // Получаем ObjectId определения блока
                    ObjectId blockDefId = bt[options.BlockName];

                    // Создаём вхождение блока (экземпляр на чертеже)
                    var blockRef = new BlockReference(insertionPoint, blockDefId)
                    {
                        ScaleFactors = new Scale3d(1, 1, 1),  // Масштаб 1:1
                        Rotation = 0                          // Без поворота
                    };

                    // Добавляем блок в пространство модели
                    modelSpace.AppendEntity(blockRef);
                    tr.AddNewlyCreatedDBObject(blockRef, true);

                    // Создаём и заполняем атрибуты блока
                    CreateAttributes(tr, blockRef, options);

                    // Фиксируем транзакцию
                    tr.Commit();

                    result.Success = true;
                    result.Message = $"Блок '{options.BlockName}' успешно вставлен";
                    return result;
                }
                catch (System.Exception ex)
                {
                    // В случае ошибки откатываем все изменения
                    tr.Abort();
                    result.Success = false;
                    result.Message = $"Ошибка при вставке блока: {ex.Message}";
                    return result;
                }
            }
        }

        // Импортирует определение блока из внешнего файла blocks.dwg
        private void ImportBlockFromContainer(Database targetDb, Transaction targetTr, 
                                              BlockTable targetBt, string blockName, string containerPath)
        {
            // Создаём временную базу данных для файла-контейнера
            using (var sourceDb = new Database(false, true))
            {
                // Загружаем файл blocks.dwg во временную базу
                sourceDb.ReadDwgFile(containerPath, FileOpenMode.OpenForReadAndAllShare, true, null);

                // Начинаем отдельную транзакцию для исходного файла
                using (var sourceTr = sourceDb.TransactionManager.StartTransaction())
                {
                    // Получаем таблицу блоков исходного файла
                    var sourceBt = (BlockTable)sourceTr.GetObject(sourceDb.BlockTableId, OpenMode.ForRead);

                    // Проверяем, есть ли нужный блок в исходном файле
                    if (!sourceBt.Has(blockName))
                    {
                        throw new System.Exception($"Блок '{blockName}' не найден в файле blocks.dwg");
                    }

                    // Получаем ObjectId определения блока в исходном файле
                    ObjectId sourceBlockId = sourceBt[blockName];

                    // Создаём маппинг для отслеживания скопированных объектов
                    var idMapping = new IdMapping();

                    // Клонируем определение блока из исходного файла в текущий чертёж
                    // Глубокое копирование объектов между базами данных
                    targetDb.WblockCloneObjects(
                        new ObjectIdCollection { sourceBlockId },   // Что копируем
                        targetBt.ObjectId,                          // Владелец в целевом чертеже
                        idMapping,                                  // Маппинг ID
                        DuplicateRecordCloning.Replace,             // Заменять при совпадении имён
                        false                                       // Не копировать вложенные объекты
                    );

                    // Фиксируем транзакцию исходного файла
                    sourceTr.Commit();
                }
            }
        }

        // Создаёт атрибуты для вхождения блока и заполняет их значениями
        private void CreateAttributes(Transaction tr, BlockReference blockRef, BlockInsertOptions options)
        {
            // Получаем определение блока (BlockTableRecord)
            var blockDef = (BlockTableRecord)tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead);

            // Если у блока нет атрибутов - выходим
            if (!blockDef.HasAttributeDefinitions)
                return;

            // Перебираем объекты в определении блока
            foreach (ObjectId objId in blockDef)
            {
                // Получаем объект из чертежа
                var obj = tr.GetObject(objId, OpenMode.ForRead);

                // Находим определения атрибутов (AttributeDefinition)
                if (obj is AttributeDefinition attDef)
                {
                    // Создаём новый атрибут (AttributeReference)
                    var attRef = new AttributeReference();

                    // Копируем все свойства из определения и связываем атрибут с блоком
                    attRef.SetAttributeFromBlock(attDef, blockRef.BlockTransform);

                    // TransformBy применяет матрицу трансформации блока (позиция, масштаб, поворот)
                    // к позиции атрибута, чтобы он правильно разместился на чертеже
                    attRef.Position = attDef.Position.TransformBy(blockRef.BlockTransform);

                    // Получаем тег атрибута для определения, какое значение подставить
                    string tag = attDef.Tag.Trim().ToUpperInvariant();

                    // Заполняем значение атрибута в зависимости от тега
                    switch (tag)
                    {
                        case "ТИП":
                            attRef.TextString = options.TypeDesignation;
                            break;
                        case "ПОЗ":
                            attRef.TextString = options.Position;
                            break;
                        default:
                            // Для остальных атрибутов оставляем значение по умолчанию
                            attRef.TextString = attDef.TextString;
                            break;
                    }

                    // Добавляем атрибут в коллекцию атрибутов блока
                    blockRef.AttributeCollection.AppendAttribute(attRef);

                    // Регистрируем новый объект в транзакции
                    tr.AddNewlyCreatedDBObject(attRef, true);
                }
            }
        }

        // Проверяет доступность файла blocks.dwg
        public bool IsBlocksFileAvailable() => PathHelper.BlocksFileExists();

        // Возвращает путь к файлу blocks.dwg
        public string GetBlocksFilePath() => PathHelper.GetBlocksFilePath();
    }
}