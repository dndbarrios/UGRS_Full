using UGRS.Core.SDK.DI.Purchases.Services;
using UGRS.Core.SDK.DI.Purchases.Services.ServicesDAO;

namespace UGRS.Core.SDK.DI.Purchases
{
   public class PurchasesServiceFactory
    {
       public SetupService GetSetupService()
       {
           return new SetupService();
       }

       public VouchersService GetVouchersService()
       {
           return new VouchersService();
       }

       public VouchersDetailService GetVouchersDetailService()
       {
           return new VouchersDetailService();
       }

       public PurchaseNoteService GetPurchaseNoteService()
       {
           return new PurchaseNoteService();
       }

       public PurchaseXmlService GetPurchaseXmlService()
       {
           return new PurchaseXmlService();
       }

       public PurchaseInvoiceService GetPurchaseInvoiceService()
       {
           return new PurchaseInvoiceService();
       }

       public PurchasePaymentDIService GetPurchasePaymentService()
       {
           return new PurchasePaymentDIService();
       }

       public PurchaseReceiptsService GetPurchaseReceiptsService()
       {
           return new PurchaseReceiptsService();
       }

       public PurchaseMessageService GetPurchaseMessageService()
       {
           return new PurchaseMessageService();
       }

       public PurchaseVouchersService GetPurchaseVouchersService()
       {
           return new PurchaseVouchersService();
       }

       public PurchaseCheeckingCostService GetPurchaseCheeckingCostService()
       {
           return new PurchaseCheeckingCostService();
       }

       public PurchasePermissionService GetPurchasePermissionsService()
       {
           return new PurchasePermissionService();
       }

       public PurchaseService GetPurchaseService()
       {
           return new PurchaseService();
       }

       public AssetService GetAssetService()
       {
           return new AssetService();
       }
    }
}
