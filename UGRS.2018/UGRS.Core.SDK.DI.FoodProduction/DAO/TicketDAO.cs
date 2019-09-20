using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using UGRS.Core.Exceptions;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.SDK.DI.FoodProduction.Tables;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;
using UGRS.Core.Utility;


namespace UGRS.Core.SDK.DI.FoodProduction.DAO
{
    public class TicketDAO
    {
        QueryManager mObjQueryManager;
        public TicketDAO()
        {
            mObjQueryManager = new QueryManager();
        }

        public string GetFilteredTickets(string pBPCode, string pDate1, string pDate2, int pIntCapType, string pStrStatus, string pIntDocType)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("BPCode", pBPCode);
            lLstStrParameters.Add("Date1", pDate1);
            lLstStrParameters.Add("Date2", pDate2);
            lLstStrParameters.Add("CapType", pIntCapType.ToString());
            lLstStrParameters.Add("Status", pStrStatus);
            lLstStrParameters.Add("DocType", pIntDocType);
           
            //lLstStrParameters.Add("CardCode", pStrCardCode);
           // GetItemCodesList(DateTime.Now, "");
            return this.GetSQL("SearchTicket").Inject(lLstStrParameters);

            //var lObjStream = lObjBaseType.Assembly.GetManifestResourceStream(lStrNamespace + "." + mStrDatabaseType + "." + pStrResource + ".sql"
        }


        public string GetPrice(string pStrItemCode)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            string lStrPrice = "";

