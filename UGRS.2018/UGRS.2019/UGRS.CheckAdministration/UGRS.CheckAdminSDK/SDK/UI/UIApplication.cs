using SAPbouiCOM.Framework;


namespace UGRS.CheckAdminSDK.SDK.UI {
    public class UIApplication {
        public static bool IsActiveForm(string uniqueId) => Application.SBO_Application.Forms.ActiveForm.UniqueID == uniqueId ? true : false;
        public static SAPbouiCOM.Application GetApplication() => Application.SBO_Application;
        public static SAPbobsCOM.Company GetDICompany() => (SAPbobsCOM.Company)GetApplication().Company.GetDICompany();
        public static void ShowMessageBox(string message) => Application.SBO_Application.MessageBox(message, 1, "Ok", "", "");
        public static int ShowOptionBox(string message) => Application.SBO_Application.MessageBox(message, 1, "Aceptar", "Cancelar", "");
        public static void ShowError(string message) => Application.SBO_Application.StatusBar.SetText(message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
        public static void ShowMessage(string message) => Application.SBO_Application.StatusBar.SetText(message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_None);
        public static void ShowWarning(string message) => Application.SBO_Application.StatusBar.SetText(message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
        public static void ShowSuccess(string message) => Application.SBO_Application.StatusBar.SetText(message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
    }
}
