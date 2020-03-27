using SAPbouiCOM;

namespace Core.SDK.UI
{
    public class UIApplication
    {
        private static SAPbouiCOM.Company m_Company;

        public static bool IsActiveForm(string uniqueId)
        {
            return SAPbouiCOM.Framework.Application.SBO_Application.Forms.ActiveForm.UniqueID == uniqueId ? true : false;
        }

        public static SAPbouiCOM.Application GetApplication()
        {
            return SAPbouiCOM.Framework.Application.SBO_Application;
        }

        public static SAPbouiCOM.Company GetCompany()
        {
            if (m_Company == null)
                m_Company = (SAPbouiCOM.Company)SAPbouiCOM.Framework.Application.SBO_Application.Company;

            return m_Company;
        }

        public static void ShowMessageBox(string pStrMessage)
        {
            SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(pStrMessage, 1, "Ok", "", "");
        }

        public static int ShowOptionBox(string pStrMessage)
        {
            return SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(pStrMessage, 1, "Aceptar", "Cancelar", "");
        }

        public static void ShowMessage(string pStrMessage)
        {
            SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText(pStrMessage, BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_None);
        }

        public static void ShowWarning(string pStrMessage)
        {
            SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText(pStrMessage, BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Warning);
        }

        public static void ShowSuccess(string pStrMessage)
        {
            SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText(pStrMessage, BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Success);
        }

        public static void ShowError(string pStrMessage)
        {
            SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText(pStrMessage, BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Error);
        }
    }
}
