﻿using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using UGRS.AddOn.FoodProduction.DTO;
using UGRS.AddOn.FoodProduction.Services;
using UGRS.Core.SDK.DI.FoodProduction.Tables;
using UGRS.Core.Services;

namespace UGRS.AddOn.FoodProduction.UI.Matriz
{
    
    public class Calculations
    {
        Validations mObjValidations = new Validations();
        TicketServices mObjTicketServices = new TicketServices();

        public TotalsDTO CalcTotals(SAPbouiCOM.IMatrix pObjMatrix, double pDblInput, double pDblOutput)
        {
           TotalsDTO lObjTotalsDTO = new TotalsDTO();
           try
           {
               int sd = pObjMatrix.Columns.Item(1).Cells.Count;
               for (int i = 1; i <= pObjMatrix.RowCount; i++)
               {
                   string ss = ((SAPbouiCOM.EditText)pObjMatrix.Columns.Item("Importe").Cells.Item(i).Specific).Value;
                   lObjTotalsDTO.Amount += Convert.ToDouble(((SAPbouiCOM.EditText)pObjMatrix.Columns.Item("Importe").Cells.Item(i).Specific).Value, CultureInfo.GetCultureInfo("en-US"));
                   //lObjTotalsDTO.Bags += Convert.ToDouble(((SAPbouiCOM.EditText)pObjMatrix.Columns.Item("Sacos").Cells.Item(i).Specific).Value);

                   if ((pObjMatrix.Columns.Item("Check").Cells.Item(i).Specific as CheckBox).Checked)
                   {
                       lObjTotalsDTO.Tara += Convert.ToDouble(((SAPbouiCOM.EditText)pObjMatrix.Columns.Item("PesoN").Cells.Item(i).Specific).Value, CultureInfo.GetCultureInfo("en-US"));
                   }
                   else
                   {
                       lObjTotalsDTO.WeightB += Convert.ToDouble(((SAPbouiCOM.EditText)pObjMatrix.Columns.Item("PesoN").Cells.Item(i).Specific).Value, CultureInfo.GetCultureInfo("en-US"));
                   }

               }
               //txtVari.Value =  (txtInWT.Value - tOutputWT.Value) - lDblVariacion).ToString();
               if (lObjTotalsDTO.WeightB < 0)
               {
                   lObjTotalsDTO.WeightB = lObjTotalsDTO.WeightB * -1;
               }
               lObjTotalsDTO.WeightNet = lObjTotalsDTO.WeightB + lObjTotalsDTO.Tara;
               lObjTotalsDTO.OutputW = pDblOutput;
               lObjTotalsDTO.InputW = pDblInput;

              lObjTotalsDTO.WeightTotal = lObjTotalsDTO.InputW - lObjTotalsDTO.OutputW;  
               if (lObjTotalsDTO.WeightTotal < 0)
               {
                   lObjTotalsDTO.WeightTotal = lObjTotalsDTO.WeightTotal * -1;
               }

               lObjTotalsDTO.Variation = Convert.ToDouble(Convert.ToDecimal(lObjTotalsDTO.WeightTotal, CultureInfo.GetCultureInfo("en-US")) - Convert.ToDecimal(lObjTotalsDTO.WeightNet, CultureInfo.GetCultureInfo("en-US")), CultureInfo.GetCultureInfo("en-US"));
              // lObjTotalsDTO.Variation =lObjTotalsDTO.WeightTotal - lObjTotalsDTO.WeightNet;
           }

           catch (Exception ex)
           {
               LogService.WriteError("[CalcTotals]: " + ex.Message);
               LogService.WriteError(ex);
               UIApplication.ShowMessageBox("Error al calcular totales" + ex.Message);
           }
            return lObjTotalsDTO;
        }

