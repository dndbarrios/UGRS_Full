using System;
using System.Collections.Generic;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.Services;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.CreditNote.Utils;

namespace UGRS.Core.SDK.DI.CreditNote.DAO
{
    public class CreditNoteDAO
    {
        QueryManager mObjQueryManager = new QueryManager();

        public string GetInvoicesQuery(DateTime pDtmDateTime)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("StartDate", "20190601");// pDtmDateTime.ToString("yyyyMMdd"));
            lLstStrParameters.Add("EndDate", "20190730");// pDtmDateTime.ToString("yyyyMMdd"));
            string lStrQuery = this.GetSQL("Exec_Invoices").Inject(lLstStrParameters);
            LogService.WriteInfo(lStrQuery);
            return lStrQuery;
        }

        public string GetBonusItemCode()
        {
            return mObjQueryManager.GetValue("U_VALUE", "Name", Constants.STR_ENTRY_BONUS, Constants.STR_CONFIG_TABLE);
        }

        public int GetLastCode()
        {
            string lStrLastDispId = "0";
            lStrLastDispId = mObjQueryManager.Max<string>("U_NcId", "[@UG_PE_NC]");
            lStrLastDispId = string.IsNullOrEmpty(lStrLastDispId) ? "0" : lStrLastDispId.Substring(3);
            return Convert.ToInt32(lStrLastDispId);

        }

        public int GetFirstCode()
        {
            string lStrLastDispId = "0";
            lStrLastDispId = mObjQueryManager.Min<string>("U_NcId", "[@UG_PE_NC]");
            lStrLastDispId = string.IsNullOrEmpty(lStrLastDispId) ? "0" : lStrLastDispId.Substring(3);
            
            return Convert.ToInt32(lStrLastDispId);
        }
    }
}
