using System;
using SAPbouiCOM.Framework;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.UI;
using UGRS.Core.SDK.DI.Purchases;
using UGRS.Core.Services;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using servTim = UGRS.AddOn.Purchases.TimbradoSoap33Prodigia;
using UGRS.Core.SDK.DI.Purchases.Services;

namespace UGRS.AddOn.Purchases
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
                LogService.Filename("AddOnCompras");
               
                Menu MyMenu = new Menu();
                MyMenu.AddMenuItems();
                oApp.RegisterMenuEventHandler(MyMenu.SBO_Application_MenuEvent);
                DIApplication.DIConnect((SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany());
                Application.SBO_Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(SBO_Application_AppEvent);
                //TestXML();
               // TestValidateXML();
                //Initialize Tables 
                UIApplication.ShowSuccess(string.Format("Inicializar las tablas"));
                PurchasesServiceFactory lObjFoodProductionFactory = new PurchasesServiceFactory();
                lObjFoodProductionFactory.GetSetupService().InitializeTables();
               UIApplication.ShowSuccess(string.Format("AddonCompras 1.2.70 iniciado correctamente"));
                LogService.WriteSuccess(string.Format("AddonCompras 1.2.70 iniciado correctamente"));
                oApp.Run();
              
               
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                System.Windows.Forms.Application.Exit();
            }
        }

        static void TestXML()
        {

            string lStrUri = @"C:\Users\ssandoval\Desktop\Qualisys Saul\PROJECTS\Union Ganadera\Compras\XML Enero- Feb";
            DirectoryInfo d = new DirectoryInfo(lStrUri);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.xml"); //Getting Text files
            DateTime mDblTimeGEneral = DateTime.Now;
            int i = 0;
            int Correcto = 0;
            int incorrecto = 0;
            foreach (FileInfo file in Files.Skip(1).Take(1))
            {
                DateTime mDblTime = DateTime.Now;
                LogService.WriteInfo (i + " de " + Files.Length + "Leyendo archivo" + file.Name);
                UGRS.AddOn.Purchases.Services.ReadXMLService lObjReadXML = new Services.ReadXMLService();
                UGRS.Core.SDK.DI.Purchases.DTO.PurchaseXMLDTO lOBj =  lObjReadXML.ReadXML(file.FullName);

                if (lOBj == null || string.IsNullOrEmpty(lOBj.Total))
                {
                    LogService.WriteError("Carga fallida" + file.Name);
                    incorrecto++;
                }
                else
                {
                    LogService.WriteSuccess("Lectura correcta");
                    Correcto++;
                    servTim.PadeTimbradoServiceClient mObjTimbradorp = new  servTim.PadeTimbradoServiceClient();
                    string lStrCFDI = mObjTimbradorp.cfdiPorUUID("1d3027c6-5c49-11e3-a2a4-109add4fad20", "factugrs", "A123456789$", "9E5B72AA-11F4-4E2E-BAF2-E4D83465784B");

                }

                LogService.WriteInfo("Correcto: " + Correcto + " Incorrecto: " + incorrecto);
                TimeSpan lTmsTime = DateTime.Now - mDblTime;
                LogService.WriteInfo(lTmsTime.Seconds + "." + lTmsTime.Milliseconds);
                i++;
            }

            TimeSpan lTmsTimeGral = DateTime.Now - mDblTimeGEneral;
            LogService.WriteInfo(lTmsTimeGral.Seconds + "." + lTmsTimeGral.Milliseconds);
        }

      

        static void TestValidateXML()
        {

            string lStrUri = @"C:\Users\amartinez\Documents\Proyectos\UGRS\Compras\XML\XML TODOS";
            DirectoryInfo d = new DirectoryInfo(lStrUri);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.xml"); //Getting Text files
            DateTime mDblTimeGEneral = DateTime.Now;
            int i = 0;
            int Correcto = 0;
            int incorrecto = 0;
            foreach (FileInfo file in Files.Take(5))
            {
                DateTime mDblTime = DateTime.Now;
                LogService.WriteInfo(i + " de " + Files.Length + "Leyendo archivo" + file.Name);
                UGRS.AddOn.Purchases.Services.ReadXMLService lObjReadXML = new Services.ReadXMLService();
                UGRS.Core.SDK.DI.Purchases.DTO.PurchaseXMLDTO lOBj = lObjReadXML.ReadXML(file.FullName);
                lOBj = AddValuesTest(lOBj);

                if (lOBj == null || string.IsNullOrEmpty(lOBj.Total))
                {
                    LogService.WriteError("Carga fallida" + file.Name);
                    incorrecto++;
                }
                else
                {
                    LogService.WriteSuccess("Lectura correcta");
                    LogService.WriteInfo(string.Format("UUID: {0} Total: {1} XMLTotal: {2} Subtotal: {3} Retenciones: {4} Ieps: {5} Iva: {6} RetIva: {7} RetIva4: {8}",
                        lOBj.FolioFiscal, lOBj.Total, lOBj.XMLTotal, lOBj.SubTotal , lOBj.WithholdingTax.Sum(x => Convert.ToDouble(x.Amount)) , lOBj.Ieps, lOBj.Iva, lOBj.RetIva , lOBj.RetIva4));
         
                    Correcto++;
                    InvoiceDI lObjInvoiceDI = new InvoiceDI();
                    if (lObjInvoiceDI.CreateDocument(lOBj, false))
                    {
                        LogService.WriteSuccess("Factura generada correctamente");
                    }
                }

                LogService.WriteInfo("Correcto: " + Correcto + " Incorrecto: " + incorrecto);
                TimeSpan lTmsTime = DateTime.Now - mDblTime;
                LogService.WriteInfo(lTmsTime.Seconds + "." + lTmsTime.Milliseconds);
                i++;
            }

            TimeSpan lTmsTimeGral = DateTime.Now - mDblTimeGEneral;
            LogService.WriteInfo(lTmsTimeGral.Seconds + "." + lTmsTimeGral.Milliseconds);
        }

        static UGRS.Core.SDK.DI.Purchases.DTO.PurchaseXMLDTO AddValuesTest(UGRS.Core.SDK.DI.Purchases.DTO.PurchaseXMLDTO pObjPurchaseDTO)
        {
            pObjPurchaseDTO.CardCode = "PR00000001";
            pObjPurchaseDTO.TaxDate = DateTime.Now;
            pObjPurchaseDTO.DocDate = DateTime.Now;
            pObjPurchaseDTO.Obs = "Obs";
            pObjPurchaseDTO.MQRise = "";
            pObjPurchaseDTO.Folio = "Test";
            pObjPurchaseDTO.RowLine = "";

           

            //foreach (System.Reflection.PropertyInfo pinfo in pObjPurchaseDTO.GetType().GetProperties())
            //{
            //    string pStr = pinfo.Name;
            //    object value = pinfo.GetValue(pObjPurchaseDTO, null);

            //    if (value == null)
            //    {
            //        value = "null";
                 
            //    }  
            //    LogService.WriteInfo(pStr + " " +  value.ToString());
            //}


            foreach (var lObjItems in pObjPurchaseDTO.ConceptLines)
            {
                lObjItems.CodeItmProd = "A00000209";
                lObjItems.CostingCode = "OG_GRAL";
                lObjItems.AdmOper = "A";
                lObjItems.AF = "";
                lObjItems.Project = "";
                lObjItems.AGL = "";
                lObjItems.WareHouse = "OFGE";
               // lObjItems.
                double lDblRate = 0;
                if (lObjItems.LstTaxes != null && lObjItems.LstTaxes.Count > 0)
                {
                    if (lObjItems.LstTaxes.Where(x => x.Tax == "002").Count() > 0)
                    {
                        string lStrRate = lObjItems.LstTaxes.Where(x => x.Tax == "002").FirstOrDefault().Rate;
                        lDblRate = string.IsNullOrEmpty(lStrRate) ? 0 : Convert.ToDouble(lStrRate);
                    }
                }
                PurchasesServiceFactory mObjPurchaseServiceFactory = new PurchasesServiceFactory();
                string lStrTaxCode = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetTaxCode((lDblRate * 100).ToString()); //
                lObjItems.TaxCode = lStrTaxCode;
                //foreach (System.Reflection.PropertyInfo pinfo in lObjItems.GetType().GetProperties())
                //{
                //    string pStr = pinfo.Name;
                //    object value = pinfo.GetValue(lObjItems, null);

                //    if (value == null)
                //    {
                //        value = "null";

                //    }
                //    LogService.WriteInfo(pStr + " " + value.ToString());
                //}
            }

            return pObjPurchaseDTO;
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
