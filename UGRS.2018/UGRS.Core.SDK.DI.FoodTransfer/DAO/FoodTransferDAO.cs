/*
Autor: LCC Abraham Saúl Sandoval Meneses
Description: Food Plant Data Access Object
Date: 04/09/2018
Company: Qualisys
*/


using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.SDK.DI.FoodTransfer.DTO;
using UGRS.Core.SDK.UI;
using UGRS.Core.Utility;
using UGRS.Core.Services;

namespace UGRS.Core.SDK.DI.FoodTransfer.DAO {

    public class FoodTransferDAO {

        static Object padlock = new Object();

        #region GetPendingTransfers
        public PendingTransfer[] GetPendingTransfers() {

            Recordset recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset); ;
            PendingTransfer[] pendingTransfers = null;

            try {
                recordset.DoQuery(this.GetSQL("GetPendingTransfers").Inject(new Dictionary<string, string>() { { "Whs", GetUserDefaultWarehouse() } }));

                if(recordset.RecordCount > 0) {
                    pendingTransfers = new PendingTransfer[recordset.RecordCount];
                    for(int i = 0; i < recordset.RecordCount; i++) {
                        var pendingTransfer = new PendingTransfer();
                        Parallel.ForEach(recordset.Fields.OfType<Field>(), field => {
                            pendingTransfer.GetType().GetProperty(field.Name).SetValue(pendingTransfer, field.Value);
                        });
                        pendingTransfers[i] = pendingTransfer;
                        recordset.MoveNext();
                    }
                }
            }
            catch(Exception ex) {
                HandleException(ex, "GetPendingTransfers");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }


            return pendingTransfers;
        }
        #endregion

        #region GetTransferItems
        public TransferItem[] GetTransferItems(string DocEntry) {

            Recordset recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset); ;
            TransferItem[] transferItems = null;

            try {

                string query = this.GetSQL("GetTransferItems").Inject(new Dictionary<string, string>() { { "DocEntry", DocEntry } });
                recordset.DoQuery(query);

                if(recordset.RecordCount > 0) {
                    transferItems = new TransferItem[recordset.RecordCount];
                    for(int i = 0; i < recordset.RecordCount; i++) {
                        var transferItem = new TransferItem();
                        Parallel.ForEach(recordset.Fields.OfType<Field>(), field => {
                            transferItem.GetType().GetProperty(field.Name).SetValue(transferItem, field.Value);
                        });
                        transferItems[i] = transferItem;
                        recordset.MoveNext();
                    }
                }
            }
            catch(Exception ex) {
                HandleException(ex, "GetTransferItems");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }

