using UGRS.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Purchases.DTO;
using UGRS.Core.SDK.DI.Purchases.Tables;
using UGRS.Core.SDK.UI;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;
using UGRS.Core.Utility;
using UGRS.Core.SDK.DI.Purchases.Enums;
using SAPbobsCOM;

namespace UGRS.Core.SDK.DI.Purchases.DAO
{
    public class PurchaseVouchersDAO
    {
        QueryManager mObjQueryManager = new QueryManager();
        PurchasesServiceFactory mObjPurchaseServiceFactory = new PurchasesServiceFactory();
        #region Vouchers
        /// <summary>
        /// Obtener lineas de comprobantes.
        /// </summary>
        /// 
        public List<VouchersDetail> GetVouchesDetail(string pStrCode)
        {
            List<VouchersDetail> lLstVoucherDetail = new List<VouchersDetail>();
            try
            {
                var lVarResult = mObjQueryManager.GetObjectsList<VouchersDetail>("U_CodeVoucher", pStrCode, "[@UG_GLO_VODE]").Where(x=> x.DocEntry != null).ToList();
                if (lVarResult != null)
                {
                    lLstVoucherDetail = lVarResult;
                }
            }
            catch (Exception lObjException)
            {
                UIApplication.ShowError(string.Format("Account Refound: {0}", lObjException.Message));
                LogService.WriteError("PurchasesDAO (GetVouchesDetail): " + lObjException.Message);
                LogService.WriteError(lObjException);
            }

            return lLstVoucherDetail;
        }

        public List<VouchersDetailDTO> GetInvoiceVouchesDetail(string pStrCostCenter, string pStrFolio)
        {

            List<VouchersDetailDTO> lLstVoucherDetail = new List<VouchersDetailDTO>();
            Recordset lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            string lStrUserCode = DIApplication.Company.UserName;
           
            try {

                string lStrQuery = this.GetSQL("GetInvoiceVouchers").Inject(new Dictionary<string, string>() {{ "Area", pStrCostCenter }, { "Folio", pStrFolio } });

                lObjRecordset.DoQuery(lStrQuery);
                if(lObjRecordset.RecordCount > 0) {

                    for(int i = 0; i < lObjRecordset.RecordCount; i++) {

                        VouchersDetailDTO lObjVoucherDetail = new VouchersDetailDTO();

                        lObjVoucherDetail.ISR = (Double)lObjRecordset.Fields.Item("U_ISR").Value;
                        lObjVoucherDetail.IVA = (Double)lObjRecordset.Fields.Item("U_IVA").Value;
                        lObjVoucherDetail.RetIVA = (Double)lObjRecordset.Fields.Item("U_RetIVA").Value;
                        lObjVoucherDetail.IEPS = (Double)lObjRecordset.Fields.Item("U_IEPS").Value;
                        lObjVoucherDetail.Subtotal = (Double)lObjRecordset.Fields.Item("U_SubTotal").Value;
                        lObjVoucherDetail.Total = (Double)lObjRecordset.Fields.Item("U_Total").Value;
                        lObjVoucherDetail.DocNum = lObjRecordset.Fields.Item("DocNum").Value.ToString();
                        lObjVoucherDetail.Provider = lObjRecordset.Fields.Item("U_Provider").Value.ToString();
                        lObjVoucherDetail.Status = lObjRecordset.Fields.Item("U_Status").Value.ToString();
                        lObjVoucherDetail.Coment = lObjRecordset.Fields.Item("U_Coment").Value.ToString();
                        lObjVoucherDetail.Coments = lObjRecordset.Fields.Item("U_Coments").Value.ToString();
                        lObjVoucherDetail.UserCode = lObjRecordset.Fields.Item("U_UserCode").Value.ToString();
                        lObjVoucherDetail.Date = (DateTime)lObjRecordset.Fields.Item("U_Date").Value;
                        lObjVoucherDetail.Type = lObjRecordset.Fields.Item("U_Type").Value.ToString();
                        lObjVoucherDetail.NA = lObjRecordset.Fields.Item("U_NA").Value.ToString();
                        lObjVoucherDetail.CodeVoucher = lObjRecordset.Fields.Item("U_CodeVoucher").Value.ToString();
                        lObjVoucherDetail.RowCode = lObjRecordset.Fields.Item("Code").Value.ToString();
                        lObjVoucherDetail.DocEntry = lObjRecordset.Fields.Item("DocEntry").Value.ToString();
                        lObjVoucherDetail.Line = lObjRecordset.Fields.Item("U_Line").Value.ToString();
                        lObjVoucherDetail.DocFolio = lObjRecordset.Fields.Item("DocFolio").Value.ToString();
                        lLstVoucherDetail.Add(lObjVoucherDetail);
                        lObjRecordset.MoveNext();
                    }
                       
                }


            }
            catch(Exception lObjException) {
                UIApplication.ShowError(string.Format("Account Refound: {0}", lObjException.Message));
                LogService.WriteError("PurchasesDAO (GetInvoiceVouchesDetail): " + lObjException.Message);
                LogService.WriteError(lObjException);
            }

            return lLstVoucherDetail;
        }


