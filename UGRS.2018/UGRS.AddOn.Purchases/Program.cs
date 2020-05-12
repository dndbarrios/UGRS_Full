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
              
                //Initialize Tables 
                UIApplication.ShowSuccess(string.Format("Inicializar las tablas"));
                PurchasesServiceFactory lObjFoodProductionFactory = new PurchasesServiceFactory();
                lObjFoodProductionFactory.GetSetupService().InitializeTables();
                UIApplication.ShowSuccess(string.Format("AddonCompras 1.3.11 iniciado correctamente"));
                LogService.WriteSuccess(string.Format("AddonCompras 1.3.11 iniciado correctamente"));
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

            //string lStrUri = @"C:\Users\amartinez\Documents\Proyectos\UGRS\Compras\XML\XML TODOS";
            //string lStrUri = @"C:\Users\amartinez\Documents\Proyectos\UGRS\Compras\XML\Test";
            string lStrUri = @"C:\Users\amartinez\Documents\Proyectos\UGRS\Compras\XML\XML Productivo";
            DirectoryInfo d = new DirectoryInfo(lStrUri);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.xml"); //Getting Text files
            DateTime mDblTimeGEneral = DateTime.Now;
            int i = 0;
            int Correcto = 0;
            int incorrecto = 0;
            Files = Files.Skip(Math.Max(0, Files.Count() - 2507)).ToArray();
            foreach (FileInfo file in Files)
            {
                try
                {
                    DateTime mDblTime = DateTime.Now;
                    LogService.WriteInfo(i + " de " + Files.Length + " Leyendo archivo " + file.Name);
                    UGRS.AddOn.Purchases.Services.ReadXMLService lObjReadXML = new Services.ReadXMLService();
                    UGRS.Core.SDK.DI.Purchases.DTO.PurchaseXMLDTO lObjPurchaseDTO = lObjReadXML.ReadXML(file.FullName);
                    lObjPurchaseDTO = AddValuesTest(lObjPurchaseDTO);

                    if (lObjPurchaseDTO == null || string.IsNullOrEmpty(lObjPurchaseDTO.Total))
                    {
                        LogService.WriteError("Carga fallida" + file.Name);
                        incorrecto++;
                    }
                    else
                    {
                       // LogService.WriteSuccess("Lectura correcta");
                        float lDblIva = 0;
                        foreach (var item in lObjPurchaseDTO.ConceptLines)
                        {
                            if (item.LstTaxes != null)
                            {
                                lDblIva += item.LstTaxes.Sum(t => float.Parse(t.Amount));
                            }
                        }
		                      //lDblIva = lObjPurchaseDTO.ConceptLines.Sum(z => z.LstTaxes.Where//(t => t != null).Sum(y => Convert.ToDouble(y.Amount)));//Iva
                        
                            //
                        

                        //LogService.WriteInfo(string.Format("UUID XML:| {0}| Total:| {1}| XMLTotal:| {2}|Subtotal:| {3}| Retenciones:| {4}| Ieps:| {5}| Iva:| {6}| RetIva:| {7}| RetIva4:| {8}| TotalImpuestosTraslados| {9}|",
                        //    lObjPurchaseDTO.FolioFiscal //UUID
                        //    , lObjPurchaseDTO.Total //Total
                        //    , lObjPurchaseDTO.XMLTotal // XMLTotal
                        //    , lObjPurchaseDTO.SubTotal //Subtotal
                        //    , lObjPurchaseDTO.WithholdingTax.Sum(x => Convert.ToDouble(x.Amount)) // Retenciones
                        //    , lObjPurchaseDTO.Ieps //Ieps
                        //    , lDblIva//Iva
                        //    , lObjPurchaseDTO.RetIva //RetIVa
                        //    , lObjPurchaseDTO.RetIva4//RetIva4
                        //    , lObjPurchaseDTO.TaxesTransfers /// Total de impuestos trasladados
                        //    ));

                      
                        InvoiceDI lObjInvoiceDI = new InvoiceDI();
                        if (lObjInvoiceDI.CreateDocument(lObjPurchaseDTO, false))
                        {
                            SAPbobsCOM.Documents lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                            lObjDocument.GetByKey(Convert.ToInt32(DIApplication.Company.GetNewObjectKey()));

                            double lFlTotalIVa = 0;
                            double lFlTotalLine = 0;
                            for (int k = 0; k < lObjDocument.Lines.Count; k++)
                            {
                                lObjDocument.Lines.SetCurrentLine(k);
                                lFlTotalIVa += lObjDocument.Lines.TaxTotal;
                                lFlTotalLine += lObjDocument.Lines.LineTotal;
                            }
                            string ss = lObjDocument.GetAsXML();
                            LogService.WriteInfo(string.Format("UUID SAP:| {0}| Total:| {1}| XMLTotal:| {2}|Subtotal:| {3}| XML Subtotal:| {4}| Retenciones:| {5}| XML Retenciones:| {6}| Ieps:| {7}| Iva:| {8}| XML Iva:| {9}| RetIva:| {10}| RetIva4:| {11}| TotalImpuestosTraslados| {12}| TotalImpuestosTraslados XML| {13}| ",
                            lObjDocument.UserFields.Fields.Item("U_UDF_UUID").Value //UUID
                            , lObjDocument.DocTotal //Total
                            , lObjPurchaseDTO.Total // XMLTotal
                            ,  lFlTotalLine //Subtotal
                            , lObjPurchaseDTO.SubTotal
                            , lObjDocument.WithholdingTaxData.WTAmount  // Retenciones
                            , lObjPurchaseDTO.WithholdingTax != null? lObjPurchaseDTO.WithholdingTax.Sum(x => Convert.ToDouble(x.Amount)) : 0
                            , "" //Ieps
                            , lFlTotalIVa //Iva
                            , lDblIva
                            , "" //RetIVa
                            , "" //REtIva4
                            , lObjDocument.VatSum /// Total de impuestos trasladados
                            , lObjPurchaseDTO.TaxesTransfers == null ? "0" : lObjPurchaseDTO.TaxesTransfers                     
                            )); //RetIva4
                            Correcto++;
                            //LogService.WriteSuccess("Factura generada correctamente");
                        }
                        else
                        {
                            incorrecto++;
                        }

                    }

                    LogService.WriteInfo("Correcto: " + Correcto + " Incorrecto: " + incorrecto);
                    TimeSpan lTmsTime = DateTime.Now - mDblTime;
                    LogService.WriteInfo(lTmsTime.Seconds + "." + lTmsTime.Milliseconds);
                    i++;
                }
                catch (Exception ex)
                {
                    incorrecto++;
                    LogService.WriteError(ex.Message);
                }
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
            pObjPurchaseDTO.XMLFile = @"C:\Desarrollo\200007729088.xml";

            
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

            System.Collections.Generic.List<UGRS.Core.SDK.DI.Purchases.DTO.ConceptsXMLDTO> LstIeps = new System.Collections.Generic.List<Core.SDK.DI.Purchases.DTO.ConceptsXMLDTO>();
             PurchasesServiceFactory mObjPurchaseServiceFactory = new PurchasesServiceFactory(); 
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
                lObjItems.Subtotal = (Convert.ToDecimal(lObjItems.UnitPrice) * Convert.ToDecimal(lObjItems.Quantity)).ToString();
                double lDblDesc = 0;
                if (Convert.ToDecimal(lObjItems.Discount) > 0)
                {
                    lDblDesc = Convert.ToDouble(lObjItems.Discount) / Convert.ToDouble(lObjItems.Subtotal);
                    //lDblDesc = Math.Round(100 * lDblDesc) / 100;
                   // lObjItems.lLines.DiscountPercent = lDblDesc * 100;
                }
                if (lObjItems.LstTaxes != null && lObjItems.LstTaxes.Count > 0)
                {
                    if (lObjItems.LstTaxes.Where(x => x.Tax == "002").Count() > 0)
                    {
                        string lStrRate = lObjItems.LstTaxes.Where(x => x.Tax == "002").FirstOrDefault().Rate;
                        lDblRate = string.IsNullOrEmpty(lStrRate) ? 0 : Convert.ToDouble(lStrRate);
                    }
                }
              
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

                if (lObjItems.LstTaxes != null)
                {
                    foreach (var item in lObjItems.LstTaxes.Where(x => x.Tax == "003").ToList())
                    {
                        string lStrItemCode = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetItemIEPS();
                        UGRS.Core.SDK.DI.Purchases.DTO.ConceptsXMLDTO lObjIeps = new Core.SDK.DI.Purchases.DTO.ConceptsXMLDTO();
                        lObjIeps.CodeItmProd = lStrItemCode;
                        lObjIeps.CostingCode = "OG_GRAL";
                        lObjIeps.AdmOper = "A";
                        lObjIeps.AF = "";
                        lObjIeps.Project = "";
                        lObjIeps.AGL = "";
                        lObjIeps.WareHouse = "OFGE";
                        lObjIeps.Quantity = "1";
                        lObjIeps.UnitPrice = item.Amount;
                        LstIeps.Add(lObjIeps);
                    }
                }
            }
            if (pObjPurchaseDTO.LstLocalTax != null)
                {
                    foreach (var item in pObjPurchaseDTO.LstLocalTax)
                    {
                        string lStrItemCode = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetLocalTaxt();
                        if (!string.IsNullOrEmpty(lStrItemCode))
                        {
                            UGRS.Core.SDK.DI.Purchases.DTO.ConceptsXMLDTO lObjIeps = new Core.SDK.DI.Purchases.DTO.ConceptsXMLDTO();
                            lObjIeps.CodeItmProd = lStrItemCode;
                            lObjIeps.CostingCode = "OG_GRAL";
                            lObjIeps.AdmOper = "A";
                            lObjIeps.AF = "";
                            lObjIeps.Project = "";
                            lObjIeps.AGL = "";
                            lObjIeps.WareHouse = "OFGE";
                            lObjIeps.Quantity = "1";
                            lObjIeps.UnitPrice = item.Amount;
                            LstIeps.Add(lObjIeps);
                        }
                    }
                }
            pObjPurchaseDTO.ConceptLines.AddRange(LstIeps);

           
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
