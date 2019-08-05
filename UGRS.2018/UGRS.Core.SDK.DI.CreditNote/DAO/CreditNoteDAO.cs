using System;
using System.Collections.Generic;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.Services;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.CreditNote.Utils;
using UGRS.Core.SDK.UI;
using UGRS.Core.Utility;
using UGRS.Core.SDK.DI.CreditNote.Tables;
using System.Linq;
using UGRS.Core.SDK.DI.CreditNote.DTO;

namespace UGRS.Core.SDK.DI.CreditNote.DAO
{
    public class CreditNoteDAO
    {
        QueryManager mObjQueryManager = new QueryManager();

        public string GetInvoicesQuery(DateTime pDtmDateTimeFrom, DateTime pDtmDateTimeTo)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("StartDate", "20190601");// pDtmDateTime.ToString("yyyyMMdd"));
            lLstStrParameters.Add("EndDate", "20190730");// pDtmDateTime.ToString("yyyyMMdd"));
            string lStrQuery = this.GetSQL("Exec_Invoices").Inject(lLstStrParameters);
            LogService.WriteInfo(lStrQuery);
            return lStrQuery;
        }

        public string GetBonusItemCode()
        {
            return mObjQueryManager.GetValue("U_VALUE", "Name", Constants.STR_ENTRY_BONUS, Constants.STR_CONFIG_TABLE);
        }

        public int GetLastCode()
        {
            string lStrLastDispId = "0";
            lStrLastDispId = mObjQueryManager.Max<string>("U_NcId", "[@UG_PE_NC]");
            lStrLastDispId = string.IsNullOrEmpty(lStrLastDispId) ? "0" : lStrLastDispId.Substring(3);
            return Convert.ToInt32(lStrLastDispId);

        }

        public int GetFirstCode()
        {
            string lStrLastDispId = "0";
            lStrLastDispId = mObjQueryManager.Min<string>("U_NcId", "[@UG_PE_NC]");
            lStrLastDispId = string.IsNullOrEmpty(lStrLastDispId) ? "0" : lStrLastDispId.Substring(3);
            
            return Convert.ToInt32(lStrLastDispId);
        }

        public string GetTaxCode(string pStrRate)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            string lStrVoucherCode = "VE";
            try
            {
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Rate", pStrRate);
                string lStrQuery = this.GetSQL("GetTaxCodeAR").Inject(lLstStrParameters);
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lStrVoucherCode = lObjRecordset.Fields.Item("Code").Value.ToString();
                }
                else
                {
                    LogService.WriteError("No se encontro impuesto con  tasa o cuota de: " + pStrRate);
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

        public CreditNoteT GetCreditNoteTSaved(string pStrId)
        {
            return mObjQueryManager.GetObjectsList<CreditNoteT>("U_NcId", pStrId, "[@UG_PE_NC]").First();
        }

        public List<CreditNoteDoc> GetCreditNoteDocSaved(string pStrId)
        {
            return mObjQueryManager.GetObjectsList<CreditNoteDoc>("U_NcId", pStrId, "[@UG_PE_NCDOC]").ToList();
        }

        public List<CreditNoteDet> GetCreditNoteDetSaved(string pStrId)
        {
            return mObjQueryManager.GetObjectsList<CreditNoteDet>("U_NcId", pStrId, "[@UG_PE_NCDET]").ToList();
        }

        public List<DraftReferenceDTO> GetDraftRelation(string pStrNcId)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            List<DraftReferenceDTO> lLstDraftReference = new List<DraftReferenceDTO>();
            try
            {
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("NcId", pStrNcId);
                string lStrQuery = this.GetSQL("ValidateDraft").Inject(lLstStrParameters);
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        DraftReferenceDTO lObjDraftReference = new DraftReferenceDTO
                        {
                            DocEntryDraft = lObjRecordset.Fields.Item("DocEntry").Value.ToString(),
                            OrigenFolioDet = lObjRecordset.Fields.Item("U_MQ_OrigenFol_Det").Value.ToString(),
                            RefDocEntr = Convert.ToInt32(lObjRecordset.Fields.Item("RefDocEntr").Value.ToString()),
                            RefDocNum = lObjRecordset.Fields.Item("RefDocNum").Value.ToString()
                        };
                        lObjRecordset.MoveNext();
                        lLstDraftReference.Add(lObjDraftReference);
                    }
                }
                else
                {
                    LogService.WriteError("No se encontraron documentos relacionados ");
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
            return lLstDraftReference;
        }
    }
}
