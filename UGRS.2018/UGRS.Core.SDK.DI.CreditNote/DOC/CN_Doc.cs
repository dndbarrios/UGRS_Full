using SAPbobsCOM;
using SAPbouiCOM;
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
        bool mBolQuestionFlag = true;

        /// <summary>
        /// Crea un pago
        /// </summary>
        /// 
        public bool CreateCreditNote(CreditNoteDoc pObjCreditNoteDoc)
        {
           
            bool lBolIsSuccess = false;
            try
            {
                UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
                //Documents lObjDraft = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes);
                SAPbobsCOM.Documents lObjDraft = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                // SAPbobsCOM.Payments lObjPayment = (SAPbobsCOM.Payments)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oVendorPayments);
                lObjDraft.CardCode = pObjCreditNoteDoc.CardCode;
                lObjDraft.UserFields.Fields.Item("U_MQ_OrigenFol").Value = string.Format("{0}_{1}", pObjCreditNoteDoc.NcId, pObjCreditNoteDoc.Line);
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
                    UIApplication.ShowError(lStrErrMsg);
                }
                else
                {
                    string pStrNewDocEntry = DIApplication.Company.GetNewObjectKey();
                    pObjCreditNoteDoc.DocEntryDraft = pStrNewDocEntry;
                    pObjCreditNoteDoc.IsDraft = "Y";
                    mObjFactory.GetCreditNoteDocService().Update(pObjCreditNoteDoc);
                    // LogService.WriteSuccess("pago creado correctamente: InvoiceDocEntry: " + pObjPurchase.DocEntry);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("Error al crear el borrador {}", ex.Message, pObjCreditNoteDoc.);
                LogService.WriteError(ex);

            }
            return lBolIsSuccess;
        }

        private void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
           
            if (mBolQuestionFlag == true && pVal.FormTypeEx == "0")// && pVal.FormTypeCount == 2 && pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD && pVal.Action_Success == false)
            {
                try
                {
                    DateTime lDtmStart = DateTime.Now;
                    var mFormQuestion = UIApplication.GetApplication().Forms.ActiveForm;
                    if (mFormQuestion.Modal == true && mFormQuestion.Title == "Nota de crédito de clientes - Borrador" && pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_ACTIVATE)
                    {
                        StaticText llblQuestion = ((SAPbouiCOM.StaticText)(mFormQuestion.Items.Item("7").Specific));
                        if (llblQuestion.Caption == "Este documento no puede modificarse tras la creación. ¿Continuar?")
                        {
                            Button BtnQuestionYes = ((SAPbouiCOM.Button)(mFormQuestion.Items.Item("1").Specific));
                            BtnQuestionYes.Item.Click();
                        }
                        else if (llblQuestion.Caption == "Se calculará retención de impuestos. ¿Desea abrir la tabla de retención de impuestos")
                        {
                            Button BtnQuestionNo = ((SAPbouiCOM.Button)(mFormQuestion.Items.Item("2").Specific));
                            BtnQuestionNo.Item.Click();
                        }


                      
                    }
                }
                catch (Exception ex)
                {
                    LogService.WriteError("Question" + ex.Message);
                    UIApplication.ShowError("Question " + ex.Message);
                }
                finally
                {


                }


                //txtCardCode = ((SAPbouiCOM.EditText)(mFormInvoice.Items.Item("4").Specific));
            }
        }

        private void DocRelByUI(string pStrDocEntry, List<CreditNoteDet> pObjCreditNoteDet)
        {
            SAPbouiCOM.Form lFrmNC = SAPbouiCOM.Framework.Application.SBO_Application.OpenForm((SAPbouiCOM.BoFormObjectEnum)112, "", pStrDocEntry);
            //lFrmNC.Freeze(true);
            try
            {
                //open invoice draft form

                //Pestaña
                SAPbouiCOM.Folder lFolderFinances = (SAPbouiCOM.Folder)lFrmNC.Items.Item("138").Specific;
                lFolderFinances.Item.Click();

                //Boton doc relacionados
                SAPbouiCOM.Button lBtnDoc = (SAPbouiCOM.Button)lFrmNC.Items.Item("498").Specific;
                lBtnDoc.Item.Click();

                //Formulario documentos referencia
                SAPbouiCOM.Form lFrmRelation = UIApplication.GetApplication().Forms.ActiveForm;

                //Grid doc referencia
                SAPbouiCOM.Matrix lMtxRelation = (SAPbouiCOM.Matrix)lFrmRelation.Items.Item("5").Specific;

                int i = 1;
                foreach (var item in pObjCreditNoteDet)
                {
                    if (item.DocNumINV != "0")
                    {
                        //Combobox Col 1
                        ((SAPbouiCOM.ComboBox)lMtxRelation.Columns.Item("1").Cells.Item(i).Specific).Select("13", BoSearchKey.psk_ByValue);
                        ((SAPbouiCOM.ComboBox)lMtxRelation.Columns.Item("254000018").Cells.Item(i).Specific).Select("01", BoSearchKey.psk_ByValue);
                        ((SAPbouiCOM.EditText)lMtxRelation.Columns.Item("3").Cells.Item(i).Specific).Value = item.DocNumINV;
                        i++;
                    }
                }  
                //Boton OK
                SAPbouiCOM.Button lBtnOK = (SAPbouiCOM.Button)lFrmRelation.Items.Item("540020001").Specific;
                lBtnOK.Item.Click();

               
                if (UIApplication.GetApplication().Forms.ActiveForm.TypeEx == "179")
                {
                    if (ValidateDocRel(lFrmNC, pObjCreditNoteDet))
                    {
                        SAPbouiCOM.Button lBtnUpdate = (SAPbouiCOM.Button)lFrmNC.Items.Item("1").Specific;
                        lBtnUpdate.Item.Click();
                        mBolQuestionFlag = true;
                    }
                }

                lFrmNC.Close();
               


              
            }
            catch (Exception ex)
            {

                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
               
            }
            finally
            {
                //lFrmNC.Freeze(false);
            }
        }

        private bool ValidateDocRel(SAPbouiCOM.Form pObjFrmNC, List<CreditNoteDet> pObjCreditNoteDet)
        {
            try
            {
                SAPbouiCOM.Button lBtnDoc = (SAPbouiCOM.Button)pObjFrmNC.Items.Item("498").Specific;
                lBtnDoc.Item.Click();

                //Formulario documentos referencia
                SAPbouiCOM.Form lFrmRelation = UIApplication.GetApplication().Forms.ActiveForm;
                //Grid doc referencia
                SAPbouiCOM.Matrix lMtxRelation = (SAPbouiCOM.Matrix)lFrmRelation.Items.Item("5").Specific;

                if (lMtxRelation.RowCount - 1 == pObjCreditNoteDet.Count)
                {
                    lFrmRelation.Close();
                    return true;
                }
                else
                {
                    lFrmRelation.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(ex.Message);
                LogService.WriteError(ex);
                return false;
                
            }
           


        }


    }
}
