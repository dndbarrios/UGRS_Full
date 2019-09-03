using SAPbouiCOM.Framework;
using System;
using System.Runtime.Remoting;
using UGRS.AddOn.FoodProduction.UI;
using UGRS.AddOn.FoodProduction.UI.Event;
using UGRS.AddOn.FoodProduction.UI.Menu;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.FoodProduction.Services;
using UGRS.Core.Services;

namespace UGRS_AddOn.FoodProduction
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
      
        [STAThread]
        static void Main(string[] pArrStrArgs)
        {

           
               try
               {
                   LogService.Filename("FoodProduction");
                   LogService.WriteInfo("Inicio del addon 1.3.8");

                   Application lObjApplication = null;

                   if (pArrStrArgs.Length < 1)
                   {
                       lObjApplication = new Application();
                   }
                   else
                   {
                       lObjApplication = new Application(pArrStrArgs[0]);
                   }
                 

                   //Prepare menu
                   MenuManager lObjMenuManager = new MenuManager();
                   lObjMenuManager.Initialize();

                   //Prepare filters 
                   EventManager lObjEventManager = new EventManager();
                   UIApplication.GetApplication().SetFilter(lObjEventManager.GetItemEventFiltersByMenu(lObjMenuManager.Menu));
                   UIApplication.ShowSuccess(string.Format("Menu manager"));

                   //Add menu events
                   lObjApplication.RegisterMenuEventHandler(lObjMenuManager.GetApplicationMenuEvent);
                   LogService.WriteSuccess("Registro del menú");
                   UIApplication.ShowSuccess(string.Format("Registro del menu"));
                 

                   //Add application events
                   UIApplication.GetApplication().AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(GetApplicationAppEvent);
                   DIApplication.DIConnect((SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany());
                   UIApplication.ShowSuccess(string.Format("DI Connect"));
                   //Bascula
                   ConnectRemoteAccess();


                   //Initialize Tables 
                   UIApplication.ShowSuccess(string.Format("Inicializar las tablas"));
                   SetupService mObjSetupService;
                   mObjSetupService = new SetupService();
                   mObjSetupService.InitializeTables();


                   UIApplication.ShowSuccess(string.Format("Addon Iniciado correctamente"));
                   LogService.WriteSuccess("[AddOn FoodProduction 1.3.8 STARTED]");
                   //Ticket instance = (Ticket)Activator.CreateInstance(typeof(Ticket));

                   //Init application
                   lObjApplication.Run();
               }
               catch (Exception ex)
               {
                   System.Windows.Forms.MessageBox.Show(ex.Message);
                   LogService.WriteError("[ERROR]" + ex.Message);
               }
        }

        private static bool ConnectRemoteAccess()
        {
            try
            {
                RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false); //Desconectar al cerrar
                return true;

            }

            catch (Exception lObjException)
            {
                UIApplication.ShowError(string.Format("ItemEventException: {0}", lObjException.Message));
                return false;
            }
        }

        static void GetApplicationAppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
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
