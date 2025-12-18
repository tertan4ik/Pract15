using System.Globalization;
using System.Windows.Controls;

namespace pract_15.Validators
{
    public class NotEmptyValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult(false, "Поле не может быть пустым");

            return ValidationResult.ValidResult;
        }
    }
}
