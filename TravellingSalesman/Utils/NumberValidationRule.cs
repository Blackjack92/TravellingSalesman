using System.Globalization;
using System.Windows.Controls;

namespace TravellingSalesman.Utils
{
    public class NumberValidationRule : ValidationRule
    {
        /// <summary>
        ///     Lower bound of the number.
        /// </summary>
        public int LowerBound { get; set; }

        /// <summary>
        ///     Upper bound of the number.
        /// </summary>
        public int UpperBound { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // Is the value a number?
            int number;
            if (!int.TryParse((string)value, out number))
            {
                var msg = $"{value} is not a number.";
                return new ValidationResult(false, msg);
            }

            // Is number in the expected range
            if (number < LowerBound || number > UpperBound)
            {
                var msg = $"Number must be between {LowerBound} - {UpperBound}.";
                return new ValidationResult(false, msg);
            }

            // Number is valid
            return ValidationResult.ValidResult;
        }
    }
}
