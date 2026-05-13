using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teigha.DatabaseServices;
using NanoCAD.API.Models;

namespace NanoCAD.API.Services
{
    internal class ContourReportService
    {
        private readonly Database _db;

        public ContourReportService(Database db)
        {
            _db = db;
        }

        // Модель для контуров в таблице
        public class ContourTableRow
        {
            public int ContourNumber { get; set; }
            public int Counter { get; set; }
            public int ActualBlocks { get; set; }
            public string Types { get; set; } = "";
            public string ColorName { get; set; } = "";
            public string LastInsert { get; set; } = "";
        }

        // Модель для прибора в контуре
        public class DeviceInfo
        {
            public string Position { get; set; } = "";
            public string BlockName { get; set; } = "";
            public string TypeDesignation { get; set; } = "";
        }

        // Экспорт в CSV (плоская таблица)
        public List<string> GetContourCsv()
        {
            var lines = new List<string>();
            var colorService = new ContourColorService();
            var contourBlocks = colorService.GetContourBlocks(_db);

            lines.Add("Контур;ПОЗ;Блок;ТИП;Цвет");

            foreach (var kvp in contourBlocks.OrderBy(c => c.Key))
            {
                int contour = kvp.Key;
                string colorName = ContourColorService.GetColorName(contour);
                var devices = GetDevicesForContour(contour);

                foreach (var device in devices)
                {
                    lines.Add($"{contour};\t{device.Position};{device.BlockName};{device.TypeDesignation};{colorName}");
                }
            }

            return lines;
        }

        // Получить данные для таблицы контуров
        public List<ContourTableRow> GetContourTableData()
        {
            var contourService = new ContourService(_db);
            var result = new List<ContourTableRow>();
            var allContours = contourService.GetAllContours();
            var colorService = new ContourColorService();
            var contourBlocks = colorService.GetContourBlocks(_db);

            var allNumbers = new HashSet<int>();
            foreach (var key in allContours.Keys) allNumbers.Add(key);
            foreach (var key in contourBlocks.Keys) allNumbers.Add(key);

            foreach (int contour in allNumbers.OrderBy(c => c))
            {
                int counter = allContours.ContainsKey(contour) ? allContours[contour] : 0;
                int actual = contourBlocks.ContainsKey(contour) ? contourBlocks[contour].Count : 0;
                string types = GetTypesForContour(contour, contourBlocks);
                string colorName = ContourColorService.GetColorName(contour);
                string lastInsert = counter > 0 ? $"{contour}-{counter}" : "—";

                result.Add(new ContourTableRow
                {
                    ContourNumber = contour,
                    Counter = counter,
                    ActualBlocks = actual,
                    Types = types,
                    ColorName = colorName,
                    LastInsert = lastInsert
                });
            }

            return result;
        }

        // Получить отчёт по контурам в виде списка строк
        public List<string> GetContourReport()
        {
            var contourService = new ContourService(_db);
            var lines = new List<string>();
            var allContours = contourService.GetAllContours();
            var colorService = new ContourColorService();
            var contourBlocks = colorService.GetContourBlocks(_db);

            var allNumbers = new HashSet<int>();
            foreach (var key in allContours.Keys) allNumbers.Add(key);
            foreach (var key in contourBlocks.Keys) allNumbers.Add(key);

            lines.Add("=== Сводка по контурам ===");
            lines.Add($"Дата экспорта: {DateTime.Now:dd.MM.yyyy HH:mm}");
            lines.Add("");
            lines.Add($"Всего контуров автоматизации: {contourService.GetTotalContourCount()}");
            lines.Add($"Всего приборов в контурах: {contourService.GetTotalBlockCount()}");
            lines.Add("");
            lines.Add("Контур | Учтено | На чертеже | Типы (ТИП) | Цвет");
            lines.Add("-------|--------|------------|------------|-----");

            foreach (int contour in allNumbers.OrderBy(c => c))
            {
                int counter = allContours.GetValueOrDefault(contour);
                int actual = contourBlocks.ContainsKey(contour) ? contourBlocks[contour].Count : 0;
                string types = GetTypesForContour(contour, contourBlocks);
                string color = ContourColorService.GetColorName(contour);
                string lastInsert = counter > 0 ? $"{contour}-{counter}" : "—";

                lines.Add($"{contour,-6} | {lastInsert,-6} | {actual,-10} | {types,-10} | {color}");
            }

            lines.Add("");
            lines.Add("=== Детализация по контурам ===");
            lines.Add("");

            foreach (int contour in allNumbers.OrderBy(c => c))
            {
                var devices = GetDevicesForContour(contour);
                if (devices.Count == 0) continue;

                lines.Add($"Контур {contour}:");
                foreach (var device in devices)
                    lines.Add($"  {device.Position}: {device.BlockName} (ТИП: {device.TypeDesignation})");
                lines.Add("");
            }

            return lines;
        }

        // Получить список приборов для указанного контура из чертежа
        public List<DeviceInfo> GetDevicesForContour(int contour)
        {
            var result = new List<DeviceInfo>();
            var colorService = new ContourColorService();
            var contourBlocks = colorService.GetContourBlocks(_db);

            if (!contourBlocks.ContainsKey(contour)) return result;

            using (var tr = _db.TransactionManager.StartTransaction())
            {
                foreach (var blockId in contourBlocks[contour])
                {
                    var blockRef = (BlockReference)tr.GetObject(blockId, OpenMode.ForRead);
                    var blockDef = (BlockTableRecord)tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead);

                    string position = "";
                    string typeDesignation = "";

                    foreach (ObjectId attId in blockRef.AttributeCollection)
                    {
                        var attRef = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                        if (attRef == null) continue;

                        string tag = attRef.Tag.ToUpperInvariant();
                        if (tag == "ПОЗ") position = attRef.TextString;
                        if (tag == "ТИП") typeDesignation = attRef.TextString;
                    }

                    result.Add(new DeviceInfo
                    {
                        Position = position,
                        BlockName = blockDef.Name == "ПриборНаЩите" ? "ПНЩ" : "ПВЩ",
                        TypeDesignation = string.IsNullOrEmpty(typeDesignation) ? "—" : typeDesignation
                    });
                }
                tr.Commit();
            }

            return result;
        }

        // Собрать типы ТИП для контура
        private string GetTypesForContour(int contour, Dictionary<int, List<ObjectId>> contourBlocks)
        {
            var types = new HashSet<string>();

            if (!contourBlocks.ContainsKey(contour)) return "—";

            using (var tr = _db.TransactionManager.StartTransaction())
            {
                foreach (var blockId in contourBlocks[contour])
                {
                    var blockRef = (BlockReference)tr.GetObject(blockId, OpenMode.ForRead);
                    foreach (ObjectId attId in blockRef.AttributeCollection)
                    {
                        var attRef = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                        if (attRef != null && attRef.Tag.ToUpperInvariant() == "ТИП")
                        {
                            types.Add(attRef.TextString);
                            break;
                        }
                    }
                }
                tr.Commit();
            }

            return types.Count > 0 ? string.Join(", ", types) : "—";
        }
    }
}
