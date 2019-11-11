using System;
using UGRS.CheckAdminSDK.SDK.DI;
using UGRS.Core.Utility;
using UGRS.CheckAdminSDK.SDK.UI;
using SAPbobsCOM;

namespace UGRS.CheckAdminSDK.Services {
    public class AlertMessage {
        public bool Insert(string userCode, string subject) {

            Messages oMessages = (Messages)DIApplication.Company.GetBusinessObject(BoObjectTypes.oMessages);
            try {
                oMessages.Subject = subject;
                oMessages.Recipients.Add();
                oMessages.Recipients.UserCode = userCode;
                oMessages.Recipients.UserType = BoMsgRcpTypes.rt_InternalUser;
                oMessages.Recipients.SendInternal = BoYesNoEnum.tYES;
                oMessages.Recipients.SendEmail = BoYesNoEnum.tNO;
          
                if (oMessages.Add() != 0) {
                    var errorMsg = DIApplication.Company.GetLastErrorDescription();
                    UIApplication.ShowMessageBox(errorMsg);
                    LogEntry.WriteError($"Failed to create Alert Message: {errorMsg}");
                    return false;
                }
                else {
                    LogEntry.WriteSuccess($"Alerta enviada correctamente al usuario: {userCode}" );
                    return true;
                }
            }
            catch (Exception ex) {
                LogEntry.WriteException(ex);
            }
            finally {
                MemoryUtility.ReleaseComObject(oMessages);
            }
            return false;
        }
    }
}
