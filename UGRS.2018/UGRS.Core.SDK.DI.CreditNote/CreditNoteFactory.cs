using UGRS.Core.SDK.DI.CreditNote.DOC;
using UGRS.Core.SDK.DI.CreditNote.Services;

namespace UGRS.Core.SDK.DI.CreditNote
{
    public class CreditNoteFactory
    {
        public CreditNoteService GetCreditNoteService()
        {
            return new CreditNoteService();
        }

        public SetupService GetSetupService()
        {
            return new SetupService();
        }

        public  CreditNoteDOC GetCreditNoteDocService()
        {
            return new CreditNoteDOC();  
        }
    }
}
