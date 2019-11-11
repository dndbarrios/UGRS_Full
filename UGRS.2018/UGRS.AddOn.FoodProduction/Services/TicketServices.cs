using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using UGRS.AddOn.FoodProduction.UI;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.FoodProduction;
using UGRS.Core.SDK.DI.FoodProduction.DAO;
using UGRS.Core.SDK.DI.FoodProduction.Services;
using UGRS.Core.SDK.DI.FoodProduction.Tables;
using UGRS.Core.Services;
using UGRS.Core.Utility;

namespace UGRS.AddOn.FoodProduction.Services
{
    public class TicketServices
    {
        FoodProductionSeviceFactory mObjFoodProductionFactory = new FoodProductionSeviceFactory();
        TicketDAO mObjTicketDAO = new TicketDAO();
        QueryManager mObjQueryManager = new QueryManager();
        /// <summary>
        /// Busqueda de nombre de socio de negocio.
        /// </summary>
        public string SearchBPName(string pStrCardCode)
        {
            return mObjQueryManager.GetValue("CardName", "CardCode", pStrCardCode, "[OCRD]");
        }

        /// <summary>
        /// busqueda de nombre de item.
        /// </summary>
        public string SearchItemName(string pStrItemCode)
        {
            return mObjQueryManager.GetValue("ItemName", "ItemCode", pStrItemCode, "[OITM]");
        }

        /* Acomodar
        public List<string> GetDocumentByBP(string pStrTableO, string pStrTableL, string pStrBP)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            List<string> lLstDoc = new List<string>();

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string lStrQuery = string.Format(@"
select T0.DocEntry from {0} T0
inner join {1} T1 on T1.DocEntry=T0.DocEntry
where CardCode ='{2}' and DocStatus !='C' and t1.WhsCode ='PLHE' and DocNum not in(select A0.U_Number from [@UG_PL_TCKT] A0 where A0.U_Status != '0')
             ", pStrTableO, pStrTableL, pStrBP);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {
                        lLstDoc.Add(lObjRecordSet.Fields.Item(0).Value.ToString());
                        lObjRecordSet.MoveNext();
                    }
                }
            }
            catch(Exception lObjException)
            {
                UIApplication.ShowMessageBox(string.Format("Error de consulta", lObjException.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

            return lLstDoc;
        }

        public List<string> GetDocumentByFiller(string pStrFiller)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            List<string> lLstDoc = new List<string>();

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string lStrQuery = string.Format(@"
select DocEntry from OWTQ where DocStatus !='C' and Filler = '{0}' and CardCode is null and DocNum not in(select U_Number from [@UG_PL_TCKT] where U_Status != '0' and  U_BPCode = '')
", pStrFiller);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {
                        lLstDoc.Add(lObjRecordSet.Fields.Item(0).Value.ToString());
                        lObjRecordSet.MoveNext();
                    }
                }
            }
            catch (Exception lObjException)
            {
                UIApplication.ShowMessageBox(string.Format("Error de consulta", lObjException.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

            return lLstDoc;
        }

        public List<string> GetDocumentByInvoice(string pStrCardCode, string pStrTable, string pStrUpdivnt)
        {
                  SAPbobsCOM.Recordset lObjRecordSet = null;
            List<string> lLstDoc = new List<string>();

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
         string lStrQuery = string.Format(@"

select DocEntry from {0} where UPdinvnt = '{1}' and CardCode = '{2}' and DocStatus != 'C' and CANCELED = 'N' and DocNum not in (select U_Number from [@UG_PL_TCKT] where U_Status != '0')
", pStrTable, pStrUpdivnt, pStrCardCode);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {
                        lLstDoc.Add(lObjRecordSet.Fields.Item(0).Value.ToString());
                        lObjRecordSet.MoveNext();
                    }
                }
            }
            catch (Exception lObjException)
            {
                UIApplication.ShowMessageBox(string.Format("Error de consulta", lObjException.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

            return lLstDoc;
        }
        */

