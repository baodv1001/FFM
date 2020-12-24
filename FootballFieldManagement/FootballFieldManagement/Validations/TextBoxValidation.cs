using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace FootballFieldManagement.Validations
{
    public class TextBoxValidation : ValidationRule
    {
        public string ErrorMessage { get; set; }
        public string ErrorMessageNull { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);
            if (value == null)
                return result;
            if (value.ToString() == "")
            {
                return new ValidationResult(false, this.ErrorMessageNull);
            }
            Regex regex = new Regex(@"^[a-zA-Z0-9_]+$");
            return new ValidationResult(regex.IsMatch(value.ToString()), this.ErrorMessage);
        }
    }
}
