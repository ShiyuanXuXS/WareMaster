using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WareMaster
{
    public partial class Category
    {
        public override string ToString()
        {
            return $"Category ID: {id}, Category Name: {Category_Name}";
        }

        public static bool IsCategoryNameValid(string categoryname, out string error)
        {
            List<string> allNames = Globals.wareMasterEntities.Categories.Select(category => category.Category_Name.ToLower()).ToList();
            if (categoryname.Length < 1 || categoryname.Length > 200 || !Regex.IsMatch(categoryname, "^[a-zA-Z]+$"))
            {
                error = "Categoryname must be 5-45 characters long, only letters";
                return false;
            }
            else if (allNames.Contains(categoryname.ToLower()))
            {
                error = "Categoryname must be unique";
                return false;
            }
            error = null;
            return true;
        }
    }
}
