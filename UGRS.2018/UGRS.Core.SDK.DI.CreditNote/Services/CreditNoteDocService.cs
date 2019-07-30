using UGRS.Core.SDK.DI.CreditNote.Tables;
using UGRS.Core.SDK.DI.DAO;

namespace UGRS.Core.SDK.DI.CreditNote.Services
{
    public class CreditNoteDocService
    {
        private TableDAO<CreditNoteDoc> mObjCreditNoteDocDAO;

        public CreditNoteDocService()
        {
            mObjCreditNoteDocDAO = new TableDAO<CreditNoteDoc>();
        }

        public int Add(CreditNoteDoc pObjCreditNoteT)
        {
            return mObjCreditNoteDocDAO.Add(pObjCreditNoteT);
        }

        public int Update(CreditNoteDoc pObjCreditNoteT)
        {
            return mObjCreditNoteDocDAO.Update(pObjCreditNoteT);
        }
    }

}