        public List<string> GetDocumentByWhs(string pStrWhsCode, string pStrTable, string pStrCardCode)
        {
            List<string> lLstDoc = new List<string>();

            string lStrQuery = "select DocEntry from {Table} where WhsCode='{WhereFieldValue}'";// and CardCode = '{CardCode}'";
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("WhereFieldValue", pStrWhsCode);
                lLstStrParameters.Add("Table", pStrTable);
                //lLstStrParameters.Add("CardCode", pStrCardCode);

                lStrQuery = lStrQuery.Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {
                        lLstDoc.Add(lObjRecordSet.Fields.Item(0).Value.ToString());
                        lObjRecordSet.MoveNext();
                    }
                    return lLstDoc;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogService.WriteError("[GetDocumentByWhs]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(string.Format("Error de consulta", ex.Message));

            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

            return lLstDoc;
        }

        public List<string> GetDocumentsByInvoice(string pStrCardCode) //Factura de proveedores
        {
            List<string> lLstDoc = new List<string>();
            string lStrQuery = "select DocEntry from oinv where UPdinvnt = 'c' and CardCode = '{Cardcode}' and DocStatus != 'C' and CANCELED = 'N'";
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Cardcode", pStrCardCode);

                lStrQuery = lStrQuery.Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {
                        lLstDoc.Add(lObjRecordSet.Fields.Item(0).Value.ToString());
                        lObjRecordSet.MoveNext();
                    }
                    return lLstDoc;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogService.WriteError("[GetDocumentsByInvoice]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(string.Format("Error de consulta", ex.Message));

            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

            return lLstDoc;
        }

        public string GetLineStatus(string pStrSource, string pStrDocEntry, string pIntRow)
        {
            string lStatus = "";
            string lStrQuery = "select LineStatus from {Table} where DocEntry = '{DocEntry}' and LineNum = '{Line}'";
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Table", pStrSource);
                lLstStrParameters.Add("DocEntry", pStrDocEntry);
                lLstStrParameters.Add("Line", pIntRow);

                lStrQuery = lStrQuery.Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    lStatus = lObjRecordSet.Fields.Item(0).Value.ToString();
                    return lStatus;
                }
                return lStatus;
            }
            catch (Exception ex)
            {
                LogService.WriteError("[GetLineStatus]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(string.Format("Error de consulta", ex.Message));

            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

            return lStatus;

        }


        public List<string> GetDocumentsByAPInvoice(string pStrCardCode) //Factura de proveedores
        {
            List<string> lLstDoc = new List<string>();
            string lStrQuery = "select DocEntry from OPCH where UPdinvnt = 'O' and CardCode = '{Cardcode}' and DocStatus != 'C' and CANCELED = 'N'";
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Cardcode", pStrCardCode);

                lStrQuery = lStrQuery.Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {
                        lLstDoc.Add(lObjRecordSet.Fields.Item(0).Value.ToString());
                        lObjRecordSet.MoveNext();
                    }
                    return lLstDoc;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogService.WriteError("[GetDocumentsByAPInvoice]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(string.Format("Error de consulta", ex.Message));

            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            return lLstDoc;
        }


        public List<string> GetServerDatetime()
        {
            string lStrQuery = "select getdate(), SUBSTRING( convert(varchar, getdate(),108),1,5)";
            List<string> lStrResult = new List<string>();
            SAPbobsCOM.Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {

                    lStrResult.Add(lObjRecordSet.Fields.Item(0).Value.ToString());
                    lStrResult.Add(lObjRecordSet.Fields.Item(1).Value.ToString());

                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[GetServerDatetime]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(string.Format("Error de consulta", ex.Message));

            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

            return lStrResult;
        }


        public List<string> GetDateTimeUpdate(string pStrCardCode)
        {
            string lStrQuery = "select U_EntryDate, U_EntryTime, U_OutputDate, U_OutputTime from [@UG_PL_TCKD] where code = '{Cardcode}' ";
            List<string> lStrResult = new List<string>();
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            try
            {
                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Cardcode", pStrCardCode);

                lStrQuery = lStrQuery.Inject(lLstStrParameters);
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    lStrResult.Add(lObjRecordSet.Fields.Item(0).Value.ToString());
                    lStrResult.Add(lObjRecordSet.Fields.Item(1).Value.ToString());
                    lStrResult.Add(lObjRecordSet.Fields.Item(2).Value.ToString());
                    lStrResult.Add(lObjRecordSet.Fields.Item(3).Value.ToString());
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[GetDateTimeUpdate]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(string.Format("Error de consulta", ex.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            return lStrResult;
        }

        public string GetOpenLine(string pStrTable, string psTrTable2, string pStrField, string pStrDocnum, string pStrLineNum)
        {
            Dictionary<string, string> lLstStrParameters = null;
            string lStrDelivery = "";
            SAPbobsCOM.Recordset lObjRecordsetCT = null;
            try
            {

                lObjRecordsetCT = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string lStrQuery = ("select {Field} from {Table} T1 inner join {Table2} T2 on T1.Docentry = T2.DocEntry where T1.DocEntry = '{DocEntry}' and T2.LineNum = '{LineNum}'");
                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Table", pStrTable);
                lLstStrParameters.Add("Table2", psTrTable2);
                lLstStrParameters.Add("DocEntry", pStrDocnum);
                lLstStrParameters.Add("LineNum", pStrLineNum);
                lLstStrParameters.Add("Field", pStrField);

                lStrQuery = lStrQuery.Inject(lLstStrParameters);
                lObjRecordsetCT.DoQuery(lStrQuery);

                if (lObjRecordsetCT.RecordCount > 0)
                {
                    lStrDelivery = lObjRecordsetCT.Fields.Item(0).Value.ToString();
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[GetOpenLine]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowError("Error al obtener el peso neto " + ex.Message);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordsetCT);
            }

            return lStrDelivery;
        }

        public string GetDeliveryLine(string pStrTable, string psTrTable2, string pStrField, string pStrDocnum, string pStrLineNum)
        {
            Dictionary<string, string> lLstStrParameters = null;
            string lStrDelivery = "";
            SAPbobsCOM.Recordset lObjRecordsetCT = null;
            try
            {

                lObjRecordsetCT = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string lStrQuery = ("select Quantity - OpenCreQty as Field from {Table} T1 inner join {Table2} T2 on T1.Docentry = T2.DocEntry where T1.DocNum = '{DocNum}' and T2.LineNum = '{LineNum}'");
                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Table", pStrTable);
                lLstStrParameters.Add("Table2", psTrTable2);
                lLstStrParameters.Add("DocNum", pStrDocnum);
                lLstStrParameters.Add("LineNum", pStrLineNum);
                lLstStrParameters.Add("Field", pStrField);

                lStrQuery = lStrQuery.Inject(lLstStrParameters);
                lObjRecordsetCT.DoQuery(lStrQuery);

                if (lObjRecordsetCT.RecordCount > 0)
                {
                    lStrDelivery = lObjRecordsetCT.Fields.Item(0).Value.ToString();
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[GetDeliveryLine]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowError("Error al obtener el peso neto " + ex.Message);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordsetCT);
            }
            return lStrDelivery;
        }


        //select sum(U_netWeight) as NetWeight from  [@UG_PL_TCKD] where U_Folio = '258'
        public string GetNetWeight(string pStrFolio)
        {
            Dictionary<string, string> lLstStrParameters = null;
            string lStrNetWeight = "0";
            SAPbobsCOM.Recordset lObjRecordsetCT = null;
            try
            {

                lObjRecordsetCT = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string lStrQuery = ("select sum(U_netWeight) as NetWeight from  [@UG_PL_TCKD] where U_Folio = '{Folio}'");
                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Folio", pStrFolio);

                lStrQuery = lStrQuery.Inject(lLstStrParameters);
                lObjRecordsetCT.DoQuery(lStrQuery);

                if (lObjRecordsetCT.RecordCount > 0)
                {
                    lStrNetWeight = lObjRecordsetCT.Fields.Item(0).Value.ToString();

                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[GetNetWeight]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowError("Error al consultar cantidades pendientes " + ex.Message);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordsetCT);
            }
            return lStrNetWeight;
        }


        //Function to get a random number 
        private readonly Random random = new Random();
        private readonly object syncLock = new object();
        public int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                int lIntRandom = 0;
                lIntRandom = random.Next(min, max);
                return lIntRandom;
            }
        }

        public bool SaveTicket(Ticket pObjTicket, bool pBolIsUpdate)
        {
            try
            {
                TicketService lObjTicketService = new TicketService();
                FoodProductionSeviceFactory lObjFoodProductionFactory = new FoodProductionSeviceFactory();
                if (pBolIsUpdate)
                {
                    //lObjConfig.RowCode = mObjConfigurationService.GetConfigurationService().GetConfigCode("Code", lObjConfig.Name);
                    pObjTicket.RowCode = lObjFoodProductionFactory.GetTicketService().GetTicketCode("", pObjTicket.Folio);
                    if (lObjTicketService.Update(pObjTicket) != 0)
                    {

                        return false;
                    }
                    LogService.WriteSuccess("[Update Ticket] Folio:" + pObjTicket.Folio);
                }
                else
                {
                    if (lObjTicketService.Add(pObjTicket) != 0)
                    {
                        string error = DIApplication.Company.GetLastErrorDescription();
                        return false;
                    }
                    else
                    {
                       /* string lStrDocEntry = DIApplication.Company.GetNewObjectKey().ToString();
                        pObjTicket.Folio = lStrDocEntry;
                        SaveTicket(pObjTicket, true);*/

                    }
                    LogService.WriteSuccess("[Add Ticket] Folio:" + pObjTicket.Folio);
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[SaveTicket]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(string.Format("SaveTicket: {0}", ex.Message));
                return false;
            }
            return true;
        }

        public bool SaveTicketDetail(List<TicketDetail> pLstTicketDetail, bool pBolIsUpdate)
        {
            try
            {
                TicketDetailService lObjTicketService = new TicketDetailService();

                foreach (TicketDetail lObjTicketDetail in pLstTicketDetail)
                {
                    if (!string.IsNullOrEmpty(lObjTicketDetail.Item))
                    {
                        if (pBolIsUpdate)
                        {
                            lObjTicketDetail.RowCode = mObjFoodProductionFactory.GetTicketDetailService().GetTicketCode("", lObjTicketDetail.Folio, lObjTicketDetail.Line);
                            if (lObjTicketDetail.RowCode != "")
                            {
                                if (lObjTicketService.Update(lObjTicketDetail) != 0)
                                {
                                    return false;
                                }
                                LogService.WriteSuccess("[Update Ticket] Folio:" + lObjTicketDetail.Folio);
                            }
                            else
                            {
                                if (lObjTicketService.Add(lObjTicketDetail) != 0)
                                {
                                    return false;
                                }
                                LogService.WriteSuccess("[Add Ticket Details] Folio" + lObjTicketDetail.Folio);
                            }
                        }
                        else
                        {
                            if (lObjTicketService.Add(lObjTicketDetail) != 0)
                            {
                                return false;
                            }
                            LogService.WriteSuccess("[Add Ticket Details] Folio" + lObjTicketDetail.Folio);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[SaveTicketDetail]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(string.Format("SaveTicketDetail: {0}", ex.Message));
            }
            return true;
        }

        public bool RemoveTicketDetail(List<TicketDetail> pLstTicketDetail)
        {
            try
            {
                IList<TicketDetail> lLstTicketDetail = mObjTicketDAO.GetListTicketDetail(pLstTicketDetail[0].Folio);
                if (lLstTicketDetail.Count > pLstTicketDetail.Count)
                {
                    foreach (TicketDetail lObjTicketDetail in lLstTicketDetail)
                    {
                        List<TicketDetail> lLstTicketDelete = pLstTicketDetail.Where(x => x.RowCode == lObjTicketDetail.RowCode).ToList();
                        if (lLstTicketDelete.Count == 0)
                        {
                            if (mObjFoodProductionFactory.GetTicketDetailService().Remove(lObjTicketDetail) != 0)
                            {
                                return false;
                            }
                            LogService.WriteSuccess("[Delete Ticket] Folio:" + lObjTicketDetail.Folio);
                        }
                        //lLstTicketDetail.Where
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[RemoveTicketDetail]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(string.Format("RemoveTicketDetail: {0}", ex.Message));

            }
            return true;
        }

        public string getRowCodeDetail(string pStrFolio, int pIntItem)
        {
            FoodProductionSeviceFactory lObjFoodProductionFactory = new FoodProductionSeviceFactory();
            return lObjFoodProductionFactory.GetTicketDetailService().GetTicketCode("", pStrFolio, pIntItem);
        }

        ///<summary>    Initializes the choose from lists. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        ///<param name="pbol">      True to pbol. </param>
        ///<param name="pStrType">  Type of the string. </param>
        ///<param name="pStrID">    Identifier for the string. </param>
        ///<returns>    A ChooseFromList. </returns>
        public ChooseFromList initChooseFromLists(bool pbol, string pStrType, string pStrID, SAPbouiCOM.ChooseFromListCollection pObjCFLs) //
        {
            SAPbouiCOM.ChooseFromList lObjoCFL = null;
            try
            {
                SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
                oCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)UIApplication.GetApplication().CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);

                //  Adding 2 CFL, one for the button and one for the edit text.
                oCFLCreationParams.MultiSelection = pbol;
                oCFLCreationParams.ObjectType = pStrType;
                oCFLCreationParams.UniqueID = pStrID;

                lObjoCFL = pObjCFLs.Add(oCFLCreationParams);

            }
            catch (Exception ex)
            {
                LogService.WriteError("[RemoveTicketDetail]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(string.Format("InitCustomerChooseFromListException: {0}", ex.Message));

            }
            return lObjoCFL;
        }

        //Obtiene la fecha + la hora en formato HHmm
        public DateTime GetDateTime(DateTime pDtmEntryDate, string pStrEntryTime)
        {
            DateTime lDtmFist = pDtmEntryDate;
            if (pStrEntryTime != "0")
            {
                if (pStrEntryTime.Length == 3)
                {
                    pStrEntryTime = "0" + pStrEntryTime;
                }
                DateTime dateTime = DateTime.ParseExact(pStrEntryTime, "HHmm", CultureInfo.GetCultureInfo("en-US"));
                lDtmFist = new DateTime(lDtmFist.Year, lDtmFist.Month, lDtmFist.Day, dateTime.Hour, dateTime.Minute, 00);
            }
            return lDtmFist;
        }

        //Obtiene la fecha de tolerancia del ticket + los dias domingos
        public DateTime GetToleranceDay(DateTime pDtmDateEntry)
        {
            DateTime lDtmToleranceDay = pDtmDateEntry;
            //GetToleranceDays()

            string lSTrItemCode = mObjQueryManager.GetValue("U_Value", "Name", "PL_DAYS_TOL", "[@UG_CONFIG]");
            for (int i = 0; i < Convert.ToInt32(lSTrItemCode); i++)
            {
                lDtmToleranceDay = lDtmToleranceDay.AddDays(1);
                if (lDtmToleranceDay.DayOfWeek == DayOfWeek.Sunday || GetHoliday(lDtmToleranceDay))
                {
                    lDtmToleranceDay = lDtmToleranceDay.AddDays(1);
                }
            }
            return lDtmToleranceDay;
        }

        private string GetHldCode()
        {
            return mObjQueryManager.GetValue("HldCode", "CompnyName", DIApplication.Company.CompanyName, "[OADM]");

        }

        private bool GetHoliday(DateTime pDtmDateTime)
        {
            bool lBolIsHoliday = false;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrDelivery = "";
            SAPbobsCOM.Recordset lObjRecordsetCT = null;
            try
            {
                lObjRecordsetCT = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string lStrQuery = ("select StrDate from HLD1 where HldCode = '{HldCode}' and StrDate = CAST('{Date}' AS DATE)");
                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Date", pDtmDateTime.ToString("yyyyMMdd"));
                lLstStrParameters.Add("HldCode", GetHldCode());

                lStrQuery = lStrQuery.Inject(lLstStrParameters);
                lObjRecordsetCT.DoQuery(lStrQuery);

                if (lObjRecordsetCT.RecordCount > 0)
                {
                    lStrDelivery = lObjRecordsetCT.Fields.Item(0).Value.ToString();
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[RemoveTicketDetail]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowError("Consultar días" + ex.Message);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordsetCT);
            }

            return lBolIsHoliday;
        }


        public void print(Ticket pObjTicket, List<TicketDetail> pLstTicketDetail) //Test Print
        {
            string s = " Nom: " + "\n" +
                "Cli" + pObjTicket.BPCode + "\n" +
                "Chofer" + pObjTicket.Driver + "\n" +
                "Placas" + pObjTicket.CarTag + "\n" +
                "Fecha" + pObjTicket.EntryDate + "\n" +
                "Peso ent: " + pObjTicket.InputWT + "\n" +
                "Usuario " + "\n" +
                "Bascula sal " + "\n" +
                "Peso salida " + pObjTicket.OutputWT + "\n" +
                "Neto " + (pObjTicket.InputWT - pObjTicket.OutputWT)+ "\n" ;

            foreach (TicketDetail lObjTicketDetail in pLstTicketDetail)
            {
                s += "Prod: " + lObjTicketDetail.Item + "\n" +
                "Fecha: " + lObjTicketDetail.EntryDate.ToString() + lObjTicketDetail.EntryTime.ToString() + "\n" +
                "Peso Ent: " + lObjTicketDetail.FirstWT + "\n" +
                "Peso Sal: " + lObjTicketDetail.SecondWT + "\n";
            }

            //mObjWeighingMachine.WriteSerialPort("Test");
            PrintDocument lObjPrint = new PrintDocument();
            lObjPrint.PrintPage += delegate(object sender1, PrintPageEventArgs e1)
            {
                e1.Graphics.DrawString(s, new Font("Times New Roman", 12), new SolidBrush(Color.Black), new RectangleF(100, 100, lObjPrint.DefaultPageSettings.PrintableArea.Width, lObjPrint.DefaultPageSettings.PrintableArea.Height));

            };
            try
            {
                lObjPrint.Print();
            }
            catch (Exception ex)
            {
                LogService.WriteError("[RemoveTicketDetail]: " + ex.Message);
                LogService.WriteError(ex);
                throw new Exception("Exception Occured While Printing", ex);
            }
        }





        public bool GetLastInvoice(int pIntTicket)
        {
            SAPbobsCOM.Recordset lObjRecordsetCT = null;
            try
            {
                lObjRecordsetCT = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string lStrQuery = (string.Format("SELECT * FROM INV1 WHERE U_PL_Ticket ='{0}'",pIntTicket));
                
                lObjRecordsetCT.DoQuery(lStrQuery);

                if (lObjRecordsetCT.RecordCount > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[RemoveTicketDetail]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowError("Error: " + ex.Message);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordsetCT);
            }

            return false;
        }

        public bool GetInvDocument(string lStrDocEntry)
        {
            SAPbobsCOM.Recordset lObjRecordsetCT = null;
            try
            {
                lObjRecordsetCT = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string lStrQuery = (string.Format("SELECT * FROM OINV WHERE DocEntry ='{0}'", lStrDocEntry));

                lObjRecordsetCT.DoQuery(lStrQuery);

                if (lObjRecordsetCT.RecordCount > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[RemoveTicketDetail]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowError("Error: " + ex.Message);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordsetCT);
            }

            return false;
        }

        public bool GetLastDelivery(int pIntTicket)
        {
            SAPbobsCOM.Recordset lObjRecordsetCT = null;
            try
            {
                lObjRecordsetCT = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string lStrQuery = (string.Format("SELECT * FROM DLN1 WHERE U_PL_Ticket ='{0}'", pIntTicket));

                lObjRecordsetCT.DoQuery(lStrQuery);

                if (lObjRecordsetCT.RecordCount > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[RemoveTicketDetail]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowError("Error: " + ex.Message);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordsetCT);
            }

            return false;
        }

    }

}
