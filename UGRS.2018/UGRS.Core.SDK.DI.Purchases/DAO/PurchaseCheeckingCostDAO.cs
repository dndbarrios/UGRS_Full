using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.SDK.DI.Purchases.DTO;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;
using UGRS.Core.Utility;
using UGRS.Core.SDK.UI;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.Services;
using UGRS.Core.SDK.DI.Purchases.Tables;
using UGRS.Core.SDK.DI.Purchases.Enums;
using UGRS.Core.SDK.DI.Purchases.Services;

namespace UGRS.Core.SDK.DI.Purchases.DAO
{
    public class PurchaseCheeckingCostDAO
    {
        #region CheeckingCost
        QueryManager mObjQueryManager = new QueryManager();
        /// <summary>
        /// Obtener Pagos.
        /// </summary>
        public IList<PaymentDTO> GetPayment(string pStrCostCenter, string pStrStatus)//, bool pBolMQ_Maqui)
        {
         
            SAPbobsCOM.Recordset lObjRecordset = null;
            IList<PaymentDTO> lLstpaymentDTO = new List<PaymentDTO>();
            try
            {

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();

                string lStrQuery = this.GetSQL("GetPayments");//.Inject(lLstStrParameters);
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);
                if (!string.IsNullOrEmpty(pStrCostCenter))
                {
                    lLstStrParameters.Add("CostCenter", pStrCostCenter);
                    lStrQuery += " and U_GLO_CostCenter = '{CostCenter}'";
                }

                //if (!pBolMQ_Maqui)
                //{
                //    lStrQuery += " and U_GLO_CostCenter != 'MQ_Maqui'";
                //}


                if (!string.IsNullOrEmpty(pStrStatus) && Convert.ToInt16(pStrStatus) > 0)
                {
                    lLstStrParameters.Add("Status", pStrStatus);
                    lStrQuery += " and U_Status = '{Status}'";
                }
                else
                {
                    //Solicitud para ver todos menos los ya cerrados
                   // lLstStrParameters.Add("Status", ((int)StatusEnum.Closed).ToString());
                   // lStrQuery += " and (U_Status != '{Status}' or U_Status is null)";
                }

                lStrQuery += " group by T0.DocEntry, T0.U_GLO_CodeMov, /*DocEntry, DocNum, */U_FZ_Auxiliar, CardName, U_GLO_CostCenter,  DocTotal, isnull(A1.GLFSV,0) + isnull(B1.GLFSV,0), A2.GLSSV, A3.U_Total, A3.U_Status, MQ1.Debit, MQ1.Credit";
                lStrQuery += " order by T0.DocEntry desc";

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lStrQuery = lStrQuery.Inject(lLstStrParameters);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        PaymentDTO lObjPaymentDTO = new PaymentDTO();
                        //lObjPaymentDTO.DocEntry = lObjRecordset.Fields.Item("DocEntry").Value.ToString();
                       // lObjPaymentDTO.DocNum = lObjRecordset.Fields.Item("DocNum").Value.ToString();
                        lObjPaymentDTO.Folio = lObjRecordset.Fields.Item("U_GLO_CodeMov").Value.ToString();
                        lObjPaymentDTO.Status = lObjRecordset.Fields.Item("U_Status").Value.ToString();
                        //lObjPaymentDTO.StatusDescription = lObjRecordset.Fields.Item("C_StatusDescription").Value.ToString();
                        lObjPaymentDTO.EmpId = lObjRecordset.Fields.Item("U_FZ_Auxiliar").Value.ToString();
                        lObjPaymentDTO.Employee = lObjRecordset.Fields.Item("CardName").Value.ToString();
                        lObjPaymentDTO.Area = lObjRecordset.Fields.Item("U_GLO_CostCenter").Value.ToString();
                        //lObjPaymentDTO.Date = lObjRecordset.Fields.Item("DocDate").Value.ToString();
                        lObjPaymentDTO.ImpSol = Convert.ToDouble(lObjRecordset.Fields.Item("DocTotal").Value.ToString());
                        lObjPaymentDTO.ImpComp = Convert.ToDouble(lObjRecordset.Fields.Item("U_Total").Value.ToString());
                        //string ss = lObjRecordset.Fields.Item("GLSSV").Value.ToString();
                        lObjPaymentDTO.ImpSob = Convert.ToDouble(lObjRecordset.Fields.Item("GLSSV").Value.ToString());
                        lObjPaymentDTO.ImpFalt = Convert.ToDouble(lObjRecordset.Fields.Item("GLFSV").Value.ToString());
                        lObjPaymentDTO.SaldoPen = lObjPaymentDTO.ImpSol - lObjPaymentDTO.ImpComp + lObjPaymentDTO.ImpFalt - lObjPaymentDTO.ImpSob;
                        lObjPaymentDTO.MQ_Credit = Convert.ToDouble(lObjRecordset.Fields.Item("Credit").Value.ToString());
                        lObjPaymentDTO.MQ_Debit = Convert.ToDouble(lObjRecordset.Fields.Item("Debit").Value.ToString());
                        lLstpaymentDTO.Add(lObjPaymentDTO);
                        lObjRecordset.MoveNext();
                       
                    }
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
                LogService.WriteError("PurchasesDAO (GetPayment): " + ex.Message);
                LogService.WriteError(ex);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            lLstpaymentDTO = lLstpaymentDTO.GroupBy(x => x.Folio).Select(y => new PaymentDTO
            {
                Folio = y.First().Folio,
                Status = y.First().Status,
                EmpId = y.First().EmpId,
                Employee = y.First().Employee,
                Area = y.First().Area,
                ImpSol = y.Sum(s => s.ImpSol),
                ImpSob = y.First().ImpSob + y.First().MQ_Credit,
                ImpComp = y.First().ImpComp,
                ImpFalt = y.First().ImpFalt + y.First().MQ_Debit,
                SaldoPen = (y.Sum(s => s.ImpSol) - y.First().ImpComp + (y.First().ImpFalt + y.First().MQ_Debit) - y.First().ImpSob - y.First().MQ_Credit)
            }).ToList();

