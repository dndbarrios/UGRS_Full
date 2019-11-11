/*
Autor: LCC Abraham Saúl Sandoval Meneses
Description: Transfers For Food Plant Data Object
Date: 31/08/2018
Company: Qualisys
*/


using SAPbobsCOM;
using System;
using UGRS.Core.SDK.DI.FoodTransfer.DAO;
using UGRS.Core.SDK.DI.FoodTransfer.DTO;
using UGRS.Core.Services;
using UGRS.Core.Utility;

namespace UGRS.Core.SDK.DI.FoodTransfer.Services {

    public class InventoryRevaluationDI {

        public static Result CreateDocument(Component productionItem, User user, int exitID, int orderID) {

            var result = new Result();
            FoodTransferDAO transferDAO = new FoodTransferDAO();

            try {
                var accCodeRevaluation = transferDAO.GetAccCodeRevaluation();
                var oStockExit = (Documents)DIApplication.Company.GetBusinessObject(BoObjectTypes.oInventoryGenExit);
                oStockExit.GetByKey(exitID);
                oStockExit.Lines.SetCurrentLine(0);

                var oMaterialRevaluation = (MaterialRevaluation)DIApplication.Company.GetBusinessObject(BoObjectTypes.oMaterialRevaluation); //162
                oMaterialRevaluation.Series = transferDAO.GetSeries(user.WhsCode, "162", "Series");
                oMaterialRevaluation.DocDate = DateTime.Now;
                oMaterialRevaluation.RevalType = "M";

                oMaterialRevaluation.Lines.ItemCode = productionItem.Item;
                oMaterialRevaluation.Lines.RevaluationDecrementAccount = accCodeRevaluation;
                oMaterialRevaluation.Lines.RevaluationIncrementAccount = accCodeRevaluation;
                oMaterialRevaluation.Lines.WarehouseCode = productionItem.Whs;
                oMaterialRevaluation.Lines.Quantity = 1;
                oMaterialRevaluation.Lines.DebitCredit = transferDAO.GetRevalorizationCost(exitID.ToString(), orderID.ToString());
                oMaterialRevaluation.Lines.DistributionRule = user.Area;

                LogService.WriteInfo("Revaluation " + oMaterialRevaluation.Lines.DebitCredit.ToString());

                if(oMaterialRevaluation.Lines.DebitCredit == 0) {
                    result.Success = false;
                    return result;
                }
                //oMaterialRevaluation.Lines.Price = Math.Abs(transferDAO.GetActualCost(productionItem.Item, user.WhsCode) - oStockExit.Lines.Price);
                oMaterialRevaluation.Lines.Add();

                if(oMaterialRevaluation.Add() != 0) {
                    LogService.WriteError("TransferDI (Draft) " + DIApplication.Company.GetLastErrorDescription());
                    result.Success = false;
                    result.Message = "Error: " + DIApplication.Company.GetLastErrorDescription();

                }
                else {
                    result.Success = true;
                    result.Message = "La Revalorización de Inventario se realizó con éxito!";
                }
            }

            catch(Exception ex) {
                HandleException(ex, "StockEntryDI.CreateDocument");
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
