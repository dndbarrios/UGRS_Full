using SAPbouiCOM.Framework;
using System;
using UGRS.CheckAdminSDK.SDK.UI;

namespace CheckAdministration {
    class Menu {
        public void AddMenuItems() {
            SAPbouiCOM.Menus oMenus = null;
            SAPbouiCOM.MenuItem oMenuItem = null;

            oMenus = Application.SBO_Application.Menus;

            SAPbouiCOM.MenuCreationParams oCreationPackage = null;
            oCreationPackage = ((SAPbouiCOM.MenuCreationParams)(Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));
            oMenuItem = Application.SBO_Application.Menus.Item("43520"); // moudles'

            oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_POPUP;
            oCreationPackage.UniqueID = "CheckAdministration";
            oCreationPackage.String = "Administración de Cheques";
            oCreationPackage.Enabled = true;
            oCreationPackage.Position = -1;

            oMenus = oMenuItem.SubMenus;

            try {
                //  If the manu already exists this code will fail
                oMenus.AddEx(oCreationPackage);
            }
            catch {

            }

            try {
                // Get the menu collection of the newly added pop-up item
                oMenuItem = Application.SBO_Application.Menus.Item("CheckAdministration");
                oMenus = oMenuItem.SubMenus;

                // Create s sub menu
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "CheckAdministration.Form1";
                oCreationPackage.String = "Cheques Emitidos";
                oMenus.AddEx(oCreationPackage);
            }
            catch { //  Menu already exists
                UIApplication.ShowError("Menu Already Exists");
            }
        }

        public void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            try {
                if (pVal.BeforeAction && pVal.MenuUID == "CheckAdministration.Form1") {
                    CheckAdmonForm activeForm = new CheckAdmonForm();
                    activeForm.Show();
                }
            }
            catch (Exception ex) {
                Application.SBO_Application.MessageBox(ex.ToString(), 1, "Ok", "", "");
            }
        }
    }
}
