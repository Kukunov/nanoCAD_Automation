namespace NanoCAD.API.Models
{
    /// Параметры для вставки блока
    public class BlockInsertOptions
    {
        /// Имя блока в файле blocks.dwg
        public string BlockName { get; set; } = string.Empty;

        /// Обозначение типа (атрибут ТИП)
        public string TypeDesignation { get; set; } = "TE";

        /// Позиционное обозначение (атрибут ПОЗ)
        public string Position { get; set; } = "1-1";
    }

    /// Результат вставки блока
    public class BlockInsertResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string BlockName { get; set; } = string.Empty;
        public string TypeDesignation { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }
}