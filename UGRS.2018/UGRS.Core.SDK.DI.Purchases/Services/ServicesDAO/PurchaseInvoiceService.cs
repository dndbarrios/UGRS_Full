using UGRS.Core.SDK.DI.Purchases.DAO;
using UGRS.Core.SDK.DI.Purchases.DTO;
using UGRS.Core.SDK.DI.Purchases.Tables;

namespace UGRS.Core.SDK.DI.Purchases.Services.ServicesDAO
{
    public class PurchaseInvoiceService
    {
        private PurchaseInvoiceDAO lObjPurchaseInvoice;
        private InvoiceDI lObjInvoiceDI;

        public PurchaseInvoiceService()
        {
            lObjPurchaseInvoice = new PurchaseInvoiceDAO();
            lObjInvoiceDI = new InvoiceDI();
        }

        public string GetCostCenter()
        {
            return lObjPurchaseInvoice.GetCostCenter();
        }

        public string CCTypeCode(string pStrPrcCode)
        {
            return lObjPurchaseInvoice.GetCostCenterAdmOpe(pStrPrcCode);
        }

        public string GetCostAccount(string pStrItemCode)
        {
            return lObjPurchaseInvoice.GetCostAccount(pStrItemCode);
        }

        public string GetWhouse(string pStrCostingCode)
        {
            return lObjPurchaseInvoice.GetWhouse(pStrCostingCode);
        }

        public string GetDocNum(string pStrDocEntry)
        {
            return lObjPurchaseInvoice.GetDocNum(pStrDocEntry);
        }

        public string GetDocStatus(string pStrDocEntry)
        {
            return lObjPurchaseInvoice.GetDocStatus(pStrDocEntry);
        }

        public string GetDocCanceled(string pStrDocEntry, string pStrType)
        {
            return lObjPurchaseInvoice.GetDocCanceled(pStrDocEntry, pStrType);
        }

        public string GetTaxCode(string pStrRate)
        {
            return lObjPurchaseInvoice.GetTaxCode(pStrRate);
        }

        public string GetWithholdingTaxCodeBP(double pDblRate, string pStrCardCode)
        {
            return lObjPurchaseInvoice.GetWithholdingTaxCodeBP(pDblRate, pStrCardCode);
        }

        public string GetWithholdingTaxCode(double pDblRate)
        {
            return lObjPurchaseInvoice.GetWithholdingTaxCode(pDblRate);
        }

        public string GetItemIEPS()
        {
            return lObjPurchaseInvoice.GetItemIEPS();
        }

        public string GetLocalTaxt()
        {
            return lObjPurchaseInvoice.GetLocalTax();
        }

        public bool CreateDocument(PurchaseXMLDTO pObjPurchaseXmlDTO)
        {
            return lObjInvoiceDI.CreateDocument(pObjPurchaseXmlDTO, true);
        }

        public string GetMQWhs(string pStrItemCode)
        {
            return lObjPurchaseInvoice.GetWhsMQ(pStrItemCode);
        }

        public bool UpdateStatus(VouchersDetail pObjVouchersDetail)
        {
            return lObjPurchaseInvoice.UpdateStatus(pObjVouchersDetail);
        }

        public string GetCreatedInvoice(string pStrVoucherCode, string pStrVoucherLine)
        {
            return lObjPurchaseInvoice.GetCreatedInvoice(pStrVoucherCode, pStrVoucherLine);
        }

    }
}
