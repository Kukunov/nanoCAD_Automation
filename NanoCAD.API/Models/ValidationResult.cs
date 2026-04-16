namespace NanoCAD.API.Models
{
    // Результат валидации
    public class ValidationResult
    {
        public bool IsValid { get; set; } = true;
        public string ErrorMessage { get; set; } = string.Empty;
        public string WarningMessage { get; set; } = string.Empty;

        public static ValidationResult Success()
        {
            return new ValidationResult { IsValid = true };
        }

        public static ValidationResult Error(string message)
        {
            return new ValidationResult { IsValid = false, ErrorMessage = message };
        }

        public static ValidationResult Warning(string message)
        {
            return new ValidationResult { IsValid = true, WarningMessage = message };
        }
    }
}