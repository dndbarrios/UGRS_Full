using System;
using UGRS.CheckAdminSDK.DAO;
using UGRS.CheckAdminSDK.Models;
using UGRS.CheckAdminSDK.SDK.DI;
using UGRS.CheckAdminSDK.SDK.DI.DAO;
using UGRS.CheckAdminSDK.SDK.Tables;
using UGRS.Core.Utility;

namespace UGRS.CheckAdminSDK.Services {
    public class CheckAdministration {

        TableDAO<CheckAdmin> tableDAO = new TableDAO<CheckAdmin>();

        public Result Insert(CheckAdmin checkAdmin) {

            var result = new Result();
            var msg = "";
            try {
                if (tableDAO.Add(checkAdmin).Equals(0)) {
                    msg = $"Se creo el registro de administración de cheque: {checkAdmin.CheckNum}";
                    LogEntry.WriteSuccess(msg);
                    result.Success = true;
                }
                else {
                    msg = $"Fallo en crearse el registro de administración de cheque# {checkAdmin.CheckNum}: {DIApplication.Company.GetLastErrorDescription()}";
                    LogEntry.WriteError(msg);
                }
                result.Msg = msg;
            }
            catch (Exception ex) {
                LogEntry.WriteException(ex);
            }
            return result;
        }

        public Result Update(CheckAdmin checkAdmin) {
            var result = new Result();
            var msg = "";

            try {
                var checkAdminDAO = new CheckAdminDAO();
                var key = checkAdminDAO.GetTableKey(checkAdmin.CheckNum);

                if (tableDAO.Update(checkAdmin, key).Equals(0)) {
                    msg = $"Se actualizo el registro de administración de cheque: {checkAdmin.CheckNum}";
                    LogEntry.WriteSuccess(msg);
                    result.Success = true;
                }
                else {
                    msg = $"Fallo en actualizarse el registro de administración de cheque# {checkAdmin.CheckNum}: {DIApplication.Company.GetLastErrorDescription()}";
                    LogEntry.WriteError(msg);
                }
                result.Msg = msg;
            }
            catch (Exception ex) {
                LogEntry.WriteException(ex);
            }
            return result;
        }
    }
}
