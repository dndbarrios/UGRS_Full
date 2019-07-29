using System;
using UGRS.Core.SDK.DI.CreditNote.DAO;
using UGRS.Core.SDK.DI.CreditNote.DOC;
using UGRS.Core.SDK.DI.CreditNote.DTO;

namespace UGRS.Core.SDK.DI.CreditNote.Services
{
    public class CreditNoteService
    {
        private CreditNoteDAO mObjCreditNoteDAO;
        private CreditNoteDOC mObjCreditNoteDOC;

        public CreditNoteService()
        {
            mObjCreditNoteDAO = new CreditNoteDAO();
            mObjCreditNoteDOC = new CreditNoteDOC();
        }

        public string GetInvoiceQuery(DateTime pDtmDate)
        {
            return mObjCreditNoteDAO.GetInvoicesQuery(pDtmDate);
        }

        public bool GetCreditNoteDOC(CreditNoteDTO pObjCreditNoteDTO)
        {
            return mObjCreditNoteDOC.CreateCreditNote(pObjCreditNoteDTO);
        }

        public string GetBonusItemCode()
        {
            return mObjCreditNoteDAO.GetBonusItemCode();
        }
       
    }
}
