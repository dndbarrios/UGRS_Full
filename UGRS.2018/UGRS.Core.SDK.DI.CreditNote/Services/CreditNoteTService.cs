using UGRS.Core.SDK.DI.CreditNote.Tables;
using UGRS.Core.SDK.DI.DAO;

namespace UGRS.Core.SDK.DI.CreditNote.Services
{
    public class CreditNoteTService
    {
        private TableDAO<CreditNoteT> mObjCreditNoteTDAO;

        public CreditNoteTService()
        {
            mObjCreditNoteTDAO = new TableDAO<CreditNoteT>();
        }

        public int Add(CreditNoteT pObjCreditNoteT)
        {
            return mObjCreditNoteTDAO.Add(pObjCreditNoteT);
        }

        public int Update(CreditNoteT pObjCreditNoteT)
        {
            return mObjCreditNoteTDAO.Update(pObjCreditNoteT);
        }
    }
}
