/*
Autor: LCC Abraham Saúl Sandoval Meneses
Description: Transfers For Food Plant Data Object
Date: 31/08/2018
Company: Qualisys
*/


using SAPbobsCOM;
using System;
using System.Linq;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI.FoodTransfer.DAO;
using UGRS.Core.SDK.DI.FoodTransfer.DTO;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;
using UGRS.Core.Utility;

namespace UGRS.Core.SDK.DI.FoodTransfer.Services {

    public class StockEntryDI {

        public static Result CreateDocument(DocumentProduction document, User user, bool cancellation, DateTime prodDate) {

            var result = new Result();
            FoodTransferDAO foodPlantDAO = new FoodTransferDAO();
            Component[] itemCosts = null;
            var oStockEntry = (Documents)DIApplication.Company.GetBusinessObject(BoObjectTypes.oInventoryGenEntry); //OIGN

            try {
               
                var task = Task.Factory.StartNew(() => {

                    oStockEntry.Series = foodPlantDAO.GetSeries(user.IsFoodPlant ? user.WhsCode : document.Lines[0].Whs, "59", "Series");
                    oStockEntry.UserFields.Fields.Item("U_GLO_ObjType").Value = user.FormID;
                    oStockEntry.UserFields.Fields.Item("U_GLO_InMo").Value = !cancellation ? "E-PROD" : "E-PRODCANCEL";
                    oStockEntry.DocDate = prodDate;
                    if (!user.IsFoodPlant) {
                        oStockEntry.TaxDate = DateTime.Now;
                    }

                    if(!user.IsFoodPlant && cancellation) {
                        oStockEntry.UserFields.Fields.Item("U_GLO_Status").Value = "C";
                    }

                });

                //Line Item
                var task2 = Task.Factory.StartNew(() => {

                    if(cancellation) {
                        itemCosts = foodPlantDAO.GetComponentsExitCosts(document.DocEntry.ToString(), user.IsFoodPlant);
                    }

                    foreach(var line in document.Lines.Where(l => l.Resource == 0 && l.Qty > 0 && l.Inventorial == 0)) { //exclude resource items

                        oStockEntry.Lines.ItemDescription = line.Desc;
                        oStockEntry.Lines.Quantity = line.Qty;
                        oStockEntry.Lines.CostingCode = user.Area;
                        oStockEntry.Lines.WarehouseCode = line.Whs;
                        oStockEntry.Lines.AccountCode = line.AccCode;
                        oStockEntry.Lines.UserFields.Fields.Item("U_GLO_BagsBales").Value = line.Bags;

                        if(cancellation) {
                            oStockEntry.Lines.LineTotal = itemCosts.Single(i => i.Item == line.Item).LineTotal;
                            oStockEntry.Lines.ItemCode = line.Item;
                        }
                        else if(user.IsFoodPlant && !cancellation) {
                            oStockEntry.Lines.BaseEntry = document.DocEntry;
                            oStockEntry.Lines.BaseType = (int)BoObjectTypes.oProductionOrders;
                        }
                        else if(!user.IsFoodPlant && !cancellation) {
                            oStockEntry.Lines.LineTotal = line.LineTotal;
                            oStockEntry.Lines.ItemCode = line.Item;
                        }
                        else {
                            oStockEntry.Lines.ItemCode = line.Item;
                        }
                        oStockEntry.Lines.Add();
                    }
                });

                Task.WaitAll(new Task[] { task, task2 });

                if(oStockEntry.Add() != 0) {
                    LogService.WriteError("TransferDI (Draft) " + DIApplication.Company.GetLastErrorDescription());
                    result.Success = false;
                    result.Message = "Error: " + DIApplication.Company.GetLastErrorDescription();
                }
                else {
                    result.DocEntry = int.Parse(DIApplication.Company.GetNewObjectKey());
                    result.Success = true;
                    oStockEntry.GetByKey(result.DocEntry);
                    result.Message = String.Format("La entrada de mercancia#{0} se realizó con éxito!", oStockEntry.DocNum);

                    if(!user.IsFoodPlant && cancellation) {
                        CancelPreviousDocument(oStockEntry, document.DocEntry);
                    }
                }
            }

            catch(Exception ex) {
                result.Success = false;
                result.Message = ex.Message;
                HandleException(ex, "StockEntryDI.CreateDocument");
            }
            finally {
                MemoryUtility.ReleaseComObject(oStockEntry);
            }

            return result;
        }

        public static void CancelPreviousDocument(Documents oStockEntry, int docEntry) {

            oStockEntry.GetByKey(docEntry);
            oStockEntry.UserFields.Fields.Item("U_GLO_Status").Value = "C";

            if(oStockEntry.Update() != 0) {
                LogService.WriteError("StockEntryDI (CancelDocument) " + DIApplication.Company.GetLastErrorDescription());
                UIApplication.ShowMessageBox("Fallo en cancelarse la entrada de producto terminado");
            }
        }

        public static bool UpdateDocument(int entryID, int exitID, bool cancellation) {

            Documents oStockEntry = null;
            Documents oStockExit = null;

            try {
                Parallel.Invoke(
                    () => {
                        oStockEntry = (Documents)DIApplication.Company.GetBusinessObject(BoObjectTypes.oInventoryGenEntry);
                        oStockEntry.GetByKey(entryID);
                    },
                    () => {
                        oStockExit = (Documents)DIApplication.Company.GetBusinessObject(BoObjectTypes.oInventoryGenExit);
                        oStockExit.GetByKey(exitID);
                    });


                oStockEntry.UserFields.Fields.Item("U_MQ_OrigenFol").Value = oStockExit.DocNum.ToString();

                if(oStockEntry.Update() != 0) {
                    LogService.WriteError("StockEntryDI (UpdateoStockEntryStatus) " + DIApplication.Company.GetLastErrorDescription());
                    UIApplication.ShowMessageBox("Fallo en referenciarse la entrada con la salida: " + DIApplication.Company.GetLastErrorDescription());
                    return false;
                }
            }
            catch(Exception ex) {

                HandleException(ex, "UpdateDocument");
                MemoryUtility.ReleaseComObject(oStockEntry);
                MemoryUtility.ReleaseComObject(oStockExit);
                return false;
            }
            return true;
        }

        #region HandleException
        public static void HandleException(Exception ex, string section) {
            LogService.WriteError(String.Format("{0}: {1}, {2}", section, ex.Message, ex.StackTrace));
        }
        #endregion
    }
}