            return transferItems;
        }
        #endregion

        #region GetTransferRequests
        public RequestTransfer[] GetTransferRequests(string docNum) {

            Recordset recordset = null;
            RequestTransfer[] requestTransfers = null;
            Dictionary<string, string> paramaters = new Dictionary<string, string>();

            try {

                paramaters.Add("DocNum", docNum);
                paramaters.Add("Whs", GetUserDefaultWarehouse());
                string query = this.GetSQL("GetTransferRequest").Inject(paramaters);
                recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                recordset.DoQuery(query);

                if(recordset.RecordCount > 0) {
                    requestTransfers = new RequestTransfer[recordset.RecordCount];
                    for(int i = 0; i < recordset.RecordCount; i++) {
                        var transferRequest = new RequestTransfer();
                        Parallel.ForEach(recordset.Fields.OfType<Field>(), field => {
                            transferRequest.GetType().GetProperty(field.Name).SetValue(transferRequest, field.Value);
                        });
                        requestTransfers[i] = transferRequest;
                        recordset.MoveNext();
                    }
                }
            }
            catch(Exception ex) {
                HandleException(ex, "GetTransferItems");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }

            return requestTransfers;
        }
        #endregion

        #region GetUserDefaultWarehouse
        public string GetUserDefaultWarehouse() {

            Recordset lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            string result = String.Empty;
            var currenUser = DIApplication.Company.UserName;

            try {

                string query = this.GetSQL("GetUserDefaultWarehouse").Inject(new Dictionary<string, string>() { { "UserCode", currenUser } });
                lObjRecordset.DoQuery(query);

                if(lObjRecordset.RecordCount > 0) {
                    result = lObjRecordset.Fields.Item("Warehouse").Value.ToString();
                }
            }
            catch(Exception e) {
                HandleException(e, "GetUserDefaultWarehouse");
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return !String.IsNullOrEmpty(result) ? result : "PLHE";
        }
        #endregion

        #region GetUserCostCenter
        public string GetUserCostCenter() {

            Recordset lObjRecordset = null;
            Dictionary<string, string> lLstParams = new Dictionary<string, string>();
            string result = String.Empty;
            var currenUser = DIApplication.Company.UserName;

            try {

                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                lLstParams.Add("UserCode", currenUser);
                string lStrQuery = this.GetSQL("GetUserCostCenter");

                lObjRecordset.DoQuery(lStrQuery.Inject(lLstParams));

                if(lObjRecordset.RecordCount > 0) {
                    result = lObjRecordset.Fields.Item(0).Value.ToString();
                }
            }
            catch(Exception e) {
                HandleException(e, "GetUserCostCenter");
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return !String.IsNullOrEmpty(result) ? result : "PL_PLANT";
        }
        #endregion

        #region GetAvailableTransitWarehouse
        public string GetAvailableTransitWarehouse(string itemCode) {

            Recordset lObjRecordset = null;
            string result = String.Empty;
            var currenUser = DIApplication.Company.UserName;

            try {

                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(this.GetSQL("GetAvailableTransitWarehouse").Inject(new Dictionary<string, string>() { { "ItemCode", itemCode } }));
                if(lObjRecordset.RecordCount > 0) {
                    result = lObjRecordset.Fields.Item("WhsCode").Value.ToString();
                }
            }
            catch(Exception e) {
                HandleException(e, "GetAvailableTransitWarehouse");
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return result;
        }
        #endregion

        #region GetSeries
        public int GetSeries(string seriesName, string objectCode, string field) {

            Recordset lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset); ;
            int result = 0;

            try {

                string query = this.GetSQL("GetSeries").Replace("Field", field);
                lObjRecordset.DoQuery(query.Inject(new Dictionary<string, string>() { { "SeriesName", seriesName }, { "ObjectCode", objectCode } }));

                if(lObjRecordset.RecordCount > 0) {
                    result = (int)lObjRecordset.Fields.Item(field).Value;
                }
            }
            catch(Exception e) {

                HandleException(e, "GetSeries");
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return result;
        }
        #endregion

        #region GetComponents
        public Component[] GetComponents(string param, string whs, bool cancellation) {

            Recordset recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            Component[] components = null;

            try {

                string query = (whs.Equals("PLHE") ? "GetComponents" : !cancellation ? "GetMaterialList" : "GetComponentsFSE");
                //var test = (this.GetSQL(query).Inject(new Dictionary<string, string>() { { "Param", param }, { "Whs", whs } }));

                recordset.DoQuery(this.GetSQL(query).Inject(new Dictionary<string, string>() { { "Param", param }, { "Whs", whs } }));
                if(recordset.RecordCount > 0) {
                    components = new Component[recordset.RecordCount];

                    for(int i = 0; i < recordset.RecordCount; i++) {

                        var component = new Component();
                        Parallel.ForEach(recordset.Fields.OfType<Field>(), field => {
                            component.GetType().GetProperty(field.Name).SetValue(component, field.Value);
                        });
                        components[i] = component;
                        recordset.MoveNext();
                    }
                }
            }
            catch(Exception ex) {
                HandleException(ex, "GetTransferItems");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }

            return components ?? new Component[0];
        }
        #endregion

        #region GetPlannedQty
        public double GetPlannedQty(string docEntry) {

            Recordset lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            dynamic result = 0;

            try {
                string query = this.GetSQL("GetPlannedQty");
                lObjRecordset.DoQuery(query.Inject(new Dictionary<string, string>() { { "DocEntry", docEntry } }));

                if(lObjRecordset.RecordCount > 0) {
                    result = lObjRecordset.Fields.Item("PlannedQty").Value;
                }
            }
            catch(Exception e) {
                HandleException(e, "GetPlannedQty");
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return result;
        }
        #endregion

        #region GetProdItemBags
        public double GetProdItemBags(int docEntry) {

            Recordset lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset); ;
            Dictionary<string, string> lLstParams = new Dictionary<string, string>();
            dynamic result = 0;

            try {
                lObjRecordset.DoQuery(this.GetSQL("GetProdItemBags").Inject(new Dictionary<string, string>() { { "DocEntry", docEntry.ToString() } }));
                if(lObjRecordset.RecordCount > 0) {
                    result = lObjRecordset.Fields.Item(0).Value;
                }
            }
            catch(Exception e) {
                HandleException(e, "GetPlannedQty");
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return result;
        }
        #endregion

        #region GetSeriesNumbers
        public SeriesNumber[] GetSeriesNumbers(string docEntry) {

            Recordset recordset = null;
            SeriesNumber[] seriesNumbers = null;
            Dictionary<string, string> paramaters = new Dictionary<string, string>();
            string baseType = "67"; //BoObjectTypes.oStockTransfer 

            try {

                paramaters.Add("DocEntry", docEntry);
                paramaters.Add("BaseType", baseType);
                var query = this.GetSQL("GetSeriesNumbers").Inject(paramaters);

                recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                recordset.DoQuery(query);

                if(recordset.RecordCount > 0) {
                    seriesNumbers = new SeriesNumber[recordset.RecordCount];
                    for(int i = 0; i < recordset.RecordCount; i++) {
                        var seriesNumber = new SeriesNumber();
                        Parallel.ForEach(recordset.Fields.OfType<Field>(), field => {
                            seriesNumber.GetType().GetProperty(field.Name).SetValue(seriesNumber, field.Value);
                        });
                        seriesNumbers[i] = seriesNumber;
                        recordset.MoveNext();
                    }
                }
            }
            catch(Exception ex) {
                HandleException(ex, "GetSeriesNumbers");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }

            return seriesNumbers;
        }
        #endregion

        #region GetAccountCode
        public string GetAccountCode(string itemCode, string whs) {

            Recordset lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset); ;
            string result = "";

            try {

                string query = this.GetSQL("GetAccountCode");
                lObjRecordset.DoQuery(query.Inject(new Dictionary<string, string>() { { "ItemCode", itemCode }, { "Whs", whs } }));

                if(lObjRecordset.RecordCount > 0) {
                    result = lObjRecordset.Fields.Item(0).Value.ToString();
                }
            }
            catch(Exception e) {
                HandleException(e, "GetAccountCode");
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return result;
        }
        #endregion

        #region GetUserCode
        public string GetUserCode(string userID) {

            Recordset lObjRecordset = null;
            string result = String.Empty;
            var currenUser = DIApplication.Company.UserName;

            try {

                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(this.GetSQL("GetUserCodeBy").Inject(new Dictionary<string, string>() { { "UserID", userID } }));
                if(lObjRecordset.RecordCount > 0) {
                    result = lObjRecordset.Fields.Item(0).Value.ToString();
                }
            }
            catch(Exception e) {
                HandleException(e, "GetUserCodeBy");
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return result;
        }
        #endregion

        #region GetIsBagRequired
        public int GetIsBagRequired(string itemCode) {

            Recordset recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            int result = 0;

            try {
                recordset.DoQuery(this.GetSQL("GetIsBagRequired").Inject(new Dictionary<string, string>() { { "ItemCode", itemCode } }));

                if(recordset.RecordCount > 0) {
                    result = recordset.Fields.Item(0).Value.ToString() == "Y" ? 1 : 0;
                }
            }
            catch(Exception e) {
                HandleException(e, "GetIsBagRequired");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }
            return result;
        }
        #endregion

        #region GetActualCost
        public double GetActualCost(string itemCode, string whsCode) {

            Recordset recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            double result = 0;

            try {
                recordset.DoQuery(this.GetSQL("GetActualCost").Inject(new Dictionary<string, string>() { { "ItemCode", itemCode }, { "WhsCode", whsCode } }));

                if(recordset.RecordCount > 0) {
                    result = (double)recordset.Fields.Item(0).Value;
                }
            }
            catch(Exception e) {
                HandleException(e, "GetActualCost");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }
            return result;
        }
        #endregion

        #region GetItemStock
        public double GetItemStock(string itemCode, string whsCode) {

            Recordset recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            double result = 0;

            try {
                recordset.DoQuery(this.GetSQL("GetItemStock").Inject(new Dictionary<string, string>() { { "ItemCode", itemCode }, { "WhsCode", whsCode } }));

                if(recordset.RecordCount > 0) {
                    result = (double)recordset.Fields.Item(0).Value;
                }
            }
            catch(Exception e) {
                HandleException(e, "GetItemStock");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }
            return result;
        }
        #endregion

        #region GetCancelledOrders
        public string[] GetCancelledOrders(string whs) {

            Recordset recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset); ;
            string[] cancelledOrders = null;


            try {

                var series = GetSeries(whs, "60", "Series").ToString();
                recordset.DoQuery(this.GetSQL("GetCancelledOrders").Inject(new Dictionary<string, string>() { {"Series", series }}));

                if(recordset.RecordCount > 0) {
                    cancelledOrders = new string[recordset.RecordCount];
                    for(int i = 0; i < recordset.RecordCount; i++) {

                        cancelledOrders[i] = recordset.Fields.Item(0).Value.ToString();
                        recordset.MoveNext();
                    }
                }
            }
            catch(Exception ex) {
                HandleException(ex, "GetCancelledOrders");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }

            return cancelledOrders ?? new string[0];
        }
        #endregion

        #region GetProductionItem
        public Component GetProductionItem(int docEntry, string whs) {

            Recordset recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset); ;
            Component productioItem = new Component();

            try {

                recordset.DoQuery(this.GetSQL("GetProductionItem").Inject(new Dictionary<string, string>() { { "DocEntry", docEntry.ToString() }, { "Whs", whs } }));
                if(recordset.RecordCount > 0) {
                    Parallel.ForEach(recordset.Fields.OfType<Field>(), field => {
                        productioItem.GetType().GetProperty(field.Name).SetValue(productioItem, field.Value);
                    });
                }
            }
            catch(Exception ex) {
                HandleException(ex, "GetCancelledOrders");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }

            return productioItem;
        }
        #endregion

        #region GetRevalorizationCost
        public double GetRevalorizationCost(string exitID, string orderID) {

            var recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            double result = 0;

            try {

                var query = this.GetSQL("GetRevalorizationCost").Inject(new Dictionary<string, string>() { { "ExitID", exitID }, { "OrderID", orderID } });

                recordset.DoQuery(this.GetSQL("GetRevalorizationCost").Inject(new Dictionary<string, string>() { { "ExitID", exitID }, { "OrderID", orderID } }));
                if(recordset.RecordCount > 0) {
                    result = (double)recordset.Fields.Item(0).Value;
                }

            }
            catch(Exception ex) {
                HandleException(ex, "GetRevalorizationCost");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }

            return result;
        }
        #endregion



        #region GetDocTotalFromJournalEntry
        public double GetDocTotalFromJournalEntry(int transID) {

            var recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            double result = 0;

            try {

                var query = this.GetSQL("GetDocTotalFromJournalEntry").Inject(new Dictionary<string, string>() { { "TransId", transID.ToString() } });
                recordset.DoQuery(query);
                if(recordset.RecordCount > 0) {
                    result = (double)recordset.Fields.Item(0).Value;
                }
            }
            catch(Exception ex) {
                HandleException(ex, "GetRevalorizationCost");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }

            return result;
        }
        #endregion




        


        #region GetAccCodeRevaluation
        public string GetAccCodeRevaluation() {

            var recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            string result = String.Empty;

            try {
                recordset.DoQuery(this.GetSQL("GetAccCodeRevaluation"));
                if(recordset.RecordCount > 0) {
                    result = recordset.Fields.Item(0).Value.ToString();
                }

            }
            catch(Exception ex) {
                HandleException(ex, "GetRevalirizationCost");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }

            return result;
        }
        #endregion

        #region GetComponentsExitCosts
        public Component[] GetComponentsExitCosts(string docEntry, bool isFoodplant) {

            var recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            Component[] itemStockPrices = null;
            var objectType = (int)BoObjectTypes.oInventoryGenExit;

            try {

                string query = this.GetSQL("GetComponentsExitCosts").Inject(new Dictionary<string, string>() { { "DocEntry", docEntry }, { "ObjType", objectType.ToString() } });

                if(!isFoodplant) {
                    query = query.Replace("OWOR", "OIGN").Replace("U_GLO_DocNumSal", "U_MQ_OrigenFol");
                }

                recordset.DoQuery(query);

                if(recordset.RecordCount > 0) {
                    itemStockPrices = new Component[recordset.RecordCount];
                    for(int i = 0; i < recordset.RecordCount; i++) {
                        var component = new Component();
                        Parallel.ForEach(recordset.Fields.OfType<Field>(), field => {
                            component.GetType().GetProperty(field.Name).SetValue(component, field.Value);
                        });
                        itemStockPrices[i] = component;
                        recordset.MoveNext();
                    }
                }
            }
            catch(Exception ex) {
                HandleException(ex, "GetComponentsExitCosts");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }

            return itemStockPrices;
        }
        #endregion

        #region GetInventoryExitByDocNum
        public int GetInventoryExitByDocNum(string docNum) {

            var recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            int result = 0;

            try {
                recordset.DoQuery(this.GetSQL("GetInventoryExitByDocNum").Inject(new Dictionary<string, string>() { { "DocNum", docNum } }));
                if(recordset.RecordCount > 0) {
                    result = int.Parse(recordset.Fields.Item(0).Value.ToString());
                }
            }
            catch(Exception ex) {
                HandleException(ex, "GetInventoryExitByDocNum");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }

            return result;
        }
        #endregion

        #region GetProductionProcessUsers
        public string[] GetProductionProcessUsers() {

            Recordset recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset); ;
            string[] users = null;

            try {

                recordset.DoQuery(this.GetSQL("GetProductionProcessUsers"));

                if(recordset.RecordCount > 0) {
                    users = new string[recordset.RecordCount];
                    for(int i = 0; i < recordset.RecordCount; i++) {

                        users[i] = recordset.Fields.Item(0).Value.ToString();
                        recordset.MoveNext();
                    }
                }
            }
            catch(Exception ex) {
                HandleException(ex, "GetCancelledOrders");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }

            return users ?? new string[0];
        }
        #endregion

        #region GetUserType
        public bool GetUserType(string user) {

            Recordset recordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset); ;
            string userType = String.Empty; ;

            try {

                recordset.DoQuery(this.GetSQL("GetUserType").Inject(new Dictionary<string, string>() { { "User", user } }));

                if(recordset.RecordCount > 0) {
                    userType = recordset.Fields.Item(0).Value.ToString();
                }
            }
            catch(Exception ex) {
                HandleException(ex, "GetUserType");
            }
            finally {
                MemoryUtility.ReleaseComObject(recordset);
            }

            return userType.Equals("Y") ? true : false;
        }
        #endregion

        #region HandleException
        public static void HandleException(Exception ex, string section) {

            lock(padlock) {
                UIApplication.ShowMessageBox(String.Format("Error: {0}", ex.Message));
                LogService.WriteError(String.Format("{0}: {1}, {2}", section, ex.Message, ex.StackTrace));
            }
        }
        #endregion
    }
}
