using UGRS.Core.SDK.DI.Purchases.DAO;
using UGRS.Core.SDK.DI.Purchases.Enums;

namespace UGRS.Core.SDK.DI.Purchases.Services.ServicesDAO
{
    public class PurchasePermissionService
    {
        private PurchasePermissionsDAO lObjPurchasePermission;

        public PurchasePermissionService()
        {
            lObjPurchasePermission = new PurchasePermissionsDAO();
        }

        public PermissionsEnum.Permission GetPermissionType(string pStrCostCenter, string pStrType, string pStrArea)
        {
            return lObjPurchasePermission.GetPermissionType(pStrCostCenter, pStrType, pStrArea);
        }

        
    }
}
