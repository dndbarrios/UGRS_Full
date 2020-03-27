using System;
using System.Collections.Generic;


using SAPbouiCOM.Framework;
using Core.SDK.DI;
using Core.SDK.UI;
using Addon.CFDI_Facturacion.Utils;
using Core.Services;

namespace Addon.CFDI_Facturacion
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

                LogService.FileName("AddOnTimbrado");

                DIApplication.DIConnect((SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany());
                Application.SBO_Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(SBO_Application_AppEvent);
                Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);

                UIApplication.ShowSuccess(string.Format("Addon de [Facturación CFDI] iniciado correctamente."));

                oApp.Run();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                System.Windows.Forms.Application.Exit();
            }
        }

        static void SBO_Application_ItemEvent(string formUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (pVal.ActionSuccess && pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD)
                {
                    SAPbouiCOM.Form lObjForm = Application.SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);

                    if (pVal.FormTypeEx.Equals(Constants.STR_AR_INVOICE_FORM))
                    {
                        new BtnTimbradoControl(lObjForm);
                    }
                }
            }
            catch
            {
                
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
