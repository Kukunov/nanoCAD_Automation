using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Teigha.Runtime;
using NanoCAD.API.Models;
using NanoCAD.API.Services;
using Application = HostMgd.ApplicationServices.Application;

namespace NanoCAD.API.Commands
{
    public class BlockCommands
    {
        private readonly BlockService _blockService = new();

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

        [CommandMethod("ПНЩ", CommandFlags.Modal)]
        [CommandMethod("PNSH", CommandFlags.Modal)]
        public void InsertPNSH()
        {
            InsertBlock("ПриборНаЩите", "ПНЩ");
        }

        [CommandMethod("ПВЩ", CommandFlags.Modal)]
        [CommandMethod("PVSH", CommandFlags.Modal)]
        public void InsertPVSH()
        {
            InsertBlock("ПриборВнеЩита", "ПВЩ");
        }

        [CommandMethod("ВСТАВИТЬБЛОК", CommandFlags.Modal)]
        [CommandMethod("INSERTBLOCK", CommandFlags.Modal)]
        public void InsertBlockWithChoice()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;

            if (!_blockService.IsBlocksFileAvailable())
            {
                ed.WriteMessage($"\n[ОШИБКА] Файл blocks.dwg не найден: {_blockService.GetBlocksFilePath()}");
                return;
            }

            var keyOptions = new PromptKeywordOptions("\nВыберите тип блока: ");
            keyOptions.Keywords.Add("ПНЩ");
            keyOptions.Keywords.Add("ПВЩ");
            keyOptions.Keywords.Add("PNSH");
            keyOptions.Keywords.Add("PVSH");
            keyOptions.AllowNone = true;

            var keyResult = ed.GetKeywords(keyOptions);
            if (keyResult.Status != PromptStatus.OK) return;

            string blockName = keyResult.StringResult.ToUpperInvariant() switch
            {
                "ПНЩ" or "PNSH" => "ПриборНаЩите",
                "ПВЩ" or "PVSH" => "ПриборВнеЩита",
                _ => "ПриборНаЩите"
            };

            InsertBlock(blockName, keyResult.StringResult);
        }

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

            // 1. Запрос обозначения типа (ТИП) с валидацией
            string typeDesignation = PromptForTypeDesignation(ed);
            if (typeDesignation == null) return; // Пользователь отменил ввод

            // 2. Запрос позиции (ПОЗ) с валидацией и автоматической генерацией
            string position = PromptForPosition(ed, contourService);
            if (position == null) return; // Пользователь отменил ввод

            // 3. Запрос точки вставки
            var pointOptions = new PromptPointOptions("\nУкажите точку вставки (мышью или введите координаты): ");
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
                ed.WriteMessage($"\n[OK] {result.Message}");
                ed.WriteMessage($"\n     Блок: {result.BlockName}");
                ed.WriteMessage($"\n     ТИП: {result.TypeDesignation}");
                ed.WriteMessage($"\n     ПОЗ: {result.Position}");
                ed.WriteMessage($"\n     {contourService.GetStatusInfo()}");
            }
            else
            {
                ed.WriteMessage($"\n[ОШИБКА] {result.Message}");
            }
        }

        // Запрос обозначения типа с валидацией
        private string PromptForTypeDesignation(Editor ed)
        {
            while (true)
            {
                var typeOptions = new PromptStringOptions("\nВведите обозначение типа (ТИП) - только латинские буквы, 1-4 символа: ");
                typeOptions.AllowSpaces = false;
                typeOptions.DefaultValue = "TE";
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
                if (string.IsNullOrWhiteSpace(input))
                {
                    input = "TE";
                }

                input = input.ToUpperInvariant();

                var validation = ValidationService.ValidateTypeDesignation(input);
                if (validation.IsValid)
                {
                    if (!string.IsNullOrEmpty(validation.WarningMessage))
                    {
                        ed.WriteMessage($"\n[ПРЕДУПРЕЖДЕНИЕ] {validation.WarningMessage}");
                    }
                    return input;
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
                    // Обновляем счётчики контуров на основе введённой позиции
                    TryUpdateCounterFromPosition(contourService, input);

                    if (!string.IsNullOrEmpty(validation.WarningMessage))
                    {
                        ed.WriteMessage($"\n[ПРЕДУПРЕЖДЕНИЕ] {validation.WarningMessage}");
                    }

                    return input;
                }

                ed.WriteMessage($"\n[ОШИБКА] {validation.ErrorMessage}");
                ed.WriteMessage("\nПопробуйте снова или нажмите ESC для отмены.");
            }
        }

        // Пытается распарсить введённую позицию и обновить счётчик контура
        private void TryUpdateCounterFromPosition(ContourService service, string position)
        {
            if (!ValidationService.IsPositionFormat(position))
                return;

            int contour = ValidationService.ExtractContourNumber(position);
            int element = ValidationService.ExtractElementNumber(position);

            // Если введённый контур отличается от текущего - меняем текущий
            if (contour != service.GetCurrentContour())
            {
                service.SetCurrentContour(contour);
            }

            // Обновляем счётчик, если введённый элемент больше текущего
            var allContours = service.GetAllContours();
            if (allContours.ContainsKey(contour))
            {
                if (element >= allContours[contour])
                {
                    // Устанавливаем счётчик на введённое значение
                    service.ResetContour(contour);
                    for (int i = 0; i < element; i++)
                    {
                        service.GetNextPosition(contour);
                    }
                }
            }
            else
            {
                // Новый контур - инициализируем счётчик
                service.SetCurrentContour(contour);
                service.ResetContour(contour);
                for (int i = 0; i < element - 1; i++)
                {
                    service.GetNextPosition(contour);
                }
            }
        }
    }
}