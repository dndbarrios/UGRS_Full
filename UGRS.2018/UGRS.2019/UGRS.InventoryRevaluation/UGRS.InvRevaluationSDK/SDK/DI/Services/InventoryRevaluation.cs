﻿using SAPbobsCOM;
using System;
using UGRS.Core.Utility;

using UGRS.InvRevaluationSDK.Models;

namespace UGRS.InvRevaluationSDK.SDK.DI.Services {
    public class InventoryRevaluation {

        static Object padLock = new Object();
        public bool Insert(string docNum, RevaluationItem[] items, int type, string docTypeES) {
            try {
                //if (debitCredit == 0) {
                //    return true;
                //}
                var oMaterialRevaluation = (MaterialRevaluation)DIApplication.Company.GetBusinessObject(BoObjectTypes.oMaterialRevaluation); //162

                foreach (var item in items) {

                    oMaterialRevaluation.DocDate = type.Equals(1) ? item.DocDateRev1 : item.DocDateRev2;
                    oMaterialRevaluation.RevalType = "M";
                    oMaterialRevaluation.UserFields.Fields.Item("U_DocNumSalida").Value = $"{docTypeES}{docNum}";
                    oMaterialRevaluation.Lines.ItemCode = item.ItemCode;
                    oMaterialRevaluation.Lines.WarehouseCode = item.whCode;
                    oMaterialRevaluation.Lines.Quantity = 1;
                    oMaterialRevaluation.Lines.DebitCredit = type.Equals(1)? item.Rev1 : item.Rev2;

                    oMaterialRevaluation.Lines.Add();
                }

                lock (padLock) {
                    if (oMaterialRevaluation.Add().Equals(0)) {
                        var key = DIApplication.Company.GetNewObjectKey();
                        LogEntry.WriteInfo($"Inventory Revaluation Created Successfully: {key} ,For GE {docNum}");
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

