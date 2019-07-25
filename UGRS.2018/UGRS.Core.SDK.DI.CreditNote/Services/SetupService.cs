
using UGRS.Core.SDK.DI.CreditNote.Tables;
using UGRS.Core.SDK.DI.DAO;
namespace UGRS.Core.SDK.DI.CreditNote.Services
{
    public class SetupService
    {
        private TableDAO<CreditNoteT> mObjCreditNoteTDAO;
        private TableDAO<CreditNoteDet> mObjCreditNoteDetailDAO;

        public SetupService()
        {
            mObjCreditNoteTDAO = new TableDAO<CreditNoteT>();
            mObjCreditNoteDetailDAO = new TableDAO<CreditNoteDet>();
        }

        public void InitializeTables()
        {
            mObjCreditNoteDetailDAO.Initialize();
            mObjCreditNoteTDAO.Initialize();
        }

        
    }
}