            foreach (PaymentDTO lObjPayDTO in lLstpaymentDTO)
            {
                if (!string.IsNullOrEmpty(lObjPayDTO.Status) && Convert.ToInt16(lObjPayDTO.Status) == (int)StatusEnum.Authorized_Ope_Admon && lObjPayDTO.SaldoPen == 0)
                {
                    UpdateStatus(lObjPayDTO.Folio);
                }
            }

            return lLstpaymentDTO;
        }


        private bool UpdateStatus(string lStrFolio)
        {
            bool lBolResult = false;
            string lStrCodeVoucher = mObjQueryManager.GetValue("Code", "U_CodeMov", lStrFolio, "[@UG_GLO_VOUC]");
            VouchersService lObjVoucherService = new VouchersService();
            if (!string.IsNullOrEmpty(lStrCodeVoucher))
            {
                lBolResult = lObjVoucherService.Update(lStrCodeVoucher, StatusEnum.Closed) == 0 ? true : false;
            }
            return lBolResult;
        }


        /// <summary>
        /// Obtener codigo de voucher.
        /// </summary>
        public string CheckingCost(string pStrCodeMov)
        {

            string lStrCostAccount = "";
            try
            {
                lStrCostAccount = mObjQueryManager.GetValue("Code", "U_CodeMov", pStrCodeMov, "[@UG_GLO_VOUC]");
            }
            catch (Exception lObjException)
            {
                UIApplication.ShowError(string.Format("CheckingCost: {0}", lObjException.Message));
                LogService.WriteError("PurchasesDAO (CheckingCost): " + lObjException.Message);
                LogService.WriteError(lObjException);
            }
            return lStrCostAccount;

        }

        #endregion
    }
}
