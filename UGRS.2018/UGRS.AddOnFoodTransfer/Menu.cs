/*
 * Autor: LCC Abraham SaÚL Sandoval Meneses
 * Description: Transfer of Transit Inventory to Area Form
 * Date: 31/08/2018
 */


using SAPbouiCOM.Framework;
using System;
using UGRS.AddOnFoodTransfer.Forms;
using UGRS.Core.SDK.DI.FoodTransfer.DAO;
using System.Linq;
using UGRS.Core.Services;

namespace UGRS.AddOnFoodTransfer {

    class Menu {

        SAPbouiCOM.Item btnOpen;
        SAPbouiCOM.Menus oMenus = null;
        SAPbouiCOM.MenuItem oMenuItem = null;
        SAPbouiCOM.MenuCreationParams oCreationPackage = null;

        public void AddMenuItems() {

            oMenus = Application.SBO_Application.Menus;
            oCreationPackage = ((SAPbouiCOM.MenuCreationParams)(Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));
            oMenuItem = Application.SBO_Application.Menus.Item("43520"); // moudles'

            oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_POPUP;
            oCreationPackage.UniqueID = "UGRS.AddOnFoodTransfer";
            oCreationPackage.String = "Transferencias";
            oCreationPackage.Enabled = true;
            oCreationPackage.Position = -1;

            oMenus = oMenuItem.SubMenus;

            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);

            try {
                //  If the manu already exists this code will fail
                oMenus.AddEx(oCreationPackage);
            }
            catch {
            }

            try {
                // Get the menu collection of the newly added pop-up item
                oMenuItem = Application.SBO_Application.Menus.Item("UGRS.AddOnFoodTransfer");
                oMenus = oMenuItem.SubMenus;


                // Create s sub menus
                CreateSubMenu("UGRS.AddOnFoodTransfer.Forms.frmTransfer", "Transferencias Pendientes");
               // CreateSubMenu("UGRS.AddOnFoodTransfer.Forms.frmItems", "Transferencia de Artículos");

                if(CheckUser(SAPbouiCOM.Framework.Application.SBO_Application.Company.UserName)) {
                    CreateSubMenu("UGRS.AddOnFoodTransfer.Forms.frmProcess", "Proceso de Producción");
                }

            }
            catch { //  Menu already exists
                //Applic|ation.SBO_Application.SetStatusBarMessage("Menu Already Exists", SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }

        public bool CheckUser(string currentUser) {
          
            var pProcessUsers = new FoodTransferDAO().GetProductionProcessUsers();
            if(pProcessUsers.Length > 0 && pProcessUsers.Contains(currentUser))
                return true;
           
            return false;
        }

        public void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent) {

            BubbleEvent = true;
            try {
                if(pVal.BeforeAction) {
                    switch(pVal.MenuUID) {
                        case "UGRS.AddOnFoodTransfer.Forms.frmTransfer":
                        frmTransfer ofrmTransfer = new frmTransfer();
                        ShowForm(ofrmTransfer);

                        break;

                        case "UGRS.AddOnFoodTransfer.Forms.frmItems":
                        frmItems ofrmItems = new frmItems("0");
                        ShowForm(ofrmItems);
                        break;

                        case "UGRS.AddOnFoodTransfer.Forms.frmProcess":
                        frmProcess ofrmProcess = new frmProcess();
                        ShowForm(ofrmProcess);
                        break;
                    }
                }
                if(!pVal.BeforeAction) {
                    switch(pVal.MenuUID) {
                        case "3088":

                        SAPbouiCOM.Form form = Application.SBO_Application.Forms.ActiveForm;

                        if(form.Title == "Solicitud de traslado" || form.Type.Equals(1250000940)) {
                            AddButtonToForm(form);
                        }
                        break;
                    }
                }
            }
            catch(Exception ex) {
                Application.SBO_Application.MessageBox(ex.ToString(), 1, "Ok", "", "");
            }
        }
        private void BtnOpen_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;

            SAPbouiCOM.Form form = Application.SBO_Application.Forms.ActiveForm;
            var docNum = ((SAPbouiCOM.EditText)form.Items.Item("11").Specific).Value;

            frmItems ofrmItems = new frmItems(docNum);
            ShowForm(ofrmItems);
        }

        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent) {
            BubbleEvent = true;

            try {

                if(FormUID.Equals(FormUID)) {
                    if(!pVal.BeforeAction) {
                        switch(pVal.EventType) {
                            case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE:

                            if(!Object.ReferenceEquals(btnOpen, null)) {

                                SAPbouiCOM.Form form = Application.SBO_Application.Forms.ActiveForm;
                                if(form.Title == "Solicitud de traslado" || form.Type.Equals(1250000940)) {
                                    btnOpen.Left = (form.Left / 2) + 150;
                                    btnOpen.Top = form.Items.Item("2").Top;
                                }
                            }
                            break;

                            case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED:
                            SAPbouiCOM.Form frm = Application.SBO_Application.Forms.ActiveForm;
                           
                            if(frm.Title.Equals("Solicitud de traslado") || frm.Type.Equals(1250000940)) {
                                AddButtonToForm(frm);
                            }
                            break;
                        }
                    }
                }
            }
            catch(Exception ex) {
                LogService.WriteError(ex.Message + " " + ex.StackTrace);
            }
        }

        public void CreateSubMenu(string id, string title) {
            oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
            oCreationPackage.UniqueID = id;
            oCreationPackage.String = title;
            oMenus.AddEx(oCreationPackage);
        }

        public void ShowForm(UserFormBase frm) {
            frm.UIAPIRawForm.Left = 500;
            frm.UIAPIRawForm.Top = 10;
            frm.Show();
        }

        public void AddButtonToForm(SAPbouiCOM.Form form) {

            btnOpen = form.Items.Add("btnOpen", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
            btnOpen.Top = form.Items.Item("2").Top;
            btnOpen.Left = (form.Left / 2) + 150;
            btnOpen.Width = 170;
            (btnOpen.Specific as SAPbouiCOM.Button).Caption = "Abrir Tranferencia de Artículos";
            btnOpen.LinkTo = "2";
            (btnOpen.Specific as SAPbouiCOM.Button).ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.BtnOpen_ClickBefore);
        }
    }
}
