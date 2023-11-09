using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;


namespace WareMaster
{
    public class ItemInputValidator : AbstractValidator<Item>
    {
        public ItemInputValidator()
        {
            RuleFor(Item => Item.Itemname).NotNull().NotEmpty().Length(1, 200).Matches("^[a-zA-Z]+$");  // only contains letters
            RuleFor(Item => Item.Description).NotNull().NotEmpty().Length(1, 500).Matches("^[a-zA-Z\\s]+$");  // only contains letters and/or space
            RuleFor(Item => Item.Category_Id).NotNull().NotEmpty();  
            RuleFor(Item => Item.Unit).NotNull().NotEmpty();
            RuleFor(Item => Item.Location).NotNull().NotEmpty().Matches("^A\\d{1,2}$");  // A1 
        }
    }
}
