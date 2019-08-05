using SAPbouiCOM;
using System;
using System.Collections.Generic;
using UGRS.Core.SDK.DI.CreditNote.Tables;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;

namespace UGRS.Core.SDK.DI.CreditNote.DOC
{
    public class CN_DocUI
    {

        public void DocRelByUI(CreditNoteDoc pObjCreditNoteDoc)
        {
            int n;
            if (!string.IsNullOrEmpty(pObjCreditNoteDoc.DocEntryDraft) && int.TryParse(pObjCreditNoteDoc.DocEntryDraft, out n))
            {
                SAPbouiCOM.Form lFrmNC = SAPbouiCOM.Framework.Application.SBO_Application.OpenForm((SAPbouiCOM.BoFormObjectEnum)112, "", pObjCreditNoteDoc.DocEntryDraft);
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
                    foreach (var item in pObjCreditNoteDoc.LstCreditNoteDet)
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

                    string ss = lFrmNC.Menu.GetAsXML();
                    string sss = lFrmNC.GetAsXML();

                    UIApplication.GetApplication().Menus.Item("5907").Activate();
                    lFrmNC.Close();

                    //if (UIApplication.GetApplication().Forms.ActiveForm.TypeEx == "179")
                    //{
                    //    if (ValidateDocRel(lFrmNC, pObjCreditNoteDet))
                    //    {
                    //        SAPbouiCOM.Button lBtnUpdate = (SAPbouiCOM.Button)lFrmNC.Items.Item("1").Specific;
                    //        lBtnUpdate.Item.Click();
                    //        mBolQuestionFlag = true;
                    //    }
                    //}

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
