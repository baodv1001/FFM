using System;
using System.Collections.Generic;
using System.Globalization;
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
                foreach (char i in value.ToString())
                {
                    if (!((i >= 48 && i <= 57) || (i >= 65 && i <= 90) || (i >= 97 && i <= 122)))
                    {
                        result = new ValidationResult(false, this.ErrorMessage);
                    }
                }
            }
            return result;
        }
    }
}