        //public List<VouchersDetail> GetVouchersDetail()
        //{
        //    List<VouchersDetail> lLstVoucherDetail = new List<VouchersDetail>();
        //    SAPbobsCOM.Recordset lObjRecordset = null;
        //    try
        //    {
        //        string lStrQuery = this.GetSQL("GetVoucher");
        //        Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
              

        //        //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

        //        lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
        //        lStrQuery = lStrQuery.Inject(lLstStrParameters);

        //        lObjRecordset.DoQuery(lStrQuery);

        //        if (lObjRecordset.RecordCount > 0)
        //        {
        //            for (int i = 0; i < lObjRecordset.RecordCount; i++)
        //            {
        //                Vouchers lObjVoucher = new Vouchers();
        //                lObjVoucher.RowCode = lObjRecordset.Fields.Item("Code").Value.ToString();
        //                lObjVoucher.Folio = lObjRecordset.Fields.Item("U_Folio").Value.ToString();
        //                lObjVoucher.Status = Convert.ToInt32(lObjRecordset.Fields.Item("U_Status").Value.ToString());
        //                lObjVoucher.Area = lObjRecordset.Fields.Item("U_Area").Value.ToString();
        //                lObjVoucher.Employee = lObjRecordset.Fields.Item("U_Employee").Value.ToString();
        //                lObjVoucher.Date = Convert.ToDateTime(lObjRecordset.Fields.Item("U_Date").Value.ToString());
        //                lObjVoucher.Total = Convert.ToDouble(lObjRecordset.Fields.Item("U_Total").Value.ToString());


        //               // lLstObjVouchers.Add(lObjVoucher);
        //                lObjRecordset.MoveNext();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
        //        LogService.WriteError("PurchasesDAO (GetVouchersList): " + ex.Message);
        //        LogService.WriteError(ex);
        //    }
        //    finally
        //    {
        //        MemoryUtility.ReleaseComObject(lObjRecordset);
        //    }



        //    return lLstVoucherDetail;
        //}

        public List<VouchersDetail> GetVoucherDetailByTrans(string pStrTransId)
        {
            List<VouchersDetail> lLstVoucherDetail = new List<VouchersDetail>();
            try
            {
                lLstVoucherDetail = mObjQueryManager.GetObjectsList<VouchersDetail>("U_DocEntry", pStrTransId, "[@UG_GLO_VODE]").ToList();
            }
            catch (Exception lObjException)
            {
                UIApplication.ShowError(string.Format("GetVoucherDetailByTrans: {0}", lObjException.Message));
                LogService.WriteError("PurchasesDAO (GetVoucherDetailByTrans): " + lObjException.Message);
                LogService.WriteError(lObjException);

            }

            return lLstVoucherDetail;
        }

