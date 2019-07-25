using System;
using System.Collections.Generic;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.Services;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;

namespace UGRS.Core.SDK.DI.CreditNote.DAO
{
    public class CreditNoteDAO
    {
        QueryManager mObjQueryManager = new QueryManager();

        public string GetInvoicesQuery(DateTime pDtmDateTime)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("StartDate", "20190101");// pDtmDateTime.ToString("yyyyMMdd"));
            lLstStrParameters.Add("EndDate", "20190724");// pDtmDateTime.ToString("yyyyMMdd"));
            string lStrQuery = this.GetSQL("Exec_Invoices").Inject(lLstStrParameters);
            LogService.WriteInfo(lStrQuery);
            return lStrQuery;
        }
    }
}
