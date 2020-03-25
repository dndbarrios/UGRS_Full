using System;
using System.Collections.Generic;
using SAPbouiCOM.Framework;
using UGRS.Core.SDK.DI;
using UGRS.Core.Services;

namespace UGRS.AddOn.Corrals {
    class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            try {
                Application oApp = null;
                if (args.Length < 1) {
                    oApp = new Application();
                }
                else {
                    oApp = new Application(args[0]);
                }
             
                LogService.Filename("AddOnCorrals");
                Menu MyMenu = new Menu();
                MyMenu.AddMenuItems();
                oApp.RegisterMenuEventHandler(MyMenu.SBO_Application_MenuEvent);
                DIApplication.DIConnect((SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany());
                Application.SBO_Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(SBO_Application_AppEvent);
                LogService.WriteInfo("AddOnCorrals 1.0.30 initialized successfully");

                oApp.Run();
            }
            catch (Exception ex) {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        static void SBO_Application_AppEvent(SAPbouiCOM.BoAppEventTypes EventType) {
            switch (EventType) {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                //Exit Add-On
                System.Windows.Forms.Application.Exit();
                break;
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                break;
                case SAPbouiCOM.BoAppEventTypes.aet_FontChanged:
                break;
                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                break;
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                break;
                default:
                break;
            }
        }
    }
}
