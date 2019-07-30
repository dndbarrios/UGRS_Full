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

        public CN_Doc GetCN_DocService()
        {
            return new CN_Doc();  
        }

        public CreditNoteTService GetCreditNoteTService()
        {
            return new CreditNoteTService();
        }

        public CreditNoteDocService GetCreditNoteDocService()
        {
            return new CreditNoteDocService();
        }

        public CreditNoteDetService GetCreditNoteDetService()
        {
            return new CreditNoteDetService();
        }
    }
}
