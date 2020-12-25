using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FootballFieldManagement.Validations
{
    class PickersValidation : ValidationRule
    {
        public string ErrorMessageNull { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(value == null)
            {
                return new ValidationResult(false, this.ErrorMessageNull);
            }
            return new ValidationResult(true, null);
        }
    }
}
