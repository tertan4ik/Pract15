using System.Globalization;
using System.Windows.Controls;

namespace pract_15.Validators
{
    public class RangeValidator : ValidationRule
    {
        public double Min { get; set; }
        public double Max { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!double.TryParse(value?.ToString(), out double number))
                return new ValidationResult(false, "Введите число");

            if (number < Min || number > Max)
                return new ValidationResult(false, $"Значение от {Min} до {Max}");

            return ValidationResult.ValidResult;
        }
    }
}
