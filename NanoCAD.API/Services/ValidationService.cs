using System.Text.RegularExpressions;
using NanoCAD.API.Models;

namespace NanoCAD.API.Services
{
    // Сервис для валидации вводимых данных
    public static class ValidationService
    {
        // Регулярное выражение для ТИП: только латинские буквы, от 1 до 4 символов
        private static readonly Regex TypeRegex = new Regex(@"^[A-Za-z]{1,4}$", RegexOptions.Compiled);

        // Регулярное выражение для ПОЗ: формат "контур-номер" (например, 1-1, 25-8, 100-15)
        private static readonly Regex PositionRegex = new Regex(@"^([1-9][0-9]{0,2})-([1-9][0-9]{0,2})$", RegexOptions.Compiled);

        // Проверить обозначение типа (ТИП)
        public static ValidationResult ValidateTypeDesignation(string typeDesignation)
        {
            if (string.IsNullOrWhiteSpace(typeDesignation))
            {
                return ValidationResult.Error("Обозначение типа не может быть пустым.");
            }

            if (!TypeRegex.IsMatch(typeDesignation))
            {
                return ValidationResult.Error(
                    "Обозначение типа должно содержать только латинские буквы и быть длиной от 1 до 4 символов.\n");
            }

            return ValidationResult.Success();
        }

        // Проверить позиционное обозначение (ПОЗ)
        public static ValidationResult ValidatePosition(string position)
        {
            if (string.IsNullOrWhiteSpace(position))
            {
                return ValidationResult.Error("Позиционное обозначение не может быть пустым.");
            }

            var match = PositionRegex.Match(position);
            if (!match.Success)
            {
                return ValidationResult.Error(
                    "Позиционное обозначение должно быть в формате 'контур-номер'.\n");
            }

            // Дополнительная проверка диапазонов
            if (int.TryParse(match.Groups[1].Value, out int contour) && contour > 999)
            {
                return ValidationResult.Error("Номер контура не может превышать 999.");
            }

            if (int.TryParse(match.Groups[2].Value, out int element) && element > 999)
            {
                return ValidationResult.Error("Номер элемента не может превышать 999.");
            }

            return ValidationResult.Success();
        }

        // Проверить и ТИП, и ПОЗ
        public static ValidationResult ValidateAll(string typeDesignation, string position)
        {
            var typeResult = ValidateTypeDesignation(typeDesignation);
            if (!typeResult.IsValid)
            {
                return typeResult;
            }

            var positionResult = ValidatePosition(position);
            if (!positionResult.IsValid)
            {
                return positionResult;
            }

            return ValidationResult.Success();
        }

        // Извлечь номер контура из позиционного обозначения
        public static int ExtractContourNumber(string position)
        {
            var match = PositionRegex.Match(position);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int contour))
            {
                return contour;
            }
            return 1; // По умолчанию
        }

        // Извлечь номер элемента из позиционного обозначения
        public static int ExtractElementNumber(string position)
        {
            var match = PositionRegex.Match(position);
            if (match.Success && int.TryParse(match.Groups[2].Value, out int element))
            {
                return element;
            }
            return 1; // По умолчанию
        }

        // Проверить, соответствует ли строка формату ПОЗ (без строгой валидации)
        public static bool IsPositionFormat(string position)
        {
            return PositionRegex.IsMatch(position);
        }
    }
}