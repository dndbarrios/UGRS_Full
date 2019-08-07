using System;
using System.Collections.Generic;
using UGRS.Core.SDK.DI.CreditNote.DAO;
using UGRS.Core.SDK.DI.CreditNote.DOC;
using UGRS.Core.SDK.DI.CreditNote.DTO;
using UGRS.Core.SDK.DI.CreditNote.Tables;

namespace UGRS.Core.SDK.DI.CreditNote.Services
{
    public class CreditNoteService
    {
        private CreditNoteDAO mObjCreditNoteDAO;
        private CN_Doc mObjCreditNoteDOC;
        private CN_DocUI mObjCreditNoteUI;

        public CreditNoteService()
        {
            mObjCreditNoteDAO = new CreditNoteDAO();
            mObjCreditNoteDOC = new CN_Doc();
             mObjCreditNoteUI = new CN_DocUI();
        }

        public string GetInvoiceQuery(string pStrDateFrom, string pStrDateTo )
        {
            return mObjCreditNoteDAO.GetInvoicesQuery(pStrDateFrom, pStrDateTo);
        }

        public string GetReportSavedQuery(string pNcId)
        {
            return mObjCreditNoteDAO.GetReportSavedQuery(pNcId);
        }

        public List<string> CreateCreditNoteDOC(CreditNoteDoc pLstCreditNoteDTO, List<string> pLstErrors)
        {
            return mObjCreditNoteDOC.CreateCreditNote(pLstCreditNoteDTO, pLstErrors);
        }

        public void UpdateDocRel(CreditNoteDoc pObjCreditNote)
        {
            mObjCreditNoteUI.DocRelByUI(pObjCreditNote);
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

        public string GetTaxCode(string pStrRate)
        {
            return mObjCreditNoteDAO.GetTaxCode(pStrRate);
        }

        public CreditNoteT GetCreditNoteTSaved(string pStrId)
        {
            return mObjCreditNoteDAO.GetCreditNoteTSaved(pStrId);
        }

        public List<CreditNoteDoc> GetCreditNoteDocSaved(string pStrId)
        {
            return mObjCreditNoteDAO.GetCreditNoteDocSaved(pStrId);
        }

        public List<CreditNoteDet> GetCreditNoteDetSaved(string pStrId)
        {
            return mObjCreditNoteDAO.GetCreditNoteDetSaved(pStrId);
        }

        public List<DraftReferenceDTO> GetDraftReference(string pStrNcId)
        {
            return mObjCreditNoteDAO.GetDraftRelation(pStrNcId);
        }

        public int SaveDraftToDocument(int pIntDocEntryDraft)
        {
            return mObjCreditNoteDOC.DraftToDocument(pIntDocEntryDraft);
        }

        public int RemoveDraft(int pIntDocEntyDraft)
        {
            return mObjCreditNoteDOC.DeleteDraft(pIntDocEntyDraft);
        }
        

        public string GetAttachPath()
        {
            return mObjCreditNoteDAO.GetAttachPath();
        }

    }
}
