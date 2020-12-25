using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FootballFieldManagement.Validations
{
    class DateBookingValidation : ValidationRule
    {
        public string ErrorMessageNull { get; set; }
        public string ErrorMessage { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, this.ErrorMessageNull);
            }
            if (DateTime.Parse(value.ToString()) < DateTime.Today)
            {
                return new ValidationResult(false, this.ErrorMessage);
            }
            return new ValidationResult(true, null);
        }
    }
}
