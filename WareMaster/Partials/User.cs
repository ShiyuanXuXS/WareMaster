using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WareMaster
{
    public partial class User
    {
        public RoleEnum RoleEnum { get; set; }

        public override string ToString()
        {
            return $"User ID: {id}, Username: {Username}, Role: {RoleEnum}, Email: {Email}";
        }
    }

}
