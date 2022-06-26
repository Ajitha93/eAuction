using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAuction.Models.ModelValidations
{
    public class CategoryValidation : ValidationAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return "Category value is invalid";
        }

        protected override ValidationResult IsValid(object objValue,
                                                       ValidationContext validationContext)
        {
            var catValue = objValue as string;


            if (!(catValue == "Painting" || catValue == "Sculpture" || catValue == "Ornament"))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }
    }
}
