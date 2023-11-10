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
            List<string> allNames = Globals.wareMasterEntities.Items.Select(item => item.Itemname.ToLower()).ToList();
            if (itemname.Length < 1 || itemname.Length > 200 || !Regex.IsMatch(itemname, "^[a-zA-Z]+$"))
            {
                error = "Item Name must be 1-200 characters long, only letters";
                return false;
            }
            else if (allNames.Contains(itemname.ToLower()))
            {
                error = "Itemname must be unique";
                return false;
            }
            error = null;
            return true;
        }

        public static bool IsDescriptionValid(string description, out string error)
        {
            if (description.Length < 1 || description.Length > 500 || !Regex.IsMatch(description, "^[a-zA-Z\\s]+$"))
            {
                error = "Description must be 1-500 characters long, contain only letters and/or space";
                return false;
            }
            error = null;
            return true;
        }

        public static bool IsCategoryValid(string category, out string error)
        {
            if (category == null)
            {
                error = "You must choose a category";
                return false;
            }
            error = null;
            return true;
        }

        public static bool IsUnitValid(string unit, out string error)
        {
            if (unit == null)
            {
                error = "You must choose a unit";
                return false;
            }
            error = null;
            return true;
        }

        public static bool IsLocationValid(string location, out string error)
        {
            if (location == null || !Regex.IsMatch(location, "^A\\d{1,2}$"))
            {
                error = "Location must be in the format 'A1'";
                return false;
            }
            error = null;
            return true;
        }
    }       
}
