using System.Collections.Generic;
using UGRS.Core.SDK.DI.Purchases.DAO;
using UGRS.Core.SDK.DI.Purchases.DTO;

namespace UGRS.Core.SDK.DI.Purchases.Services.ServicesDAO
{
    public class PurchaseCheeckingCostService
    {
        private PurchaseCheeckingCostDAO lObjPurchaseCheeckingCost;

        public PurchaseCheeckingCostService()
        {
            lObjPurchaseCheeckingCost = new PurchaseCheeckingCostDAO();
        }

        public IList<PaymentDTO> GetPayment(string pStrCostCenter, string pStrStatus)
        {
            return lObjPurchaseCheeckingCost.GetPayment(pStrCostCenter, pStrStatus);
        }

        public string CheckingCost(string pStrCodeMov)
        {
            return lObjPurchaseCheeckingCost.CheckingCost(pStrCodeMov);
        }
    }
}
