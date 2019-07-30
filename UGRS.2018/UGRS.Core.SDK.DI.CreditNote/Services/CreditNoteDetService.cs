using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI.CreditNote.Tables;
using UGRS.Core.SDK.DI.DAO;

namespace UGRS.Core.SDK.DI.CreditNote.Services
{
    public class CreditNoteDetService
    {
        private TableDAO<CreditNoteDet> mObjCreditNoteDetDAO;

        public CreditNoteDetService()
        {
            mObjCreditNoteDetDAO = new TableDAO<CreditNoteDet>();
        }

        public int Add(CreditNoteDet pObjCreditNoteDet)
        {
            return mObjCreditNoteDetDAO.Add(pObjCreditNoteDet);
        }

        public int Update(CreditNoteDet pObjCreditNoteDet)
        {
            return mObjCreditNoteDetDAO.Update(pObjCreditNoteDet);
        }
    }
}
