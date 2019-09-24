using SAPbobsCOM;
using System;
using UGRS.Core.SDK.DI.Purchases.DTO;
using UGRS.Core.SDK.DI.Purchases.Tables;
using UGRS.Core.SDK.UI;
using UGRS.Core.Utility;
using System.Linq;
using System.Collections.Generic;
using UGRS.Core.Services;
using System.IO;
using SAPbouiCOM;

namespace UGRS.Core.SDK.DI.Purchases.Services
{
	public class InvoiceDI
	{
		PurchasesServiceFactory mObjPurchaseServiceFactory = new PurchasesServiceFactory();

		/// <summary>
		/// Crear factura.
		/// </summary>
		public bool CreateDocument(PurchaseXMLDTO pObjPurchase, bool lBolCreatePayment)
		{
			bool lBolIsSuccess = false;
			 string lStrAttachXML = string.Empty;
			 string lStrAttachPDF = string.Empty;

			//SAPbobsCOM.Documents lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
			SAPbobsCOM.Documents lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
			try
			{
				//Draft Type Document
				lObjDocument.DocObjectCode = BoObjectTypes.oPurchaseInvoices;

				/*double ldbl = Convert.ToDouble(pObjPurchase.Total);
				lObjDocument.DocTotal = ldbl;*/
				string lStrCostCenter = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetCostCenter();

				lObjDocument.CardCode = pObjPurchase.CardCode;
				lObjDocument.EDocNum = pObjPurchase.FolioFiscal;
				lObjDocument.TaxDate = pObjPurchase.TaxDate;
				lObjDocument.DocDate = pObjPurchase.DocDate;
				lObjDocument.Comments = pObjPurchase.Obs;
				lObjDocument.NumAtCard = pObjPurchase.ReferenceFolio;
				lObjDocument.DocDueDate = pObjPurchase.TaxDate.AddMonths(1);
				lObjDocument.UserFields.Fields.Item("U_UDF_UUID").Value = pObjPurchase.FolioFiscal;
				//lObjDocument.UserFields.Fields.Item("U_GLO_DocEUG").Value = String.IsNullOrEmpty(pObjPurchase.CodeMov) ? "" : pObjPurchase.CodeMov;
				lObjDocument.UserFields.Fields.Item("U_GLO_ObjTUG").Value = "frmReceipts";
				lObjDocument.UserFields.Fields.Item("U_FolioFiscal").Value = pObjPurchase.FolioFiscal;
                lObjDocument.UserFields.Fields.Item("U_GLO_Memo").Value = pObjPurchase.Obs;
                //lObjDocument.Rounding = BoYesNoEnum.tYES;
                string lStrFile = AttatchFile(pObjPurchase.XMLFile);
                if (!string.IsNullOrEmpty(lStrFile))
                {
                    lObjDocument.UserFields.Fields.Item("U_ArchivoXML").Value = AttatchFile(pObjPurchase.XMLFile);
                    lObjDocument.UserFields.Fields.Item("U_ArchivoPDF").Value = AttatchFile(pObjPurchase.PDFFile);
                }
                else
                {
                    return false;
                }
			   
				lObjDocument.UserFields.Fields.Item("U_MQ_Rise").Value = pObjPurchase.MQRise;
				// lObjDocument.WithholdingTaxData

				//adding reference
				lObjDocument.UserFields.Fields.Item("U_MQ_OrigenFol").Value = pObjPurchase.Folio;/* (String.IsNullOrEmpty(pObjPurchase.CodeMov)) ?
					String.Format("{0}_{1}_{2}", pObjPurchase.Type, pObjPurchase.Area, pObjPurchase.Folio) :
					String.Format(pObjPurchase.Type +"_"+ pObjPurchase.CodeMov);*/


				lObjDocument.UserFields.Fields.Item("U_MQ_OrigenFol_Det").Value = pObjPurchase.RowLine;
				   
				bool lbolWithholdingTax = true;

				/*Base = y.First().Base,
					Tax = y.First().Tax,
					TypeFactor = y.First().TypeFactor,
					Rate = y.First().Rate,
					Amount = y.Sum(c => float.Parse(c.Amount)).ToString()*/
                if (pObjPurchase.WithholdingTax != null)
                {
                    pObjPurchase.WithholdingTax = pObjPurchase.WithholdingTax.GroupBy(x => x.Rate).Select(y => new TaxesXMLDTO
                     {
                         Base = y.First().Base,
                         Tax = y.First().Tax,
                         TypeFactor = y.First().TypeFactor,
                         Rate = y.First().Rate,
                         Amount = y.Sum(c => float.Parse(c.Amount)).ToString()

                     }).ToList();



                    foreach (TaxesXMLDTO lObjTax in pObjPurchase.WithholdingTax)
                    {
                        lObjDocument.WithholdingTaxData.WTCode = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetWithholdingTaxCodeBP(Convert.ToDouble(lObjTax.Rate) * 100, lObjDocument.CardCode);
                        if (!string.IsNullOrEmpty(lObjDocument.WithholdingTaxData.WTCode))
                        {
                            lObjDocument.WithholdingTaxData.WTAmount = Convert.ToDouble(lObjTax.Amount);
                            lObjDocument.WithholdingTaxData.Add();
                        }
                        else
                        {
                            lbolWithholdingTax = false;
                        }
                    }
                }
               

				foreach (ConceptsXMLDTO lObjConcept in pObjPurchase.ConceptLines)
				{
					lObjDocument.Lines.ItemCode = lObjConcept.CodeItmProd;
					lObjDocument.Lines.UnitPrice = Convert.ToDouble(lObjConcept.UnitPrice);
					lObjDocument.Lines.Quantity = Convert.ToDouble(lObjConcept.Quantity);
					lObjDocument.Lines.COGSCostingCode = lObjConcept.CostingCode;
					if (lObjConcept.AdmOper == "O")
					{
						lObjDocument.Lines.AccountCode = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetCostAccount(lObjConcept.CodeItmProd);
					}
					
					lObjDocument.Lines.TaxType = lObjConcept.HasTax ? BoTaxTypes.tt_Yes : BoTaxTypes.tt_No;  //= BoTaxTypes.tt_No;
					lObjDocument.Lines.WTLiable = lObjConcept.HasWht ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
					lObjDocument.Lines.UserFields.Fields.Item("U_CO_AdmOper").Value = lObjConcept.AdmOper;
					
					lObjDocument.Lines.COGSCostingCode2 = lObjConcept.AF;
					lObjDocument.Lines.ProjectCode = lObjConcept.Project;
					lObjDocument.Lines.CostingCode3 = lObjConcept.AGL;
				   
					//lObjDocument.Lines.
					lObjDocument.Lines.TaxCode = lObjConcept.TaxCode;
					//lObjDocument.Lines.TaxTotal = lObjConcept.LstTaxes;
					if (!string.IsNullOrEmpty(pObjPurchase.MQRise))
					{
						string lStrWhsMQ = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetMQWhs(lObjConcept.CodeItmProd);
						if (!string.IsNullOrEmpty(lStrWhsMQ))
						{
							lObjConcept.WareHouse = lStrWhsMQ;
						}
					}
					lObjDocument.Lines.WarehouseCode = lObjConcept.WareHouse; //mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetWhouse(lObjConcept.CostingCode);

					double lDblDesc = 0;
					if (Convert.ToDecimal(lObjConcept.Discount) > 0)
					{
						lDblDesc = Convert.ToDouble(lObjConcept.Discount) / Convert.ToDouble(lObjConcept.Subtotal);
						//lDblDesc = Math.Round(100 * lDblDesc) / 100;
						lObjDocument.Lines.DiscountPercent = lDblDesc*100;
					}


                    //double lDblUnitPrice = Convert.ToDouble(lObjConcept.UnitPrice);
                    //lDblUnitPrice = Math.Round(100 * lDblUnitPrice) / 100;

                    //double lDblQuantity = Convert.ToDouble(lObjConcept.Quantity);
                    //double lDblAmountXML = Convert.ToDouble(lObjConcept.Subtotal);

                    //double lDblAmount = lDblUnitPrice * lDblQuantity;

                    //lDblAmount = Math.Round(100 * lDblAmount) / 100;
                    //lDblAmountXML = Math.Round(100 * lDblAmountXML) / 100;


                    //if (lDblAmount != lDblAmountXML)
                    //{
                    //    double lDblnewDesc = 0;
                    //    lDblnewDesc = (lDblAmount / lDblAmountXML) - 1;
                    //    lObjDocument.Lines.DiscountPercent += lDblnewDesc * 100;

                    //}
					//lObjDocument.Lines.LineTotal = Convert.ToDouble(lObjConcept.Amount);
                  
                    
					lObjDocument.Lines.Add();

					
					// lObjDocument.Lines.
				}
                lObjDocument.Rounding = BoYesNoEnum.tYES;
              //  lObjDocument.RoundingDiffAmount = -0.01;
                
                //lObjDocument.DocTotal = Convert.ToDouble(pObjPurchase.Total);
               // var sfesf = lObjDocument.TotalDiscount;


                if (lbolWithholdingTax)
                {

                    if (lObjDocument.Add() != 0)
                    {
                        string lStrError = DIApplication.Company.GetLastErrorDescription();

                        UIApplication.ShowMessageBox(string.Format("Exception: {0}", DIApplication.Company.GetLastErrorDescription()));
                        LogService.WriteError("InvoiceDI (CreateDocument) " + DIApplication.Company.GetLastErrorDescription());
                    }
                    else
                    {
                        pObjPurchase.DocEntry = Convert.ToInt32(DIApplication.Company.GetNewObjectKey().ToString());
                        pObjPurchase.IsDraft = false;
                        lBolIsSuccess = true;

                      
                        //for (int i = 0; i < lObjDocInvoice.Lines.Count; i++)
                        //{
                        //    sss += lObjDocInvoice.Lines.TaxTotal;

                        //}


                        //pObjPurchase.Total = lObjDocInvoice.DocTotal.ToString();
                        //LogService.WriteInfo(string.Format("UUID: {0} Total: {1}  Retenciones: {2} Impuesto: {3} ",
                        //    lObjDocInvoice.UserFields.Fields.Item("U_UDF_UUID").Value //UUID
                        //    , lObjDocInvoice.DocTotal //TOTAL
                        //    , lObjDocInvoice.WithholdingTaxData.WTAmount //Retenciones
                        //    , sss //IVA
                        //    ));
                        //  , lObjDocInvoice.Lines.TaxTotal.total lOBj.Ieps, lOBj.Iva, lOBj.RetIva, lOBj.RetIva4));

                        //PaymentDI lObjPaymentDI = new PaymentDI();


                        if (lBolIsSuccess)
                        {
                            LogService.WriteSuccess("Documento realizado correctamente: " );
                        }
                    }
                }
			}
			catch (Exception ex)
			{
				lBolIsSuccess = false;
				UIApplication.ShowMessageBox(string.Format(ex.Message + "\n" + "Exception: {0}", DIApplication.Company.GetLastErrorDescription()));
				LogService.WriteError("InvoiceDI (CreateDocument) " + DIApplication.Company.GetLastErrorDescription());
				LogService.WriteError("InvoiceDI (CreateDocument) " + ex.Message);
				LogService.WriteError(ex);
			}
			finally
			{
				MemoryUtility.ReleaseComObject(lObjDocument);
			}
			return lBolIsSuccess;
		}

       


