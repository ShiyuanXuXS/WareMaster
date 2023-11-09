using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WareMaster
{
    internal class Globals
    {
        static internal WareMasterEntities wareMasterEntities;
        public static WareMasterEntities DbContext //singleton pattern
        {
            get
            {
                if (wareMasterEntities == null)
                {
                    wareMasterEntities = new WareMasterEntities();
                }
                return wareMasterEntities;
            }
        }
    }
}
