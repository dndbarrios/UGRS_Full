using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Configuration;
using UGRS.AddOn.FoodProduction.DTO;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.Utility;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.FoodProduction.DAO;
using System.Globalization;

namespace UGRS.AddOn.FoodProduction.Services
{
    public class ReceptionTransferService
    {        
        QueryManager mObjQueryManager;
        ReceptionTransferDAO mObjReceptionTransferDAO = new ReceptionTransferDAO();   

        public ReceptionTransferService()
        {
            mObjQueryManager = new QueryManager();
        }


        public IList<TransferHeader_DTO> GetTransferHeader(string pStrId, string pStrDocEntry)
        {
            Recordset lObjRecordset = null;
            TransferHeader_DTO lObjTransferHeader_DTO = null;
            IList<TransferHeader_DTO> lListObjResult = null;
            try
            {                
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                string lStrQuery = mObjReceptionTransferDAO.GetTransferHeaderQuery(pStrId, pStrDocEntry);                
                lObjRecordset.DoQuery(lStrQuery);
                if (lObjRecordset.RecordCount > 0)
                {
                    lListObjResult = new List<TransferHeader_DTO>();                   
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lObjTransferHeader_DTO = new TransferHeader_DTO();
                        lObjTransferHeader_DTO.DocEntry = Convert.ToInt32(lObjRecordset.Fields.Item("DocEntry").Value.ToString());
                        lObjTransferHeader_DTO.DocNum = Convert.ToInt32(lObjRecordset.Fields.Item("DocNum").Value.ToString());
                        lObjTransferHeader_DTO.Series = Convert.ToInt32(lObjRecordset.Fields.Item("Series").Value.ToString());
                        lObjTransferHeader_DTO.DocDate = Convert.ToDateTime(lObjRecordset.Fields.Item("DocDate").Value.ToString());
                        lObjTransferHeader_DTO.TaxDate = Convert.ToDateTime(lObjRecordset.Fields.Item("TaxDate").Value.ToString());
                        lObjTransferHeader_DTO.Filler = lObjRecordset.Fields.Item("Filler").Value.ToString();
                        lObjTransferHeader_DTO.ToWhsCode = lObjRecordset.Fields.Item("ToWhsCode").Value.ToString();
                        lObjTransferHeader_DTO.JrnlMemo = lObjRecordset.Fields.Item("JrnlMemo").Value.ToString();
                        lObjTransferHeader_DTO.Comments = lObjRecordset.Fields.Item("Comments").Value.ToString();     
                        //lObjTransferHeader_DTO.U_PL_WhsReq = lObjRecordset.Fields.Item("U_PL_WhsReq").Value.ToString();
                        //lObjTransferHeader_DTO.U_GLO_Alert = lObjRecordset.Fields.Item("U_GLO_Alert").Value.ToString();
                        //lObjTransferHeader_DTO.U_CO_TypeInvoice = lObjRecordset.Fields.Item("U_CO_TypeInvoice").Value.ToString();
                        lObjTransferHeader_DTO.U_MQ_OrigenFol = Convert.ToInt32(lObjRecordset.Fields.Item("U_MQ_OrigenFol").Value.ToString());                        
                        lListObjResult.Add(lObjTransferHeader_DTO);                       
                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception ex)
            {                
                //throw;                
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return lListObjResult;
        }

        public IList<TransferDetail_DTO> GetTransferDetail(string pStrDocEntry)
        {
            Recordset lObjRecordset = null;
            TransferDetail_DTO lObjTransferDetail_DTO = null;
            IList<TransferDetail_DTO> lListObjResult = null;
            try
            {
                string lStrQuery = mObjReceptionTransferDAO.GetTransferDetailQuery(pStrDocEntry);
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);
                if (lObjRecordset.RecordCount > 0)
                {
                    lListObjResult = new List<TransferDetail_DTO>();
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lObjTransferDetail_DTO = new TransferDetail_DTO();      
                        lObjTransferDetail_DTO.DocEntry = Convert.ToInt32(lObjRecordset.Fields.Item("DocEntry").Value.ToString());
                        lObjTransferDetail_DTO.LineNum = Convert.ToInt32(lObjRecordset.Fields.Item("LineNum").Value.ToString());
                        lObjTransferDetail_DTO.ItemDescription = lObjRecordset.Fields.Item("Dscription").Value.ToString();
                        lObjTransferDetail_DTO.ItemCode = lObjRecordset.Fields.Item("ItemCode").Value.ToString();
                        lObjTransferDetail_DTO.WhsCode = lObjRecordset.Fields.Item("WhsCode").Value.ToString();
                        lObjTransferDetail_DTO.FromWhsCode = lObjRecordset.Fields.Item("FromWhsCod").Value.ToString();
                        lObjTransferDetail_DTO.Quantity = Convert.ToInt32(lObjRecordset.Fields.Item("Quantity").Value.ToString());
                        lObjTransferDetail_DTO.U_GLO_BagsBales = Convert.ToInt32(lObjRecordset.Fields.Item("U_GLO_BagsBales").Value.ToString());
                        lObjTransferDetail_DTO.BaseType = Convert.ToInt32(lObjRecordset.Fields.Item("BaseType").Value.ToString());
                        lObjTransferDetail_DTO.BaseEntry = Convert.ToInt32(lObjRecordset.Fields.Item("BaseEntry").Value.ToString());
                        lObjTransferDetail_DTO.BaseLine = Convert.ToInt32(lObjRecordset.Fields.Item("BaseLine").Value.ToString());
                        lListObjResult.Add(lObjTransferDetail_DTO);
                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception ex)
            {
                //throw;
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return lListObjResult;
        }
    }
}