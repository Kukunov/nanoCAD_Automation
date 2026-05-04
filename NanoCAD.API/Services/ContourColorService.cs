using System;
using System.Collections.Generic;
using System.Linq;
using Teigha.DatabaseServices;
using Teigha.Geometry;
using Color = Teigha.Colors.Color;

namespace NanoCAD.API.Services
{
    /// <summary>
    /// Результат операции окрашивания
    /// </summary>
    public class ColorizationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Dictionary<int, Color> ColoredContours { get; set; } = new();
    }

    /// <summary>
    /// Сервис для цветового кодирования контуров
    /// </summary>
    public class ContourColorService
    {
        // Имя вспомогательного слоя
        public const string ColorLayerName = "ГОСТ_ЦВЕТ_КОНТУРЫ";

        // Палитра цветов
        private static readonly Color[] Palette = new[]
        {
            Color.FromRgb(255, 80, 80),    // 1. Красный
            Color.FromRgb(80, 140, 255),   // 2. Синий
            Color.FromRgb(60, 180, 60),    // 3. Зелёный
            Color.FromRgb(255, 170, 30),   // 4. Оранжевый
            Color.FromRgb(170, 90, 255)    // 5. Фиолетовый
        };

        // Получить цвет для контура по номеру
        public static Color GetColorForContour(int contourNumber)
        {
            int index = (contourNumber - 1) % Palette.Length;
            return Palette[index];
        }

        // Создать или получить слой
        public ObjectId EnsureColorLayer(Database db, Transaction tr)
        {
            var layerTable = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForWrite);

            if (layerTable.Has(ColorLayerName))
                return layerTable[ColorLayerName];

            var layerRecord = new LayerTableRecord
            {
                Name = ColorLayerName,
                IsPlottable = false,    // Не выводится на печать
                IsOff = false,          // Включён по умолчанию
                IsFrozen = false,
                IsLocked = false
            };

            var layerId = layerTable.Add(layerRecord);
            tr.AddNewlyCreatedDBObject(layerRecord, true);

            return layerId;
        }

        // Получить список активных контуров по фактическим блокам на чертеже
        public Dictionary<int, List<ObjectId>> GetContourBlocks(Database db)
        {
            var contourBlocks = new Dictionary<int, List<ObjectId>>();

            using (var tr = db.TransactionManager.StartTransaction())
            {
                var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                var modelSpace = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                foreach (ObjectId objId in modelSpace)
                {
                    var blockRef = tr.GetObject(objId, OpenMode.ForRead) as BlockReference;
                    if (blockRef == null) continue;

                    // Ищем подходящие блоки
                    var blockDef = (BlockTableRecord)tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead);
                    string blockName = blockDef.Name;
                    if (blockName != "ПриборНаЩите" && blockName != "ПриборВнеЩита")
                        continue;

                    // Извлекаем номер контура из атрибута ПОЗ
                    int? contour = ExtractContourFromBlock(tr, blockRef);
                    if (!contour.HasValue) continue;

                    if (!contourBlocks.ContainsKey(contour.Value))
                        contourBlocks[contour.Value] = new List<ObjectId>();

                    contourBlocks[contour.Value].Add(blockRef.ObjectId);
                }

                tr.Commit();
            }

            return contourBlocks;
        }

        // Применить цвета ко всем активным контурам
        public ColorizationResult ApplyAutoColors(Database db)
        {
            var result = new ColorizationResult();

            using (var tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    // Создаём слой
                    var layerId = EnsureColorLayer(db, tr);

                    // Получаем блоки по контурам
                    var contourBlocks = GetContourBlocks(db);

                    if (contourBlocks.Count == 0)
                    {
                        tr.Commit();
                        result.Success = true;
                        result.Message = "На чертеже не найдено приборов с атрибутами ПОЗ.";
                        return result;
                    }

                    // Назначаем цвета
                    foreach (var kvp in contourBlocks)
                    {
                        int contour = kvp.Key;
                        Color color = GetColorForContour(contour);

                        foreach (ObjectId blockId in kvp.Value)
                        {
                            var blockRef = (BlockReference)tr.GetObject(blockId, OpenMode.ForWrite);

                            // Назначаем цвет блоку
                            blockRef.Color = color;
                            blockRef.Layer = ColorLayerName;

                            // Назначаем цвет атрибутам
                            foreach (ObjectId attId in blockRef.AttributeCollection)
                            {
                                var attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForWrite);
                                attRef.Color = color;
                                attRef.Layer = ColorLayerName;
                            }

                            // Устанавливаем цвет ByBlock для геометрии
                            SetBlockGeometryToByBlock(tr, blockRef, color);
                        }

                        result.ColoredContours.Add(contour, color);
                    }

                    tr.Commit();

                    result.Success = true;
                    result.Message = $"Окрашено контуров: {contourBlocks.Count}, блоков: {contourBlocks.Values.Sum(l => l.Count)}";
                    return result;
                }
                catch (Exception ex)
                {
                    tr.Abort();
                    result.Success = false;
                    result.Message = $"Ошибка: {ex.Message}";
                    return result;
                }
            }
        }

        // Установить геометрии блока свойство "ByBlock" для наследования цвета
        private void SetBlockGeometryToByBlock(Transaction tr, BlockReference blockRef, Color color)
        {
            var blockDef = (BlockTableRecord)tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForWrite);

            foreach (ObjectId objId in blockDef)
            {
                var entity = tr.GetObject(objId, OpenMode.ForWrite) as Entity;
                if (entity == null) continue;

                // Атрибуты пропускаем — они уже раскрашены через blockRef.AttributeCollection
                if (entity is AttributeDefinition) continue;

                // Устанавливаем цвет ByBlock
                entity.Color = Color.FromRgb(0, 0, 0); // ByBlock в Teigha
                entity.ColorIndex = 0; // 0 = ByBlock
            }
        }

        // Очистить цвета контуров (вернуть на стандартный слой)
        public void ClearContourColors(Database db)
        {
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var contourBlocks = GetContourBlocks(db);

                foreach (var kvp in contourBlocks)
                {
                    foreach (ObjectId blockId in kvp.Value)
                    {
                        var blockRef = (BlockReference)tr.GetObject(blockId, OpenMode.ForWrite);
                        blockRef.Color = Color.FromRgb(255, 255, 255); // ByBlock
                        blockRef.Layer = "0";

                        foreach (ObjectId attId in blockRef.AttributeCollection)
                        {
                            var attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForWrite);
                            attRef.Color = Color.FromRgb(255, 255, 255);
                            attRef.Layer = "0";
                        }

                        // Возвращаем геометрии цвет ByBlock
                        SetBlockGeometryToByBlock(tr, blockRef, Color.FromRgb(0, 0, 0));
                    }
                }

                tr.Commit();
            }
        }

        // Извлечь номер контура из блока
        private int? ExtractContourFromBlock(Transaction tr, BlockReference blockRef)
        {
            foreach (ObjectId attId in blockRef.AttributeCollection)
            {
                var attRef = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                if (attRef != null && attRef.Tag.ToUpperInvariant() == "ПОЗ")
                {
                    string position = attRef.TextString;
                    if (ValidationService.IsPositionFormat(position))
                    {
                        return ValidationService.ExtractContourNumber(position);
                    }
                }
            }
            return null;
        }
    }
}