using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FootballFieldManagement.Validations
{
    class RangeOfNumberValidation : ValidationRule
    {
        public long Max { get; set; }
        public long Min { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorMessageNull { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(true, null);
            }
            if (value.ToString() == "")
            {
                return new ValidationResult(false, this.ErrorMessageNull);
            }
            Regex regex = new Regex(@"^[0-9]+$");
            if (!regex.IsMatch(value.ToString()) || (long.Parse(value.ToString()) < Min || long.Parse(value.ToString()) > Max))
            {
                return new ValidationResult(false, this.ErrorMessage);
            }
            return new ValidationResult(true, null);
        }
    }
}