		private string AttatchFile(string pStrFile)
		{
			int lIntAttachement = 0;
			string lStrAttach = string.Empty;
			string lStrAttachPath = mObjPurchaseServiceFactory.GetPurchaseService().GetAttachPath();
            if (!string.IsNullOrEmpty(pStrFile))
            {

                if (!Directory.Exists(lStrAttachPath))
                {
                    if (SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(string.Format("Carpeta {0} \n no accesible es posible que no pueda adjuntar el xml ¿Desea continuar?", lStrAttachPath), 2, "Si", "No", "") == 2)
                    {
                        return "";
                    }
                }

            }
            AttachmentDI lObjAttachmentDI = new AttachmentDI();
            lIntAttachement = lObjAttachmentDI.AttachFile(pStrFile);
            if (lIntAttachement > 0)
            {
                lStrAttach = lStrAttachPath + System.IO.Path.GetFileName(pStrFile);
            }
            else
            {
                LogService.WriteError("InvoiceDI (AttachDocument) " + DIApplication.Company.GetLastErrorDescription());
                UIApplication.ShowError(string.Format("InvoiceDI (AttachDocument) : {0}", DIApplication.Company.GetLastErrorDescription()));
            }
			return lStrAttach;
		}

	
	   
		

		/// <summary>
		/// cancela un documento por DocEntry
		/// </summary>
		public bool CancelDocument(string pStrDocEntry)
		{
			try
			{
				SAPbobsCOM.Documents lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
				lObjDocument.GetByKey(Convert.ToInt32(pStrDocEntry));
				var lObjCancelDoc = lObjDocument.CreateCancellationDocument();
				lObjCancelDoc.DocDate = DateTime.Now;

				if (lObjCancelDoc.Add() != 0)
				{
					UIApplication.ShowMessageBox(DIApplication.Company.GetLastErrorDescription());
					LogService.WriteError("InvoiceDI (AddVoucherDetail) DocEntry:"+ pStrDocEntry+ " Mensaje:" + DIApplication.Company.GetLastErrorDescription());
					return false;
				}
				else
				{
				
					LogService.WriteSuccess("Documento cancelado correctamente: " + pStrDocEntry);
					return true;
				}
			}
			catch (Exception ex)
			{
				LogService.WriteError("InvoiceDI (CancelDocument) " + ex.Message);
				LogService.WriteError(ex);
				return false;
			}
		}