        /// <summary>
        /// Obtener encabezado de comprobantes.
        /// </summary>
        public List<Vouchers> GetVouches(string pStrCode)
        {
            List<Vouchers> lStrAccountRefound = new List<Vouchers>();
            try
            {
                var lVarResult = mObjQueryManager.GetObjectsList<Vouchers>("Code", pStrCode, "[@UG_GLO_VOUC]").ToList();
                if (lVarResult != null)
                {
                    lStrAccountRefound = lVarResult;
                }
            }
            catch (Exception lObjException)
            {
                UIApplication.ShowError(string.Format("Account Refound: {0}", lObjException.Message));
                LogService.WriteError("PurchasesDAO (GetVouches): " + lObjException.Message);
                LogService.WriteError(lObjException);
            }
            return lStrAccountRefound;
        }

        /// <summary>
        /// Obtener lista de vouchers por filtro.
        /// </summary>
        public IList<Vouchers> GetVouchersList(SearchVouchersDTO pObjSearchVouchersDTO)
        {
            List<Vouchers> lLstObjVouchers = new List<Vouchers>();
            SAPbobsCOM.Recordset lObjRecordset = null;
            try
            {
                string lStrQuery = this.GetSQL("GetVoucher");
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                foreach (string lStrFilter in GetFilterssVouchers(pObjSearchVouchersDTO, lLstStrParameters))
                {
                    lStrQuery += " " + lStrFilter;
                }

                lStrQuery += "group by b0.Code, B0.U_Folio, B0.U_Status, B0.U_Area, B0.U_Employee, B0.U_Date order by Code desc";

                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lStrQuery = lStrQuery.Inject(lLstStrParameters);

                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        Vouchers lObjVoucher = new Vouchers();
                        lObjVoucher.RowCode = lObjRecordset.Fields.Item("Code").Value.ToString();
                        lObjVoucher.Folio = lObjRecordset.Fields.Item("U_Folio").Value.ToString();
                        lObjVoucher.Status = Convert.ToInt32(lObjRecordset.Fields.Item("U_Status").Value.ToString());
                        lObjVoucher.Area = lObjRecordset.Fields.Item("U_Area").Value.ToString();
                        lObjVoucher.Employee = lObjRecordset.Fields.Item("U_Employee").Value.ToString();
                       
                        lObjVoucher.Date = Convert.ToDateTime(lObjRecordset.Fields.Item("U_Date").Value.ToString());
                        lObjVoucher.Total = Convert.ToDouble(lObjRecordset.Fields.Item("U_Total").Value.ToString());
                        double lDblTotal = Convert.ToDouble((lObjRecordset.Fields.Item("DocTotal").Value.ToString()));
                        if (lObjVoucher.Total == lDblTotal && lObjVoucher.Status == (int)StatusEnum.Authorized_Ope_Admon)
                        {
                            //lObjVoucher.Status = 5;
                            if (mObjPurchaseServiceFactory.GetVouchersService().Update(lObjVoucher.RowCode, StatusEnum.Closed) != 0)
                            {
                                string lStrerror = DIApplication.Company.GetLastErrorDescription();
                                UIApplication.ShowMessageBox(lStrerror);
                            }
                            else
                            {
                                lObjVoucher.Status = (int)StatusEnum.Closed; // lObStatusEnum.GetDescription();
                            }
                        }

                        lLstObjVouchers.Add(lObjVoucher);
                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
                LogService.WriteError("PurchasesDAO (GetVouchersList): " + ex.Message);
                LogService.WriteError(ex);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return lLstObjVouchers;

        }



        /// <summary>
        /// Agregar filtros a la consulta.
        /// </summary>
        private List<string> GetFilterssVouchers(SearchVouchersDTO pObjSearchVouchersDTO, Dictionary<string, string> lLstStrParameters)
        {

            List<string> lLstFilters = new List<string>();
            try
            {
                string lStrWereAnd = "and"; //simepre sera And por la condicion de 

                if (!string.IsNullOrEmpty(pObjSearchVouchersDTO.Area))
                {
                    lLstFilters.Add(lStrWereAnd + " U_Area = '{Area}'");
                    lLstStrParameters.Add("Area", pObjSearchVouchersDTO.Area);
                    lStrWereAnd = "and";
                }

                if (!string.IsNullOrEmpty(pObjSearchVouchersDTO.Employee))
                {
                    lLstFilters.Add(lStrWereAnd + " U_Employee = '{Employee}' ");
                    lLstStrParameters.Add("Employee", pObjSearchVouchersDTO.Employee);
                    lStrWereAnd = "and";
                }
                else
                {
                    lLstFilters.Add("");
                }


                if (!string.IsNullOrEmpty(pObjSearchVouchersDTO.StartDate.ToString()) && !string.IsNullOrEmpty(pObjSearchVouchersDTO.EndDate.ToString()))
                {
                    lLstFilters.Add(lStrWereAnd + " U_Date between '{StartDate}' and '{EndDate}' ");
                    lLstStrParameters.Add("StartDate", pObjSearchVouchersDTO.StartDate);
                    lLstStrParameters.Add("EndDate", pObjSearchVouchersDTO.EndDate);
                    lStrWereAnd = "and";
                }
                else
                {
                    if (!string.IsNullOrEmpty(pObjSearchVouchersDTO.StartDate.ToString()))
                    {
                        lLstFilters.Add(lStrWereAnd + " U_Date = '{StartDate}' ");
                        lLstStrParameters.Add("StartDate", pObjSearchVouchersDTO.StartDate);
                        lStrWereAnd = "and";
                    }
                    else
                    {
                        lLstFilters.Add("");
                    }
                }

                if (pObjSearchVouchersDTO.Status > 0)
                {
                    lLstFilters.Add(lStrWereAnd + " U_Status = '{Status}' ");
                    lLstStrParameters.Add("Status", pObjSearchVouchersDTO.Status.ToString());
                    lStrWereAnd = "and";
                }
                else
                {
                    lLstFilters.Add("");
                }

            }
            catch (Exception ex)
            {
                LogService.WriteError("PurchasesDAO (GetFilterssVouchers): " + ex.Message);
                LogService.WriteError(ex);

            }

            return lLstFilters;

        }

        public IList<BankDTO> GetBanks()
        {
            SAPbobsCOM.Recordset lObjResults = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            IList<BankDTO> lLstObjBanks = new List<BankDTO>();
            try
            {
                string lStrQuery = this.GetSQL("GetBanks");
                lObjResults.DoQuery(lStrQuery);
                if (lObjResults.RecordCount > 0)
                {
                    for (int i = 0; i < lObjResults.RecordCount; i++)
                    {
                        BankDTO lObjBank = new BankDTO();
                        lObjBank.BankCode = lObjResults.GetColumnValue<string>("BankCode");
                        lObjBank.BankName = lObjResults.GetColumnValue<string>("BankName");
                        lLstObjBanks.Add(lObjBank);
                        lObjResults.MoveNext();
                    }
                }
                return lLstObjBanks;
            }
            catch (Exception e)
            {
                LogService.WriteError("GetBanks (GetFilterssVouchers): " + e.Message);
                LogService.WriteError(e);
                return lLstObjBanks;
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjResults);
            }
        }