            try
            {
                TicketDAO mObjTicketDAO = new TicketDAO();
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("ItemCode", pStrItemCode);
                lLstStrParameters.Add("WhsCode", "PLHE");
                string lStrQuery = this.GetSQL("GetPrice").Inject(lLstStrParameters);
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lStrPrice = lObjRecordset.Fields.Item(1).Value.ToString();
                    LogService.WriteInfo(string.Format("Precio DB {0}", lStrPrice));
                    //if (Convert.ToDouble(lStrPrice) > 100)
                    //{
                    //    int ii = 0;
                    //}

                    lStrPrice = Convert.ToDouble(lObjRecordset.Fields.Item(1).Value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                    LogService.WriteInfo(string.Format("Precio convertido {0}", lStrPrice));
                   
                   
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("[GetPrice]: {0} ", ex.Message));
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format("[GetPrice]: {0}", ex.Message));
               // UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return lStrPrice;
        }

        public string GetWareHouse(string pStrUserID)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            string lStrWareHouse = "";

            try
            {
                TicketDAO mObjTicketDAO = new TicketDAO();
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("UserID", pStrUserID);
                string lStrQuery = this.GetSQL("GetWarehouse").Inject(lLstStrParameters);
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lStrWareHouse = lObjRecordset.Fields.Item(0).Value.ToString();
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("[GetWareHouse]: {0} ", ex.Message));
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format("[GetWareHouse]: {0}", ex.Message));
                // UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return lStrWareHouse;
        }


        public IList<string> GetWareHousePather(string pStrWareHouse)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            IList<string> lStrWareHousePhater =  new List<string>();

            try
            {
                TicketDAO mObjTicketDAO = new TicketDAO();
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("WareHouse", pStrWareHouse);
                string lStrQuery = this.GetSQL("GetWareHousePather").Inject(lLstStrParameters);
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lStrWareHousePhater.Add(lObjRecordset.Fields.Item("WhsCode").Value.ToString());
                       
                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("[GetWareHousePather]: {0} ", ex.Message));
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format("[GetWareHousePather]: {0}", ex.Message));
                // UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return lStrWareHousePhater;
        }

        public IList<string> GetProjects()
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            IList<string> lLstProjects = new List<string>();

            try
            {
                TicketDAO mObjTicketDAO = new TicketDAO();

                string lStrQuery = this.GetSQL("GetProjects");
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstProjects.Add(lObjRecordset.Fields.Item("PrjCode").Value.ToString());

                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("[GetProjects]: {0} ", ex.Message));
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format("[GetProjects]: {0}", ex.Message));
                // UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return lLstProjects;
        }

        public Ticket GetTicket(string pStrCode)
        {
            Ticket lObjTicket = new Ticket();
            try
            {
                lObjTicket = mObjQueryManager.GetTableObject<Ticket>("Code", pStrCode, "[@UG_PL_TCKT]");
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("[GetTicket]: {0} ", ex.Message));
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format("[GetTicket]: {0}", ex.Message));
            }
            return lObjTicket;
        }

        public IList<TicketDetail> GetListTicketDetail(string pStrFolio)
        {
            IList<TicketDetail> lLstTicketDetail = null;
            try
            {
               lLstTicketDetail = mObjQueryManager.GetObjectsList<TicketDetail>("U_Folio", pStrFolio, "[@UG_PL_TCKD]");
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("[GetListTicketDetail]: {0} ", ex.Message));
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format("[GetListTicketDetail]: {0}", ex.Message));
            }
            return lLstTicketDetail;
        }


        public IList<string> GetItemCodesList(DateTime pDtmCreateDate, string pStrWhsCode)
        {
            Recordset lObjRecordset = null;
            IList<string> lLstStrResult = new List<string>();

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("CreateDate", pDtmCreateDate.ToString("yyyy-MM-dd"));
                lLstStrParameters.Add("WhsCode", pStrWhsCode);

                string lStrQuery = this.GetSQL("GetItemCodesList").Inject(lLstStrParameters);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstStrResult.Add(lObjRecordset.Fields.Item("ItemCode").Value.ToString());
                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("[GetItemCodesList]: {0} ", ex.Message));
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format("[GetItemCodesList]: {0}", ex.Message));
                throw new DAOException(ex.Message, ex);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return lLstStrResult;
        }

        public bool SearchDrafts(string pStrFolio)
        {
            Recordset lObjRecordset = null;
            IList<string> lLstStrResult = new List<string>();

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("FindDraft").InjectSingleValue("Folio", pStrFolio);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    return true;
                }
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(string.Format("[SearchDrafts]: {0} ", lObjException.Message));
                LogService.WriteError(lObjException);
                UIApplication.ShowError(string.Format("[SearchDrafts]: {0}", lObjException.Message));
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return false;
        }

        public string GetInvMovType()
        {
            Recordset lObjRecordset = null;
            IList<string> lLstStrResult = new List<string>();

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetMovType");
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    return lObjRecordset.Fields.Item(0).Value.ToString();
                }
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(string.Format("[GetInvMovType]: {0} ", lObjException.Message));
                LogService.WriteError(lObjException);
                UIApplication.ShowError(string.Format("[GetInvMovType]: {0}", lObjException.Message));
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return "";
        }


        public string GetWhsTransfer(string pStrItemCode )
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            string lStrWareHouse = "";

            try
            {
                TicketDAO mObjTicketDAO = new TicketDAO();
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("ItemCode", pStrItemCode);
                string lStrQuery = this.GetSQL("GetWhsTrans").Inject(lLstStrParameters);
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lStrWareHouse = lObjRecordset.Fields.Item(0).Value.ToString();
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("[GetWhsTransfer]: {0} ", ex.Message));
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format("[GetWhsTransfer]: {0}", ex.Message));
                // UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return lStrWareHouse;

        }

        public string GetUserTransfer(string pStrDocEntry)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            string lStrWareHouse = "";

            try
            {
                TicketDAO mObjTicketDAO = new TicketDAO();
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("DocNum", pStrDocEntry);
                string lStrQuery = this.GetSQL("GetUserTransfer").Inject(lLstStrParameters);
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lStrWareHouse = lObjRecordset.Fields.Item(0).Value.ToString();
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(string.Format("[GetWhsTransfer]: {0} ", ex.Message));
                LogService.WriteError(ex);
                UIApplication.ShowError(string.Format("[GetWhsTransfer]: {0}", ex.Message));
                // UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return lStrWareHouse;
        }
    }
}
