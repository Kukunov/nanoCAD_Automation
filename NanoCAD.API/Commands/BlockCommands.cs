using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using NanoCAD.API.Models;
using NanoCAD.API.Services;
using Teigha.Geometry;
using Teigha.Runtime;
using Application = HostMgd.ApplicationServices.Application;

namespace NanoCAD.API.Commands
{
    /// <summary>
    /// Команды и методы вставки блоков
    /// </summary>
    public class BlockCommands
    {
        private readonly BlockService _blockService = new();

        #region Команды пользователя

        // Проверить наличие файла blocks.dwg в каталоге надстройки
        [CommandMethod("ПРОВБЛОКИ", CommandFlags.Modal)]
        [CommandMethod("CHECKBLOCKS", CommandFlags.Modal)]
        public void CheckBlocksFile()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;

            if (_blockService.IsBlocksFileAvailable())
            {
                ed.WriteMessage($"\n[OK] Файл blocks.dwg найден: {_blockService.GetBlocksFilePath()}");
            }
            else
            {
                ed.WriteMessage($"\n[ОШИБКА] Файл blocks.dwg не найден: {_blockService.GetBlocksFilePath()}");
                ed.WriteMessage("\nПоместите файл blocks.dwg в папку с DLL библиотекой.");
            }
        }

        // Вставка блока ПриборНаЩите
        [CommandMethod("ПНЩ", CommandFlags.Modal)]
        [CommandMethod("PNSH", CommandFlags.Modal)]
        public void InsertPNSH()
        {
            InsertBlock("ПриборНаЩите", "ПНЩ");
        }

        // Вставка блока ПриборВнеЩита
        [CommandMethod("ПВЩ", CommandFlags.Modal)]
        [CommandMethod("PVSH", CommandFlags.Modal)]
        public void InsertPVSH()
        {
            InsertBlock("ПриборВнеЩита", "ПВЩ");
        }

        // Выбор блока для вставки через список
        [CommandMethod("ГОСТВСТАВКА", CommandFlags.Modal)]
        [CommandMethod("GOSTINSERTION", CommandFlags.Modal)]
        public void InsertBlockWithChoice()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;

            var keyOptions = new PromptKeywordOptions("\nВыберите тип блока: ");
            keyOptions.Keywords.Add("Прибор по месту");
            keyOptions.Keywords.Add("Прибор на щите");
            keyOptions.AllowNone = true;

            var keyResult = ed.GetKeywords(keyOptions);
            if (keyResult.Status != PromptStatus.OK) return;

            string blockName = keyResult.StringResult.ToUpperInvariant() switch
            {
                "Прибор по месту" => "ПриборНаЩите",
                "Прибор на щите" => "ПриборВнеЩита",
                _ => "ПриборВнеЩита"
            };

