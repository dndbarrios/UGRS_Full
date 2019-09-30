using System.ComponentModel;

namespace UGRS.Core.SDK.DI.Purchases.Enums
{
    public class PermissionsEnum
    {
        public enum Permission : int
        {
            [Description("No permiso")]
            None = 0,
            [Description("Compras")]
            Purchase = 1,

            //[Description("Autorizar Compras")]
            //AuthorizePurchase = 2,

            [Description("Autorizar Operaciones")]
            AuthorizeOperations = 3

        }
    }
}
