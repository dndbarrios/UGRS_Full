
using UGRS.Core.SDK.DI.CreditNote.Tables;
using UGRS.Core.SDK.DI.DAO;
namespace UGRS.Core.SDK.DI.CreditNote.Services
{
    public class SetupService
    {
        private TableDAO<CreditNoteT> mObjCreditNoteTDAO;
        private TableDAO<CreditNoteDet> mObjCreditNoteDetailDAO;
        private TableDAO<CreditNoteDoc> mObjCrediNoteDoc;

        public SetupService()
        {
            mObjCreditNoteTDAO = new TableDAO<CreditNoteT>();
            mObjCreditNoteDetailDAO = new TableDAO<CreditNoteDet>();
            mObjCrediNoteDoc = new TableDAO<CreditNoteDoc>();
        }

        public void InitializeTables()
        {
            mObjCreditNoteDetailDAO.Initialize();
            mObjCreditNoteTDAO.Initialize();
            mObjCrediNoteDoc.Initialize();
        }

        
    }
}
