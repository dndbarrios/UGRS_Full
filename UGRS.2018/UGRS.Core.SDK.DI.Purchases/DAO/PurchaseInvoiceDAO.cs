using System;
using System.Collections.Generic;
using UGRS.Core.SDK.UI;
using UGRS.Core.Utility;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.Services;
using UGRS.Core.SDK.DI.Purchases.Tables;
using System.Linq;

namespace UGRS.Core.SDK.DI.Purchases.DAO
{
    public class PurchaseInvoiceDAO
    {
        QueryManager mObjQueryManager = new QueryManager();
        #region Invoice
        /// <summary>
        /// Obtener centro de costo.
        /// </summary>
        public string GetCostCenter()
        {
            string lStrCostCenter = "";
            try
            {
                lStrCostCenter = mObjQueryManager.GetValue("U_GLO_CostCenter", "UserID", DIApplication.Company.UserSignature.ToString(), "OUSR");
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("CostCenter: {0}", ex.Message));
                LogService.WriteError("PurchasesDAO (GetCostCenter): " + ex.Message);
                LogService.WriteError(ex);
            }
            return lStrCostCenter;
        }

        public string GetCostCenterAdmOpe(string pStrPrcCode)
        {
            string CCTypeCode = "";
            try
            {
                CCTypeCode = mObjQueryManager.GetValue("CCTypeCode", "PrcCode", pStrPrcCode, "OPRC");
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("CostCenter: {0}", ex.Message));
                LogService.WriteError("PurchasesDAO (GetCostCenterAdmOpe): " + ex.Message);
                LogService.WriteError(ex);
            }
            return CCTypeCode;
        }

        /// <summary>
        /// Obtener Cuenta de centro de costo del articulo.
        /// </summary>
        public string GetCostAccount(string pStrItemCode)
        {
            string lStrCostAccount = "";
            try
            {
                lStrCostAccount = mObjQueryManager.GetValue("U_GLO_CostAccount", "ItemCode", pStrItemCode, "OITM");
            }
            catch (Exception lObjException)
            {
                UIApplication.ShowError(string.Format("CostAccount: {0}", lObjException.Message));
                LogService.WriteError("PurchasesDAO (GetCostAccount): " + lObjException.Message);
                LogService.WriteError(lObjException);
            }
            return lStrCostAccount;
        }

        /// <summary>
        /// Obtener almacen.
        /// </summary>
        public string GetWhouse(string pStrCostingCode)
        {
            string lStrCostAccount = "";
            try
            {
                lStrCostAccount = mObjQueryManager.GetValue("U_GLO_Whouse", "PrcCode", pStrCostingCode, "OPRC");
                if (string.IsNullOrEmpty(lStrCostAccount))
                {
                    UIApplication.ShowMessageBox(string.Format("No se encontro almacen en centro de costo {0}", pStrCostingCode));
                }
            }
            catch (Exception lObjException)
            {
                UIApplication.ShowError(string.Format("CostAccount: {0}", lObjException.Message));
                LogService.WriteError("PurchasesDAO (GetWhouse): " + lObjException.Message);
                LogService.WriteError(lObjException);
            }
            return lStrCostAccount;
        }

        public string GetDocNum(string pStrDocEntry)
        {
            string lStrDocNum = "";
            try
            {
                lStrDocNum = mObjQueryManager.GetValue("DocNum", "DocEntry", pStrDocEntry, "OPCH");
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("GetDocNum: {0}", ex.Message));
                LogService.WriteError("PurchasesDAO (GetDocNum): " + ex.Message);
                LogService.WriteError(ex);
            }

            return lStrDocNum;
        }

        public string GetDocStatus(string pStrDocEntry)
        {
            string lStrDocStatus = "";
            try
            {
                lStrDocStatus = mObjQueryManager.GetValue("DocStatus", "DocEntry", pStrDocEntry, "OPCH");
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("GetDocNum: {0}", ex.Message));
                LogService.WriteError("PurchasesDAO (GetDocNum): " + ex.Message);
                LogService.WriteError(ex);
            }

            switch (lStrDocStatus)
            {
                case "C":
                    lStrDocStatus = "Cerrado";
                    break;

                case "O":
                    lStrDocStatus = "Abierto";
                    break;
                case "P":
                    lStrDocStatus = "Pagado";
                    break;
                case "D":
                    lStrDocStatus = "Entregado";
                    break;
            }

            return lStrDocStatus;
        }

        public string GetDocCanceled(string pStrDocEntry, string pStrType)
        {
            string lStrDocCanceled = "";
            try
            {
                if (pStrType == "XML")
                {
                    lStrDocCanceled = mObjQueryManager.GetValue("CANCELED", "DocEntry", pStrDocEntry, "OPCH");
                }
                if (pStrType == "Nota")
                {
                    lStrDocCanceled = mObjQueryManager.GetValue("StornoToTr", "StornoToTr", pStrDocEntry, "OJDT");
                    if (!string.IsNullOrEmpty(lStrDocCanceled))
                    {
                        lStrDocCanceled = "Y";
                    }
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("GetDocNum: {0}", ex.Message));
                LogService.WriteError("PurchasesDAO (GetDocNum): " + ex.Message);
                LogService.WriteError(ex);
            }

            switch (lStrDocCanceled)
            {
                case "Y":
                    lStrDocCanceled = "Cancelado";
                    break;

                case "C":
                    lStrDocCanceled = "Cancelado";
                    break;
            }

            return lStrDocCanceled;
        }

        public string GetTaxCode(string pStrRate)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            string lStrVoucherCode = "C0";
            try
            {
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Rate", pStrRate);
                string lStrQuery = this.GetSQL("GetTaxCodeAP").Inject(lLstStrParameters);
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lStrVoucherCode = lObjRecordset.Fields.Item("Code").Value.ToString();
                }
                else
                {
                    LogService.WriteError("No se encontro impuesto con  tasa o cuota de: "+ pStrRate);
                }

            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
                LogService.WriteError("PurchasesDAO (GetVoucherCode): " + ex.Message);
                LogService.WriteError(ex);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return lStrVoucherCode;
        }

        public string GetWithholdingTaxCodeBP(double pDblRate, string pStrCardCode)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            string lStrVoucherCode = "";
            try
            {
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                //pDblRate = Math.Truncate(pDblRate);
                pDblRate = Math.Truncate(10 * pDblRate) / 10;
                lLstStrParameters.Add("Rate", pDblRate.ToString());
                lLstStrParameters.Add("CardCode", pStrCardCode);
                string lStrQuery = this.GetSQL("GetWithholdingTaxBP").Inject(lLstStrParameters);
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lStrVoucherCode = lObjRecordset.Fields.Item("WTCode").Value.ToString();
                }
                else
                {
                    UIApplication.ShowMessageBox("No se encontro retencion con  tasa de: " + pDblRate + " para el socio de negocio " + pStrCardCode);
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
                LogService.WriteError("PurchasesDAO (GetVoucherCode): " + ex.Message);
                LogService.WriteError(ex);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return lStrVoucherCode;
        }

        public string GetWithholdingTaxCode(double pDblRate)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            string lStrVoucherCode = "";
            try
            {
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                //pDblRate = Math.Truncate(pDblRate);
                pDblRate = Math.Truncate(10 * pDblRate) / 10;
                lLstStrParameters.Add("Rate", pDblRate.ToString());
                string lStrQuery = this.GetSQL("GetWithholdingTax").Inject(lLstStrParameters);
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lStrVoucherCode = lObjRecordset.Fields.Item("WTCode").Value.ToString();
                }
                else
                {
                    UIApplication.ShowMessageBox("No se encontro retencion con  tasa de: " + pDblRate);
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
                LogService.WriteError("PurchasesDAO (GetVoucherCode): " + ex.Message);
                LogService.WriteError(ex);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return lStrVoucherCode;
        }


        public string GetItemIEPS()
        {
            string lStrItemCode = "";
            try
            {
                lStrItemCode = mObjQueryManager.GetValue("ItemCode", "ItemName", "IEPS", "OITM");
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("GetDocNum: {0}", ex.Message));
                LogService.WriteError("PurchasesDAO (GetDocNum): " + ex.Message);
                LogService.WriteError(ex);
            }

            return lStrItemCode;
        }


        public string GetLocalTax()
        {
            string lStrItemCode = "";
            try
            {
                lStrItemCode = mObjQueryManager.GetValue("U_Value", "Name", "GLO_LOCAL_TAX", "[@UG_CONFIG]");
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("GetDocNum: {0}", ex.Message));
                LogService.WriteError("PurchasesDAO (GetDocNum): " + ex.Message);
                LogService.WriteError(ex);
            }

            return lStrItemCode;
        }

        public string GetWhsMQ(string pStrItemCode)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            string lStrWhsMQ = "";
            try
            {
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("ItemCode", pStrItemCode);
                string lStrQuery = this.GetSQL("GetItemMQWhs").Inject(lLstStrParameters);
                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lStrWhsMQ = "MQHEOBRA";
                }
                else
                {
                    lStrWhsMQ = "";
                    UIApplication.ShowMessageBox(string.Format("No se encontro almacen en articulo {0}", pStrItemCode));
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("PurchasesDAO (GetWhsMQ): " + ex.Message);
                LogService.WriteError(ex);

            }
            return lStrWhsMQ;
        }

        public bool UpdateStatus(VouchersDetail pObjVoucherDetail)
        {
            bool lBolTranSuccess = false;
            try
            {
                string lStrDocStatus = GetDocCanceled(pObjVoucherDetail.DocEntry.ToString(), pObjVoucherDetail.Type);
                if (!lStrDocStatus.Equals("Cancelado"))
                {
                    lStrDocStatus = GetDocStatus(pObjVoucherDetail.DocEntry.ToString());
                }

                if (lStrDocStatus != pObjVoucherDetail.Status && pObjVoucherDetail.Type != "Nota")
                {
                    pObjVoucherDetail.Status = lStrDocStatus;
                    DIApplication.Company.StartTransaction();
                    PurchasesServiceFactory lObjPurchasesServiceFactory = new PurchasesServiceFactory();
                    //if (pObjVoucherDetail.Type == "Nota")
                    //{
                    //    var lObjVoucher = lObjPurchasesServiceFactory.GetPurchaseVouchersService().GetVoucherDetailByTrans(pObjVoucherDetail.DocEntry);
                    //    pObjVoucherDetail.Total = lObjVoucher.Sum(x => x.Total);
                    //    pObjVoucherDetail.Subtotal = lObjVoucher.Sum(x => x.Subtotal);
                    //}

                    if (lObjPurchasesServiceFactory.GetVouchersDetailService().Update(pObjVoucherDetail) == 0)
                    {
                        if (lObjPurchasesServiceFactory.GetVouchersService().UpdateTotal(pObjVoucherDetail.CodeVoucher) != 0)
                        {
                            LogService.WriteError("InvoiceDI (UpdateTotal) " + DIApplication.Company.GetLastErrorDescription());
                        }
                        else
                        {
                            lBolTranSuccess = true;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lBolTranSuccess = false;
                LogService.WriteError("PurchasesDAO (UpdateStatus): " + ex.Message);
                LogService.WriteError(ex);
            }
            finally
            {
                try
                {
                    if (lBolTranSuccess)
                    {
                        DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        lBolTranSuccess = true;
                    }
                    else
                    {
                        if (DIApplication.Company.InTransaction)
                        {
                            DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                            lBolTranSuccess = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    lBolTranSuccess = false;
                    LogService.WriteError("PurchasesDAO (UpdateStatus): " + ex.Message);
                    LogService.WriteError(ex);
                    
                }
            }

            return lBolTranSuccess;
        }

        #endregion
    }
}
