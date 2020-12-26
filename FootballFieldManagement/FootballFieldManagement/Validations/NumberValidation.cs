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
    class NumberValidation : ValidationRule
    {
        public string ErrorMessage { get; set; }
        public string ErrorMessageNull { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(value == null)
            {
                return new ValidationResult(true, null);
            }
            if(value.ToString() == "")
            {
                return  new ValidationResult(false, this.ErrorMessageNull);
            }
            else
            {
                Regex regex = new Regex(@"^[0-9]+$");
                return new ValidationResult(regex.IsMatch(value.ToString()), this.ErrorMessage);
            }
        }
    }
}
