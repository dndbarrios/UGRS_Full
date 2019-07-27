using SAPbobsCOM;
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
        /// <summary>
        /// Crea un pago
        /// </summary>
        public bool CreatePayment(CreditNoteDTO pObjCreditNoteDTO)
        {
            bool lBolIsSuccess = false;
            try
            {

                Documents lObjCreditNote = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes);

               
                    // SAPbobsCOM.Payments lObjPayment = (SAPbobsCOM.Payments)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oVendorPayments);
                lObjCreditNote.CardCode = pObjCreditNoteDTO.C_CardCode;
              
                //lObjPayment.DocDate = pObjPurchase.DocDate;
                //lObjCreditNote.TaxDate = pObjPurchase.TaxDate;
                //lObjCreditNote.DocDate = pObjPurchase.DocDate;



                lObjCreditNote.DocTotal = pObjCreditNoteDTO.C_Amount;

               // lObjCreditNote.CashAccount = pObjPurchase.Account;// lObjPurchasesDAO.GetAccountRefund(pObjPurchase.Area);

              
             



                if (lObjCreditNote.Add() != 0)
                {
                    UIApplication.ShowMessageBox(string.Format("Exception: {0}", DIApplication.Company.GetLastErrorDescription()));
                    LogService.WriteError("PaymentDI (CreatePayment) DocEntry:" + pObjCreditNoteDTO + " Mensaje:" + DIApplication.Company.GetLastErrorDescription());
                }
                else
                {
                    LogService.WriteSuccess("pago creado correctamente: InvoiceDocEntry: " + pObjPurchase.DocEntry);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("PaymentDI (CancelDocument) InvoiceDocEntry:" + pObjPurchase.DocEntry + " Mensaje:" + ex.Message);
                LogService.WriteError(ex);

            }
            return lBolIsSuccess;
        }


    }
}
