using System.Globalization;
using System.Windows.Controls;

namespace pract_15.Validators
{
    public class PositiveNumberValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!double.TryParse(value?.ToString(), out double number))
                return new ValidationResult(false, "Введите число");

            if (number <= 0)
                return new ValidationResult(false, "Число должно быть больше 0");

            return ValidationResult.ValidResult;
        }
    }
}
