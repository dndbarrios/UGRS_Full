using SAPbobsCOM;
using System;
using UGRS.Core.Utility;

namespace UGRS.InvRevaluationSDK.SDK.DI.Services {
    public class InventoryRevaluation {

        static Object padLock = new Object();
        public bool Insert(string itemCode, string warehouse, double debitCredit, DateTime docDate, string docNum) {
            try {
                if (debitCredit == 0) {
                    return true;
                }

                var oMaterialRevaluation = (MaterialRevaluation)DIApplication.Company.GetBusinessObject(BoObjectTypes.oMaterialRevaluation); //162
                oMaterialRevaluation.DocDate = docDate;
                oMaterialRevaluation.RevalType = "M";
                oMaterialRevaluation.UserFields.Fields.Item("U_DocNumSalida").Value = docNum;
                oMaterialRevaluation.Lines.ItemCode = itemCode;
                oMaterialRevaluation.Lines.WarehouseCode = warehouse;
                oMaterialRevaluation.Lines.Quantity = 1;
                oMaterialRevaluation.Lines.DebitCredit = debitCredit;
       
                oMaterialRevaluation.Lines.Add();

                lock (padLock) {
                    if (oMaterialRevaluation.Add().Equals(0)) {
                        var key = DIApplication.Company.GetNewObjectKey();
                        LogEntry.WriteInfo($"Inventory Revaluation Created Successfully: {key} ,For GE {docNum} ,Item {itemCode} and DebitCredit {debitCredit}");
                        return true;
                    }
                    else {
                        var msg = DIApplication.Company.GetLastErrorDescription();
                        LogEntry.WriteInfo($"Failed to Insert Revaluation: {msg}");
                    }
                }
            }
            catch (Exception ex) {
                LogEntry.WriteException(ex);
            }
            return false;
        }
    }
}