        public SAPbouiCOM.DBDataSource CalcImport(string pStrTypeTicket, SAPbouiCOM.IMatrix pObjMatrix, int pIntRow, SAPbouiCOM.DBDataSource pDBDataSourceD)
        {
            try
            {
                double lDblPesoNeto = 0;
                double lDblPeso1 = 0;
                double lDblPeso2 = 0;
                double lDblImporte = 0;
                double lDblPrice = 0;
                string lStrSacos = "";

                if (pIntRow <= pObjMatrix.RowCount && !string.IsNullOrEmpty((pObjMatrix.Columns.Item("Peso2").Cells.Item(pIntRow).Specific as EditText).Value.Trim()))
                {

                    double ddd = Convert.ToDouble((pObjMatrix.Columns.Item("Peso1").Cells.Item(pIntRow).Specific as EditText).Value.Trim());
                    double eee = Convert.ToDouble((pObjMatrix.Columns.Item("Peso1").Cells.Item(pIntRow).Specific as EditText).Value.Trim(), CultureInfo.GetCultureInfo("en-US"));
                    if (ddd != eee )
                    {
                        LogService.WriteError(string.Format("Conversion incorrecta Valor: {0} Conversion: {1} ", eee, ddd));
                        double price1 = Convert.ToDouble((pObjMatrix.Columns.Item("Price").Cells.Item(pIntRow).Specific as EditText).Value.Trim());
                        double price2 = Convert.ToDouble((pObjMatrix.Columns.Item("Price").Cells.Item(pIntRow).Specific as EditText).Value.Trim(), CultureInfo.GetCultureInfo("en-US"));
                        LogService.WriteError(string.Format("Conversion incorrecta Valor: {0} Conversion: {1} ", price1, price2));
                        
                    }
                    lDblPeso1 = Convert.ToDouble((pObjMatrix.Columns.Item("Peso1").Cells.Item(pIntRow).Specific as EditText).Value.Trim(), CultureInfo.GetCultureInfo("en-US"));  //itemcode = VALUE OF CELL. COLUMN "1": ITEMCODE COLUMN. CURRENT ROW: pVal.Row
                    LogService.WriteInfo("Peso1 " + lDblPeso1.ToString());
                    lDblPeso2 = Convert.ToDouble((pObjMatrix.Columns.Item("Peso2").Cells.Item(pIntRow).Specific as EditText).Value.Trim(), CultureInfo.GetCultureInfo("en-US"));
                    lDblPrice = Convert.ToDouble((pObjMatrix.Columns.Item("Price").Cells.Item(pIntRow).Specific as EditText).Value.Trim(), CultureInfo.GetCultureInfo("en-US"));
                    if (pStrTypeTicket != "Venta de pesaje" && pStrTypeTicket != "Pesaje")
                    {
                        lStrSacos = ((SAPbouiCOM.EditText)pObjMatrix.Columns.Item("Sacos").Cells.Item(pIntRow).Specific).Value;
                    }
                    //  lDblPesoNeto = Convert.ToDouble((mObjMatrix.Columns.Item("PesoN").Cells.Item(mIntRow).Specific as EditText).Value.Trim());
                   

                    if (!((SAPbouiCOM.CheckBox)pObjMatrix.Columns.Item("Check").Cells.Item(pIntRow).Specific).Checked)
                    {
                        pDBDataSourceD.SetValue("TreeType", pIntRow - 1, "N");
                        if (lDblPeso1 != 0 && lDblPeso2 != 0 && lDblPesoNeto == 0)
                        {
                            lDblPesoNeto = lDblPeso2 - lDblPeso1;
                        }
                    }
                    else
                    {
                        pDBDataSourceD.SetValue("TreeType", pIntRow - 1, "Y");
                        lDblPesoNeto = Convert.ToDouble((pObjMatrix.Columns.Item("PesoN").Cells.Item(pIntRow).Specific as EditText).Value.Trim(), CultureInfo.GetCultureInfo("en-US"));
                    }
                    LogService.WriteInfo("Peso2 " + lDblPeso1.ToString());

                    lDblPrice = Convert.ToDouble((pObjMatrix.Columns.Item("Price").Cells.Item(pIntRow).Specific as EditText).Value.Trim(), CultureInfo.GetCultureInfo("en-US"));

                    LogService.WriteInfo("Peso3" + lDblPeso1.ToString());
                    if (mObjValidations.ValidateWeight(pStrTypeTicket, lDblPesoNeto, lDblPeso2, pObjMatrix, pIntRow))
                    {
                        LogService.WriteInfo("Peso4 "  + lDblPeso1.ToString());
                        //Valida el tipo de peso
                        if (lDblPesoNeto < 0)
                        {
                            lDblPesoNeto = lDblPesoNeto * -1;
                        }

                        if (!((SAPbouiCOM.CheckBox)pObjMatrix.Columns.Item("Check").Cells.Item(pIntRow).Specific).Checked)
                        {
                            (pObjMatrix.Columns.Item("PesoN").Cells.Item(pIntRow).Specific as EditText).Value = lDblPesoNeto.ToString();
                        }
                        //(pObjMatrix.Columns.Item("Price").Cells.Item(pIntRow).Specific as EditText).Value = lDblPrice.ToString("C");
                        pDBDataSourceD.SetValue("Quantity", pIntRow - 1, lDblPesoNeto.ToString(CultureInfo.GetCultureInfo("en-US")));
                        pDBDataSourceD.SetValue("Price", pIntRow - 1, lDblPrice.ToString(CultureInfo.GetCultureInfo("en-US")));
                        LogService.WriteInfo("Price  " + lDblPrice.ToString(CultureInfo.GetCultureInfo("en-US")));
                        string ss = lDblPrice.ToString(CultureInfo.GetCultureInfo("en-US"));
                        string sssfef = pDBDataSourceD.GetValue("Price", pIntRow - 1);
                      //  (pObjMatrix.Columns.Item("Peso1").Cells.Item(pIntRow).Specific as EditText).Value = lDblPeso1.ToString();
                      //  (pObjMatrix.Columns.Item("Peso2").Cells.Item(pIntRow).Specific as EditText).Value = lDblPeso2.ToString();
                        LogService.WriteInfo("Peso5  " + lDblPeso1.ToString());
                        pDBDataSourceD.SetValue("Weight1", pIntRow - 1, lDblPeso1.ToString(CultureInfo.GetCultureInfo("en-US")));
                        pDBDataSourceD.SetValue("Weight2", pIntRow - 1, lDblPeso2.ToString(CultureInfo.GetCultureInfo("en-US")));

                        pDBDataSourceD.SetValue("U_GLO_BagsBales", pIntRow - 1, lStrSacos);

                       
                        //mObjMatrix.Item.Visible = false;
                        //mObjMatrix.Item.Visible = true;
                        //mObjMatrix.LoadFromDataSource();

                        if (pStrTypeTicket == "Venta de pesaje" || pStrTypeTicket == "Pesaje")
                        {
                            
                            lDblImporte = lDblPrice;
                        }
                        else
                        {
                            lDblImporte = lDblPesoNeto * lDblPrice;
                        }

                        //if (lDblImporte < 0)
                        //{
                        //    lDblImporte = lDblImporte * -1;
                        //}

                        (pObjMatrix.Columns.Item("Importe").Cells.Item(pIntRow).Specific as EditText).Value = lDblImporte.ToString(CultureInfo.GetCultureInfo("en-US"));
                        pDBDataSourceD.SetValue("LineTotal", pIntRow - 1, lDblImporte.ToString(CultureInfo.GetCultureInfo("en-US")));
                        
                        //pObjMatrix.Item.Update();
                        pObjMatrix.LoadFromDataSource();
                        pObjMatrix.SelectRow(pIntRow, true, false);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError("[CalcImport]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox("Error al calcular Importe " + ex.Message);
            }
            return pDBDataSourceD;
        }

        public bool VerifyWeightSecuence(string pStrWeight, string pStrSource,  SAPbouiCOM.IMatrix pObjMatrix, string pStrTypeTicket )
        {
            double lFloWeight = 0;
            try
            {
                if (!string.IsNullOrEmpty(pStrWeight))
                {
                    lFloWeight = Convert.ToDouble(pStrWeight, CultureInfo.GetCultureInfo("en-US"));
                }

            }
            catch (Exception ex)
            {
                LogService.WriteError("[VerifyWeightSecuence]: " + ex.Message);
                LogService.WriteError(ex);
                UIApplication.ShowMessage("Error al capturar datos" + ex.Message);
            }
           

            if ((pStrSource == "RDR1" || pStrTypeTicket == "Traslado - Salida") && pStrTypeTicket != "Venta de pesaje")
            {
                if (lFloWeight > getLargeNumber(pObjMatrix))
                {
                    return true;
                }
            }
            else
            {
                if ((pStrSource == "POR1" || pStrTypeTicket == "Traslado - Entrada") && pStrTypeTicket != "Venta de pesaje")
                {
                    if (lFloWeight == 0)
                    {
                        return true;
                    }
                    else
                    {
                        double lFloSmall = getSmallerNumber(pObjMatrix);
                        if (lFloSmall == 0 || lFloWeight < lFloSmall)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }

            if (pStrTypeTicket == "Pesaje")
            {
                return true;
            }

            UIApplication.ShowError("Favor verifique la secuencia del pesaje");
            return false;
        }

        public double getLargeNumber(SAPbouiCOM.IMatrix pObjMatrix)
        {
            double lFloLargeNum = 0;
            double lFloPeso1 = 0;
            double lFloPeso2 = 0;
            string lStrPeso1;
            string lStrPeso2;
            for (int i = 1; i <= pObjMatrix.RowCount; i++)
            {
                lStrPeso1 = ((SAPbouiCOM.EditText)pObjMatrix.Columns.Item("Peso1").Cells.Item(i).Specific).Value;
                lStrPeso2 = ((SAPbouiCOM.EditText)pObjMatrix.Columns.Item("Peso2").Cells.Item(i).Specific).Value;
                if (!string.IsNullOrEmpty(lStrPeso1))
                {
                    lFloPeso1 = double.Parse(lStrPeso1, CultureInfo.GetCultureInfo("en-US"));
                }
                if (lFloPeso1 > lFloLargeNum)
                {
                    lFloLargeNum = lFloPeso1;
                }

                if (!string.IsNullOrEmpty(lStrPeso2))
                {
                    lFloPeso2 = double.Parse(lStrPeso2, CultureInfo.GetCultureInfo("en-US"));
                }

                if (lFloPeso2 > lFloLargeNum)
                {
                    lFloLargeNum = lFloPeso2;
                }
            }
            return lFloLargeNum;
        }

        public double getSmallerNumber(SAPbouiCOM.IMatrix pObjMatrix)
        {
            try
            {
                double lFloLargeNum = 0;
                double lFloPeso1 = 0;
                double lFloPeso2 = 0;
                string lStrPeso1;
                string lStrPeso2;
                for (int i = 1; i <= pObjMatrix.RowCount; i++)
                {
                    lStrPeso1 = ((SAPbouiCOM.EditText)pObjMatrix.Columns.Item("Peso1").Cells.Item(i).Specific).Value;
                    lStrPeso2 = ((SAPbouiCOM.EditText)pObjMatrix.Columns.Item("Peso2").Cells.Item(i).Specific).Value;

                    if (!string.IsNullOrEmpty(lStrPeso1))
                    {
                        lFloPeso1 = double.Parse(lStrPeso1, CultureInfo.GetCultureInfo("en-US"));
                    }
                    if (lFloLargeNum == 0)
                    {
                        lFloLargeNum = lFloPeso1;
                    }
                    if (lFloPeso1 > 0 && lFloPeso1 < lFloLargeNum)
                    {
                        lFloLargeNum = lFloPeso1;
                    }
                    if (!string.IsNullOrEmpty(lStrPeso2))
                    {
                        lFloPeso2 = double.Parse(lStrPeso2, CultureInfo.GetCultureInfo("en-US"));
                    }

                    if (lFloPeso2 > 0 && lFloPeso2 < lFloLargeNum)
                    {
                        lFloLargeNum = lFloPeso2;
                    }

                }
                return lFloLargeNum;
            }
            catch (Exception ex)
            {
                LogService.WriteError("[getSmallerNumber]: " + ex.Message);
                LogService.WriteError(ex);
                return 0;
            }
        }

        /// <summary>
        /// Obtiene los datos de ticket del formuario por linea
        /// </summary>
        public List<TicketDetail> GetTicketDetailMatrix(string pStrFolio, SAPbouiCOM.IMatrix mObjMatrix, bool mBolIsUpdate, bool pBolPesaje, DBDataSource pOBjDataSource, bool pBolIsSimple)
        {
            List<TicketDetail> lLstTicketDetail = new List<TicketDetail>();
            List<string> lLstDateTime = mObjTicketServices.GetServerDatetime();
            try
            {


                for (int i = 1; i <= mObjMatrix.RowCount; i++)
                {
                    double lFloFirstWt = Convert.ToDouble(((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Peso1").Cells.Item(i).Specific).Value, CultureInfo.GetCultureInfo("en-US"));
                    double lFloSecondWT = Convert.ToDouble(((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Peso2").Cells.Item(i).Specific).Value, CultureInfo.GetCultureInfo("en-US"));

                    //Convert.ToDateTime(lLstDateTime[0])
                    if (((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("ItemCode").Cells.Item(i).Specific).Value != "")
                    {
                        TicketDetail lObjTicketDetail = new TicketDetail();

                        lObjTicketDetail.Folio = pStrFolio;
                        lObjTicketDetail.Item = ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("ItemCode").Cells.Item(i).Specific).Value;


                        LogService.WriteInfo("GUARDAR DETALLE V1.3.18: " +((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Price").Cells.Item(i).Specific).Value.ToString() + "Precio sin conversión");
                        lObjTicketDetail.Price = Convert.ToDouble(((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Price").Cells.Item(i).Specific).Value, CultureInfo.GetCultureInfo("en-US"));
                        LogService.WriteInfo("GUARDAR DETALLE V1.3.18: " + lObjTicketDetail.Price + "Precio con conversión");

                        lObjTicketDetail.FirstWT = lFloFirstWt;
                        lObjTicketDetail.SecondWT = lFloSecondWT;
                        //Log
                        LogService.WriteInfo("GUARDAR DETALLE V1.3.18: " + ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("PesoN").Cells.Item(i).Specific).Value.ToString() + "Cantidad sin conversión");
                        lObjTicketDetail.netWeight = Convert.ToDouble(((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("PesoN").Cells.Item(i).Specific).Value, CultureInfo.GetCultureInfo("en-US"));
                        LogService.WriteInfo("GUARDAR DETALLE V1.3.18: " + lObjTicketDetail.netWeight + "Cantidad con conversión");

                        lObjTicketDetail.Amount = Convert.ToDouble(((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Importe").Cells.Item(i).Specific).Value, CultureInfo.GetCultureInfo("en-US"));
           
                        //lObjTicketDetail.WeighingM =
                        //    bool lBolWeighingM = ((SAPbouiCOM.CheckBox)mObjMatrix.Columns.Item("Check").Cells.Item(i).Specific).Checked);
                        lObjTicketDetail.Line = i - 1;
                        if (!string.IsNullOrEmpty(pOBjDataSource.GetValue("LineNum", i - 1)))
                        {
                            lObjTicketDetail.BaseLine = Convert.ToInt32(pOBjDataSource.GetValue("LineNum", i - 1));
                        }

                        if ((mObjMatrix.Columns.Item("Check").Cells.Item(i).Specific as CheckBox).Checked)
                        {
                            lObjTicketDetail.WeighingM = 1;
                        }
                        else
                        {
                            lObjTicketDetail.WeighingM = 0;
                        }

                        if (!pBolPesaje)
                        {
                            lObjTicketDetail.WhsCode = ((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("WhsCode").Cells.Item(i).Specific).Value;
                            lObjTicketDetail.BagsBales = Convert.ToDouble(((SAPbouiCOM.EditText)mObjMatrix.Columns.Item("Sacos").Cells.Item(i).Specific).Value, CultureInfo.GetCultureInfo("en-US"));
                        }


                        List<string> lLstTicket = new List<string>();
                        if (mBolIsUpdate && lObjTicketDetail.Item != "") // Calcula la hora si es una nueva pesada
                        {
                            string lStrcode = mObjTicketServices.getRowCodeDetail(pStrFolio, lObjTicketDetail.Line);
                            List<string> lLstTicketUpdate = mObjTicketServices.GetDateTimeUpdate(lStrcode);

                            if (lLstTicketUpdate.Count > 0)
                            {
                                if (lLstTicketUpdate[1] == "0")
                                {
                                    lObjTicketDetail.EntryTime = Convert.ToInt32(lLstDateTime[1].Replace(":", ""));
                                    lObjTicketDetail.EntryDate = Convert.ToDateTime(lLstDateTime[0]);
                                }
                                else
                                {
                                    lObjTicketDetail.EntryDate = Convert.ToDateTime(lLstTicketUpdate[0]);
                                    lObjTicketDetail.EntryTime = Convert.ToInt32(lLstTicketUpdate[1]);
                                }
                                if (lLstTicketUpdate[3] == "0")
                                {
                                    lObjTicketDetail.OutputTime = Convert.ToInt32(lLstDateTime[1].Replace(":", ""));
                                    lObjTicketDetail.OutputDate = Convert.ToDateTime(lLstDateTime[0]);
                                }
                                else
                                {
                                    lObjTicketDetail.OutputDate = Convert.ToDateTime(lLstTicketUpdate[2]);
                                    lObjTicketDetail.OutputTime = Convert.ToInt32(lLstTicketUpdate[3]);
                                }
                            }
                        }
                        else
                        {
                            if (lFloFirstWt > 0 || lObjTicketDetail.WeighingM == 1 )
                            {
                                lObjTicketDetail.EntryTime = Convert.ToInt32(lLstDateTime[1].Replace(":", ""));
                                lObjTicketDetail.EntryDate = Convert.ToDateTime(lLstDateTime[0]);
                            }
                            if (lFloSecondWT > 0 || pBolIsSimple)
                            {
                                lObjTicketDetail.OutputTime = Convert.ToInt32(lLstDateTime[1].Replace(":", ""));
                                lObjTicketDetail.OutputDate = Convert.ToDateTime(lLstDateTime[0]);
                            }
                        }

                        lLstTicketDetail.Add(lObjTicketDetail);

                    }
                }

            }
            catch (Exception ex)
            {

                LogService.WriteError("[GetTicketDetailMatrix]: " + ex.Message);
                LogService.WriteError(ex);
            }
            return lLstTicketDetail;
        }

    }
}
