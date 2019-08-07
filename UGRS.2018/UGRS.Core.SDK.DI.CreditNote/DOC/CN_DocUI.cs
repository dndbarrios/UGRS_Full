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
            if (!string.IsNullOrEmpty(pObjCreditNoteDoc.DocEntry) && int.TryParse(pObjCreditNoteDoc.DocEntry, out n))
            {
                string lStrUUID = string.Empty;
                //draft 112
                SAPbouiCOM.Form lFrmNC = SAPbouiCOM.Framework.Application.SBO_Application.OpenForm((SAPbouiCOM.BoFormObjectEnum)14, "", pObjCreditNoteDoc.DocEntry);
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
                    foreach (var lObjDet in pObjCreditNoteDoc.LstCreditNoteDet)
                    {
                        lStrUUID = lObjDet.FolioFiscal;
                        if (lObjDet.DocNumINV != "0")
                        {
                            //Combobox Col 1
                            ((SAPbouiCOM.ComboBox)lMtxRelation.Columns.Item("1").Cells.Item(i).Specific).Select("13", BoSearchKey.psk_ByValue);
                            ((SAPbouiCOM.ComboBox)lMtxRelation.Columns.Item("254000018").Cells.Item(i).Specific).Select("01", BoSearchKey.psk_ByValue);

                            try
                            {
                                ((SAPbouiCOM.EditText)lMtxRelation.Columns.Item("3").Cells.Item(i).Specific).Value = lObjDet.DocNumINV;

                            }
                            catch (Exception ex)
                            {
                                if (!CallCFL(lStrUUID, lMtxRelation, lObjDet.DocNumINV, i, ex))
                                {
                                    UIApplication.ShowError(ex.Message);
                                    LogService.WriteError(ex.Message);
                                    LogService.WriteError(ex);
                                }
                            }
                            i++;
                        }
                    }
                    //Boton OK
                    SAPbouiCOM.Button lBtnOK = (SAPbouiCOM.Button)lFrmRelation.Items.Item("540020001").Specific;
                    lBtnOK.Item.Click();



                    ////SaveAsDraft
                    //UIApplication.GetApplication().Menus.Item("5907").Activate();
                    //lFrmNC.Close();


                    if (UIApplication.GetApplication().Forms.ActiveForm.TypeEx == "179")
                    {
                        SAPbouiCOM.Button lBtnUpdate = (SAPbouiCOM.Button)lFrmNC.Items.Item("1").Specific;
                        lBtnUpdate.Item.Click();
                        lBtnUpdate.Item.Click();
                    }
                }
                catch (Exception ex)
                {
                    UIApplication.ShowError(ex.Message);
                    LogService.WriteError(ex.Message);
                    LogService.WriteError(ex);
                }
                finally
                {
                    //lFrmNC.Freeze(false);
                }
            }
        }


        private bool CallCFL(string pStrUUID, Matrix pMtxRelation, string pStrDocNumINV, int pIntRow, Exception ex)
        {
            try
            {
                if (BugSap(pStrUUID, pMtxRelation, pStrDocNumINV, pIntRow))
                {
                    if (SelectCFL(pStrUUID, pMtxRelation, pStrDocNumINV, pIntRow))
                    {
                        ((SAPbouiCOM.ComboBox)pMtxRelation.Columns.Item("254000018").Cells.Item(pIntRow).Specific).Select("01", BoSearchKey.psk_ByValue);
                        if (SelectCFL(pStrUUID, pMtxRelation, pStrDocNumINV, pIntRow))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception Exception)
            {
                UIApplication.ShowError(Exception.Message);
                LogService.WriteError(Exception.Message);
                LogService.WriteError(Exception);
                return false;
            }
        }

        //¬¬
        private bool BugSap(string pStrUUId, Matrix pMtxRelation, string pStrDocNumINV, int pIntRow)
        {
            //Bug SAP
            try
            {
                SAPbouiCOM.Form lFrmCFL = UIApplication.GetApplication().Forms.ActiveForm;
                lFrmCFL.Close();
                SAPbouiCOM.EditText txtInv = ((SAPbouiCOM.EditText)pMtxRelation.Columns.Item("3").Cells.Item(pIntRow).Specific);
                txtInv.ClickPicker();

                lFrmCFL = UIApplication.GetApplication().Forms.ActiveForm;
                SAPbouiCOM.EditText txtSearch = (SAPbouiCOM.EditText)lFrmCFL.Items.Item("6").Specific;
                txtSearch.Value = pStrDocNumINV;
                return true;
            }
            catch (Exception ex )
            {
                UIApplication.ShowError(ex.Message);
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
                return false;
            }
           
        }

        private bool SelectCFL(string pStrUUId, Matrix pMtxRelation, string pStrDocNumINV, int pIntRow)
        {
            try
            {

                SAPbouiCOM.Form lFrmCFL = UIApplication.GetApplication().Forms.ActiveForm;
                SAPbouiCOM.Matrix lMtxCFL = (SAPbouiCOM.Matrix)lFrmCFL.Items.Item("7").Specific;
                int lIntSelectRow = lMtxCFL.GetNextSelectedRow();
                for (int i = lIntSelectRow; i < lMtxCFL.RowCount; i++)
                {
                    string UUID = ((SAPbouiCOM.EditText)lMtxCFL.Columns.Item("U_FolioFiscal").Cells.Item(i).Specific).Value;
                    if (pStrUUId == UUID)
                    {
                        lMtxCFL.Columns.Item(0).Cells.Item(i).Click();
                       // lMtxRelation.SelectRow(i, true, false);
                        //Boton OK
                        SAPbouiCOM.Button lBtnOK = (SAPbouiCOM.Button)lFrmCFL.Items.Item("1").Specific;
                        lBtnOK.Item.Click();
                        return true;;
                    }
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(ex.Message);
                LogService.WriteError(ex.Message);
                LogService.WriteError(ex);
            }
            return false;
                 
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
