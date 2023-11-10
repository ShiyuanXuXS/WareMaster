using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WareMaster
{
    public class CategoryInputValidator : AbstractValidator<Category>
    {
        public CategoryInputValidator() 
        {
            RuleFor(Category => Category.Category_Name).NotNull().NotEmpty().Length(1, 200).Matches("^[a-zA-Z]+$").Must((category, Category_Name) => IsCategorynameUnique(Category_Name))
                .WithMessage("Category name must be unique");  // only contains letters
        }
        private bool IsCategorynameUnique(string categoryname)
        {
            try
            {
                List<string> allNames = Globals.wareMasterEntities.Categories.Select(category => category.Category_Name.ToLower()).ToList();
                return !allNames.Contains(categoryname.ToLower());
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
