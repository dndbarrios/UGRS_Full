using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.UI;

namespace UGRS.AddOn.Purchases.Utilities
{
    public class UIUtility
    {
        public static bool IsFormOpen(string typeEx)
        {
            var count = UIApplication.GetApplication().Forms.Cast<IForm>().Count(p => p.TypeEx.Equals(typeEx));

            return count > 0;
        }
    }
}
