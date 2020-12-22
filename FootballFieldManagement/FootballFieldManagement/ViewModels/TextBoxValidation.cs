using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace FootballFieldManagement.ViewModels
{
    class TextBoxValidation : ValidationRule
    {
        public string ErrorMessage { get; set; }
        public string TypeValidation { get; set; } // 1: không nhập ký tự đặc biệt và không để khoảng trống; 2 : không để trống
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);
            if (value == null)
                return result;
            if (value.ToString().Length == 0)
            {
                result = new ValidationResult(false, "Vui lòng nhập thông tin!");
            }
            if (TypeValidation == "1")
            {
                Regex regex = new Regex(@"^[0-9]+$");
                bool isMatch = regex.IsMatch(value.ToString());
                result = new ValidationResult(isMatch, this.ErrorMessage);
            }
            return result;
        }
    }
}