		/// <summary>
		/// Actualiza  estatus
		/// </summary>
		public void UpdateStatus(string pStrCode, string pStrDocEntry, string pStrCodeVoucher)
		{
			try
			{
				VouchersDetail lObjVoucher = mObjPurchaseServiceFactory.GetPurchaseVouchersService().GetVouchesDetail(pStrCode).Where(x => x.DocEntry == pStrDocEntry).First();
				if (!string.IsNullOrEmpty(lObjVoucher.DocEntry))
				{
					string lStrDocStatus = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetDocCanceled(pStrDocEntry, lObjVoucher.Type);
					if (!lStrDocStatus.Equals("Cancelado"))
					{
						lStrDocStatus = mObjPurchaseServiceFactory.GetPurchaseInvoiceService().GetDocStatus(pStrDocEntry);
					}


					lObjVoucher.Status = lStrDocStatus;
					if (mObjPurchaseServiceFactory.GetVouchersDetailService().Update(lObjVoucher) != 0)
					{
						UIApplication.ShowMessageBox(string.Format("Exception: {0}", DIApplication.Company.GetLastErrorDescription()));
						LogService.WriteError("InvoiceDI (AddVoucherDetail) " + DIApplication.Company.GetLastErrorDescription());
					}
					else
					{
						mObjPurchaseServiceFactory.GetVouchersService().UpdateTotal(pStrCodeVoucher);
					}
				}

			}
			catch (Exception ex)
			{
				LogService.WriteError("InvoiceDI (UpdateStatus) " + ex.Message);
				LogService.WriteError(ex);
			}
		}

	}
}
