﻿using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.CreditNote.DTO;
using UGRS.Core.SDK.DI.Models;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;


namespace UGRS.Core.SDK.DI.CreditNote.DOC
{
    public class CreditNoteDOC
    {
        CreditNoteFactory mObjFactory = new CreditNoteFactory();

        /// <summary>
        /// Crea un pago
        /// </summary>
        /// 
        public bool CreateCreditNote(CreditNoteDTO pObjCreditNoteDTO)
        {
            bool lBolIsSuccess = false;
            try
            {
                //pObjCreditNoteDTO = new CreditNoteDTO();
                //pObjCreditNoteDTO.LstDocEntry = new List<string>();
                //pObjCreditNoteDTO.LstDocEntry.Add("55");
                //pObjCreditNoteDTO.LstDocEntry.Add("56");
                //pObjCreditNoteDTO.LstDocEntry.Add("57");
                //pObjCreditNoteDTO.LstDocEntry.Add("58");
                //pObjCreditNoteDTO.LstDocEntry.Add("59");
                //pObjCreditNoteDTO.LstDocEntry.Add("60");
                //pObjCreditNoteDTO.LstDocEntry.Add("61");
                //pObjCreditNoteDTO.LstDocEntry.Add("62");

               // DocRelByUI("337", pObjCreditNoteDTO);

                Documents lObjCreditNote = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes);

                // SAPbobsCOM.Payments lObjPayment = (SAPbobsCOM.Payments)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oVendorPayments);
                lObjCreditNote.CardCode =  pObjCreditNoteDTO.C_CardCode;

                lObjCreditNote.UserFields.Fields.Item("U_B1SYS_MainUsage").Value = "G02";

                lObjCreditNote.Lines.ItemCode = mObjFactory.GetCreditNoteService().GetBonusItemCode();
                lObjCreditNote.Lines.Quantity = 1;
                lObjCreditNote.Lines.UnitPrice = 100;// pObjCreditNoteDTO.C_Amount;
                lObjCreditNote.Lines.Add();

                int intError = lObjCreditNote.Add();
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
                    DocRelByUI(pStrNewDocEntry, pObjCreditNoteDTO);
                    // LogService.WriteSuccess("pago creado correctamente: InvoiceDocEntry: " + pObjPurchase.DocEntry);
                    return true;
                }
            }
            catch (Exception ex)
            {
                //LogService.WriteError("PaymentDI (CancelDocument) InvoiceDocEntry:" + pObjPurchase.DocEntry + " Mensaje:" + ex.Message);
                LogService.WriteError(ex);

            }
            return lBolIsSuccess;
        }

        private void DocRelByUI(string pStrDocEntry, CreditNoteDTO pObjCreditNoteDTO)
        {
            SAPbouiCOM.Form lFrmNC = SAPbouiCOM.Framework.Application.SBO_Application.OpenForm(SAPbouiCOM.BoFormObjectEnum.fo_InvoiceCreditMemo, "", pStrDocEntry);
            lFrmNC.Freeze(true);
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
                foreach (var item in pObjCreditNoteDTO.LstDocEntry)
                {
                    //Combobox Col 1
                    ((SAPbouiCOM.ComboBox)lMtxRelation.Columns.Item("1").Cells.Item(i).Specific).Select("13", BoSearchKey.psk_ByValue); 
                    ((SAPbouiCOM.ComboBox)lMtxRelation.Columns.Item("254000018").Cells.Item(i).Specific).Select("01", BoSearchKey.psk_ByValue);
                    ((SAPbouiCOM.EditText)lMtxRelation.Columns.Item("3").Cells.Item(i).Specific).Value = item;
                  
                    i++;
                }
                //Boton OK
                SAPbouiCOM.Button lBtnOK = (SAPbouiCOM.Button)lFrmRelation.Items.Item("540020001").Specific;
                lBtnOK.Item.Click();

                SAPbouiCOM.Button lBtnUpdate = (SAPbouiCOM.Button)lFrmNC.Items.Item("1").Specific;
                lBtnUpdate.Item.Click();
                lBtnUpdate.Item.Click();
              
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
               
            }
            finally
            {
                lFrmNC.Freeze(false);
            }
        }


    }
}
