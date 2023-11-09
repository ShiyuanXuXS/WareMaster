using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace WareMaster
{
    public class UserInputValidator : AbstractValidator<User>
    {
        public UserInputValidator()
        {
            RuleFor(User => User.Username).NotNull().NotEmpty().Length(5, 45).Matches("^[a-zA-Z]+$");  // only contains letters
            RuleFor(User => User.Role).NotNull();
            RuleFor(User => User.Password).NotNull().NotEmpty().Length(1, 64).Matches("[a-zA-Z0-9]+$");  // only contains letters and numbers
            RuleFor(User => User.Email).NotNull().NotEmpty().Length(1, 200).EmailAddress();
        }
    }
}
