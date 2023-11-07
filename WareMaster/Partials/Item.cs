using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WareMaster
{
    public partial class Item
    {
        
    

        public static bool IsItemNameValid(string itemname, out string error)
        {
            if (itemname.Length < 1 || itemname.Length > 200)
            {
                error = "Item Name must be 1-100 characters long, only letters";
                return false;
            }
            error = null;
            return true;
        }

    }
}
