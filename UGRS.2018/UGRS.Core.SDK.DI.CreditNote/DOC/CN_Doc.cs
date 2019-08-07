using System;
using System.Collections.Generic;
using UGRS.Core.SDK.DI.CreditNote.Tables;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;


namespace UGRS.Core.SDK.DI.CreditNote.DOC
{
    public class CN_Doc
    {
        CreditNoteFactory mObjFactory = new CreditNoteFactory();
       // bool mBolQuestionFlag = true;

        /// <summary>
        /// Crea un pago
        /// </summary>
        /// 
        public List<string> CreateCreditNote(CreditNoteDoc pObjCreditNoteDoc, List<string> pLstErrors)
        {
            try
            {
                //Documents lObjDraft = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes);
                SAPbobsCOM.Documents lObjDraft = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                // SAPbobsCOM.Payments lObjPayment = (SAPbobsCOM.Payments)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oVendorPayments);
                lObjDraft.CardCode = pObjCreditNoteDoc.CardCode;
                lObjDraft.UserFields.Fields.Item("U_MQ_OrigenFol").Value = pObjCreditNoteDoc.NcId;
                lObjDraft.UserFields.Fields.Item("U_MQ_OrigenFol_Det").Value = pObjCreditNoteDoc.FolioDoc;
                lObjDraft.UserFields.Fields.Item("U_B1SYS_MainUsage").Value = "G02";
                lObjDraft.DocObjectCode = SAPbobsCOM.BoObjectTypes.oCreditNotes;
                lObjDraft.Lines.ItemCode = mObjFactory.GetCreditNoteService().GetBonusItemCode();
                lObjDraft.Lines.Quantity = 1;
                lObjDraft.Lines.UnitPrice = pObjCreditNoteDoc.Amount;
                lObjDraft.Lines.TaxCode = pObjCreditNoteDoc.TaxCode;
                lObjDraft.Lines.Add();

                int intError = lObjDraft.Add();
                string lStrErrMsg;
                if (intError != 0)
                {
                    DIApplication.Company.GetLastError(out intError, out lStrErrMsg);
                    LogService.WriteError(lStrErrMsg);
                    pLstErrors.Add(lStrErrMsg);
                    UIApplication.ShowError(lStrErrMsg);
                }
                else
                {
                    string pStrNewDocEntry = DIApplication.Company.GetNewObjectKey();
                    pObjCreditNoteDoc.DocEntryDraft = pStrNewDocEntry;
                    pObjCreditNoteDoc.IsDraft = "Y";
                    int i = mObjFactory.GetCreditNoteDocService().Update(pObjCreditNoteDoc);

                    //DocRelByUI(pStrNewDocEntry, pObjCreditNoteDoc.LstCreditNoteDet);
                    // LogService.WriteSuccess("pago creado correctamente: InvoiceDocEntry: " + pObjPurchase.DocEntry);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("Error al crear el borrador {1} Error {0}", ex.Message, pObjCreditNoteDoc.FolioDoc));
                LogService.WriteError(ex);
                pLstErrors.Add(ex.Message);

            }
            return pLstErrors;
        }

        //private void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        //{
        //    BubbleEvent = true;
           
        //    if (mBolQuestionFlag == true && pVal.FormTypeEx == "0")// && pVal.FormTypeCount == 2 && pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD && pVal.Action_Success == false)
        //    {
        //        try
        //        {
        //            DateTime lDtmStart = DateTime.Now;
        //            var mFormQuestion = UIApplication.GetApplication().Forms.ActiveForm;
        //            if (mFormQuestion.Modal == true && mFormQuestion.Title == "Nota de crédito de clientes - Borrador" && pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_ACTIVATE)
        //            {
        //                StaticText llblQuestion = ((SAPbouiCOM.StaticText)(mFormQuestion.Items.Item("7").Specific));
        //                if (llblQuestion.Caption == "Este documento no puede modificarse tras la creación. ¿Continuar?")
        //                {
        //                    Button BtnQuestionYes = ((SAPbouiCOM.Button)(mFormQuestion.Items.Item("1").Specific));
        //                    BtnQuestionYes.Item.Click();
        //                }
        //                else if (llblQuestion.Caption == "Se calculará retención de impuestos. ¿Desea abrir la tabla de retención de impuestos")
        //                {
        //                    Button BtnQuestionNo = ((SAPbouiCOM.Button)(mFormQuestion.Items.Item("2").Specific));
        //                    BtnQuestionNo.Item.Click();
        //                }


                      
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            LogService.WriteError("Question" + ex.Message);
        //            UIApplication.ShowError("Question " + ex.Message);
        //        }
        //        finally
        //        {


        //        }


        //        //txtCardCode = ((SAPbouiCOM.EditText)(mFormInvoice.Items.Item("4").Specific));
        //    }
        //}


        public int DraftToDocument(int pIntDocEntryDraft)
        {
            SAPbobsCOM.Documents lObjDraft = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
            lObjDraft.GetByKey(pIntDocEntryDraft);
           
            lObjDraft.Lines.SetCurrentLine(0);
            lObjDraft.Lines.WTLiable = SAPbobsCOM.BoYesNoEnum.tNO;
            lObjDraft.SaveXML(@"C:\sss"); 
            lObjDraft.UserFields.Fields.Item("U_B1SYS_MainUsage").Value = "G02";
            lObjDraft.EDocGenerationType = SAPbobsCOM.EDocGenerationTypeEnum.edocGenerateLater;
          
            return lObjDraft.SaveDraftToDocument();

        }

        public int DeleteDraft(int pIntDocEntryDraft)
        {
            SAPbobsCOM.Documents lObjDraft = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
            lObjDraft.GetByKey(pIntDocEntryDraft);
            return lObjDraft.Remove();
        }

    }
}