            InsertBlock(blockName, keyResult.StringResult);
        }

        #endregion

        private void InsertBlock(string blockName, string commandName)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;

            if (!_blockService.IsBlocksFileAvailable())
            {
                ed.WriteMessage($"\n[ОШИБКА] Файл blocks.dwg не найден.");
                ed.WriteMessage($"\nПуть: {_blockService.GetBlocksFilePath()}");
                return;
            }

            var contourService = new ContourService(doc.Database);

            ed.WriteMessage($"\n=== Вставка блока: {blockName} [{commandName}] ===");
            ed.WriteMessage($"\nТекущий контур: {contourService.GetCurrentContour()}");

            // 1. Запрос ТИП с предложением последнего использованного значения
            string typeDesignation = PromptForTypeDesignation(ed, contourService);
            if (typeDesignation == null) return;

            // 2. Запрос ПОЗ с автоматической генерацией
            string position = PromptForPosition(ed, contourService);
            if (position == null) return;

            // 3. Запрос точки вставки
            var pointOptions = new PromptPointOptions("\nУкажите точку вставки (ЛКМ или координаты X,Y): ");
            pointOptions.AllowNone = false;
            pointOptions.UseBasePoint = false;

            var pointResult = ed.GetPoint(pointOptions);

            if (pointResult.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\n[ОТМЕНА] Вставка отменена.");
                return;
            }

            // 4. Вставка блока
            var options = new BlockInsertOptions
            {
                BlockName = blockName,
                TypeDesignation = typeDesignation,
                Position = position
            };

            var result = _blockService.InsertBlock(doc.Database, options, pointResult.Value);

            // 5. Вывод результата
            if (result.Success)
            {
                ed.WriteMessage($"\n[OK] Блок вставлен.");
                ed.WriteMessage($"\nТИП: {result.TypeDesignation} | ПОЗ: {result.Position}");
                ed.WriteMessage($"\n{contourService.GetStatusInfo()}");
            }
            else
            {
                ed.WriteMessage($"\n[ОШИБКА] {result.Message}");
            }
        }

        // Запрос обозначения типа с валидацией
        private string PromptForTypeDesignation(Editor ed, ContourService contourService)
        {
            string lastType = contourService.GetLastTypeDesignation();

            while (true)
            {
                var typeOptions = new PromptStringOptions($"\nВведите обозначение типа (ТИП) <{lastType}>: ");
                typeOptions.AllowSpaces = false;
                typeOptions.DefaultValue = lastType;
                var typeResult = ed.GetString(typeOptions);

                if (typeResult.Status == PromptStatus.Cancel)
                {
                    ed.WriteMessage("\n[ОТМЕНА] Вставка отменена.");
                    return null;
                }

                if (typeResult.Status != PromptStatus.OK)
                {
                    continue;
                }

                string input = typeResult.StringResult;
                string typeDesignation = input.ToUpperInvariant();
                var validation = ValidationService.ValidateTypeDesignation(typeDesignation);

                if (validation.IsValid)
                {
                    contourService.SetLastTypeDesignation(typeDesignation);
                    return typeDesignation;
                }

                ed.WriteMessage($"\n[ОШИБКА] {validation.ErrorMessage}");
                ed.WriteMessage("\nПопробуйте снова или нажмите ESC для отмены.");
            }
        }

        // Запрос позиционного обозначения с валидацией
        private string PromptForPosition(Editor ed, ContourService contourService)
        {
            string autoPosition = contourService.GetNextPosition();

            while (true)
            {
                var posOptions = new PromptStringOptions($"\nВведите позиционное обозначение (ПОЗ) в формате 'контур-номер': ");
                posOptions.AllowSpaces = false;
                posOptions.DefaultValue = autoPosition;
                var posResult = ed.GetString(posOptions);

                if (posResult.Status == PromptStatus.Cancel)
                {
                    ed.WriteMessage("\n[ОТМЕНА] Вставка отменена.");
                    return null;
                }

                if (posResult.Status != PromptStatus.OK)
                {
                    continue;
                }

                string input = posResult.StringResult;

                // Если пользователь нажал Enter - используем автоматическую позицию
                if (string.IsNullOrWhiteSpace(input))
                {
                    return autoPosition;
                }

                var validation = ValidationService.ValidatePosition(input);
                if (validation.IsValid)
                {
                    // Синхронизируем счётчики с введённой позицией
                    SyncContourFromPosition(contourService, input);
                    return input;
                }

                ed.WriteMessage($"\n[ОШИБКА] {validation.ErrorMessage}");
                ed.WriteMessage("\nПопробуйте снова или нажмите ESC для отмены.");
            }
        }

        // Синхронизирует счётчики контуров с введённой позицией
        private void SyncContourFromPosition(ContourService service, string position)
        {
            if (!ValidationService.IsPositionFormat(position))
                return;

            int contour = ValidationService.ExtractContourNumber(position);
            int element = ValidationService.ExtractElementNumber(position);

            // Если контур отличается — переключаемся
            if (contour != service.GetCurrentContour())
            {
                service.SetCurrentContour(contour);
            }

            // Обновляем счётчик: устанавливаем на введённое значение
            var allContours = service.GetAllContours();
            int currentCounter = allContours.ContainsKey(contour) ? allContours[contour] : 0;

            // Обновляем только если введённый элемент больше текущего счётчика
            if (element > currentCounter)
            {
                service.ResetContour(contour);
                for (int i = 0; i < element; i++)
                {
                    service.GetNextPosition(contour);
                }
            }
        }      
    }
}