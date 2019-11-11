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
using UGRS.Core.Services;
using UGRS.Core.Utility;

namespace UGRS.Core.SDK.DI.FoodTransfer.Services {

    public static class StockExitDI {

        public static Result CreateDocument(DocumentProduction document, User user, bool cancellation, DateTime prodDate) {

            var result = new Result();
            FoodTransferDAO foodTransferDAO = new FoodTransferDAO();
            var oStockExit = (Documents)DIApplication.Company.GetBusinessObject(BoObjectTypes.oInventoryGenExit); //OIGE

            try {

                var task = Task.Factory.StartNew(() => {
                    oStockExit.Series = foodTransferDAO.GetSeries(user.IsFoodPlant ? user.WhsCode : document.Lines[0].Whs, "60", "Series");
                    oStockExit.UserFields.Fields.Item("U_MQ_OrigenFol").Value = user.IsFoodPlant ? document.DocNum : (!cancellation ? String.Empty : document.DocNum);
                    oStockExit.UserFields.Fields.Item("U_GLO_ObjType").Value = user.FormID;
                    oStockExit.UserFields.Fields.Item("U_GLO_InMo").Value = !cancellation ? "S-PROD" : "S-PRODCANCEL";
                    oStockExit.DocDate = prodDate;
                    if (!user.IsFoodPlant) {
                        oStockExit.TaxDate = DateTime.Now;
                    }

                    if(cancellation) {
                        oStockExit.UserFields.Fields.Item("U_GLO_Status").Value = "C";
                    }
                });

                var task2 = Task.Factory.StartNew(() => {
                    foreach(var line in document.Lines.Where(l => l.Qty > 0 && l.Inventorial.Equals(0))) {

                        if(!user.IsFoodPlant && line.Resource == 1)
                            continue;

                        oStockExit.Lines.ItemDescription = line.Desc;
                        oStockExit.Lines.Quantity = (cancellation && !user.IsFoodPlant) ? line.Plan : line.Qty;
                        oStockExit.Lines.CostingCode = user.Area;
                        oStockExit.Lines.WarehouseCode = line.Whs;
                        oStockExit.Lines.AccountCode = line.AccCode;
                        oStockExit.Lines.UserFields.Fields.Item("U_GLO_BagsBales").Value = line.Bags;

                        if(user.IsFoodPlant && !cancellation) {
                            oStockExit.Lines.BaseEntry = document.DocEntry;
                            oStockExit.Lines.BaseLine = line.LineNum;
                            oStockExit.Lines.BaseType = (int)BoObjectTypes.oProductionOrders;
                        }
                        else {
                            oStockExit.Lines.ItemCode = line.Item;
                        }
                        oStockExit.Lines.Add();
                    }
                });

                Task.WaitAll(task, task2);

                if(oStockExit.Add() != 0) {
                    LogService.WriteError("TransferDI (Document) " + DIApplication.Company.GetLastErrorDescription());
                    result.Success = false;
                    result.Message = "Error: " + DIApplication.Company.GetLastErrorDescription();
                }
                else {

                    result.DocEntry = int.Parse(DIApplication.Company.GetNewObjectKey());
                    oStockExit.GetByKey(result.DocEntry);
                    result.Success = true;
                    //result.DocTotal = oStockExit.DocTotal;
                    result.DocTotal = foodTransferDAO.GetDocTotalFromJournalEntry(oStockExit.TransNum);
                  
                   
                    result.Message = String.Format("La salida de mercancia#{0} se realizó con éxito!", oStockExit.DocNum);
                }
            }
            catch(Exception ex) {
                result.Success = false;
                result.Message = ex.Message;
                HandleException(ex, "StockExitDI.CreateDocument");
            }
            finally {
                MemoryUtility.ReleaseComObject(oStockExit);
            }
            return result;
        }

        #region HandleException
        public static void HandleException(Exception ex, string section) {
            LogService.WriteError(String.Format("{0}: {1}, {2}", section, ex.Message, ex.StackTrace));
        }
        #endregion

    }
}
