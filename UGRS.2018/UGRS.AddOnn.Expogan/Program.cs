using System;
using System.Collections.Generic;
using SAPbouiCOM.Framework;
using UGRS.Core.SDK.DI;
using UGRS.Core.Services;
using UGRS.Core.SDK.UI;
using UGRS.Core.SDK.DI.Expogan;

namespace UGRS.AddOnn.Expogan
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
              
                Application oApp = null;
                if (args.Length < 1)
                {
                    oApp = new Application();
                }
                else
                {
                    oApp = new Application(args[0]);
                }
                LogService.Filename("AddonExpogan");

                Menu MyMenu = new Menu();
                MyMenu.AddMenuItems();
                DIApplication.DIConnect((SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany());
                oApp.RegisterMenuEventHandler(MyMenu.SBO_Application_MenuEvent);
                Application.SBO_Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(SBO_Application_AppEvent);

                //Initialize Tables 
                UIApplication.ShowSuccess(string.Format("Inicializar las tablas"));
                ExpoganServiceFactory lObjExpoganFactory = new ExpoganServiceFactory();
                lObjExpoganFactory.GetSetupService().InitializeTables();
                UIApplication.ShowSuccess(string.Format("AddonExpogan 0.0.12 iniciado correctamente"));
                LogService.WriteSuccess(string.Format("AddonExpogan 0.0.12 iniciado correctamente"));

                oApp.Run();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        static void SBO_Application_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            switch (EventType)
            {
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
