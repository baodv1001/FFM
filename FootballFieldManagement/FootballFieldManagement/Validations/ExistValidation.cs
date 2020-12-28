using FootballFieldManagement.DAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FootballFieldManagement.Validations
{
    class ExistValidation : ValidationRule
    {
        public string ErrorMessage { get; set; }
        public string ErrorMessageNull { get; set; }
        public string ElementName { get; set; }


        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);
            if (value == null)
            {
                return new ValidationResult(true, null);
            }
            if (value.ToString() == "")
            {
                return new ValidationResult(false, this.ErrorMessageNull);
            }
            switch (ElementName)
            {
                case "UserName":
                    if (AccountDAL.Instance.IsExistUserName(value.ToString()))
                    {
                        result = new ValidationResult(false, this.ErrorMessage);
                    }
                    break;
                case "FieldName":
                    if (FootballFieldDAL.Instance.isExistFieldName(value.ToString()))
                    {
                        result = new ValidationResult(false, this.ErrorMessage);
                    }
                    break;
                case "GoodsName":
                    if (GoodsDAL.Instance.IsExistGoodsName(value.ToString()))
                    {
                        result = new ValidationResult(false, this.ErrorMessage);
                    }
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