        public IList<AccountDTO> GetBankAccounts(string pStrBankCode)
        {
            SAPbobsCOM.Recordset lObjResults = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            IList<AccountDTO> lLstObjBanks = new List<AccountDTO>();
            try
            {
                string lStrQuery = this.GetSQL("GetBankAccounts").InjectSingleValue("BankCode", pStrBankCode);
                lObjResults.DoQuery(lStrQuery);
                if (lObjResults.RecordCount > 0)
                {
                    for (int i = 0; i < lObjResults.RecordCount; i++)
                    {
                        AccountDTO lObjAccount = new AccountDTO();
                        lObjAccount.BankCode = lObjResults.GetColumnValue<string>("BankCode");
                        lObjAccount.Account = lObjResults.GetColumnValue<string>("Account");
                        lObjAccount.Branch = lObjResults.GetColumnValue<string>("Branch");
                        lObjAccount.GLAccount = lObjResults.GetColumnValue<string>("GLAccount");
                        lLstObjBanks.Add(lObjAccount);
                        lObjResults.MoveNext();
                    }
                }
                return lLstObjBanks;
            }
            catch (Exception e)
            {
                LogService.WriteError("GetBankAccounts (GetFilterssVouchers): " + e.Message);
                LogService.WriteError(e);
                return lLstObjBanks;
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjResults);
            }
        }
    
     

        #endregion
    }
}
