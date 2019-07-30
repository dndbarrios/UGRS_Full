using System;
using System.Collections.Generic;
using UGRS.Core.SDK.DI.CreditNote.DAO;
using UGRS.Core.SDK.DI.CreditNote.DOC;
using UGRS.Core.SDK.DI.CreditNote.Tables;

namespace UGRS.Core.SDK.DI.CreditNote.Services
{
    public class CreditNoteService
    {
        private CreditNoteDAO mObjCreditNoteDAO;
        private CN_Doc mObjCreditNoteDOC;

        public CreditNoteService()
        {
            mObjCreditNoteDAO = new CreditNoteDAO();
            mObjCreditNoteDOC = new CN_Doc();
        }

        public string GetInvoiceQuery(DateTime pDtmDate)
        {
            return mObjCreditNoteDAO.GetInvoicesQuery(pDtmDate);
        }

        public bool GetCreditNoteDOC(CreditNoteDoc pLstCreditNoteDTO)
        {
            return mObjCreditNoteDOC.CreateCreditNote(pLstCreditNoteDTO);
        }

        public string GetBonusItemCode()
        {
            return mObjCreditNoteDAO.GetBonusItemCode();
        }

        public int GetLastCode()
        {
            return mObjCreditNoteDAO.GetLastCode();
        }

        public int GetFirstCode()
        {
            return mObjCreditNoteDAO.GetFirstCode();
        }
    }
}
