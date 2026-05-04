namespace NanoCAD.API.Models
{
    /// <summary>
    /// Параметры для вставки блока
    /// </summary>
    public class BlockInsertOptions
    {
        public string BlockName { get; set; } = string.Empty; // Имя блока в файле blocks.dwg

        public string TypeDesignation { get; set; } = "TE";   // Обозначение типа (атрибут ТИП), по умолчанию - "TE"

        public string Position { get; set; } = "1-1";         // Позиционное обозначение (атрибут ПОЗ), по умолчанию - "1-1"
    }

    /// <summary>
    /// Результат вставки блока
    /// </summary>
    public class BlockInsertResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string BlockName { get; set; } = string.Empty;
        public string TypeDesignation { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }
}