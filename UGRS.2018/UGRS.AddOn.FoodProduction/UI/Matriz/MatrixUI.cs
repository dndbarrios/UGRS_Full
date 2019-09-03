using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UGRS.AddOn.FoodProduction.Services;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.FoodProduction.DAO;
using UGRS.Core.SDK.DI.FoodProduction.Tables;

namespace UGRS.AddOn.FoodProduction.UI.Matriz
{
    public class MatrixUI
    {
        TicketServices mObjTicketServices = new TicketServices();

        ///<summary>    Creates the matrix. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        public SAPbouiCOM.Matrix CreateMatrix(SAPbouiCOM.IItem pObjItem, SAPbouiCOM.ChooseFromListCollection pObjCFLs)
        {
            SAPbouiCOM.Matrix lObjMatrix;
            lObjMatrix = ((SAPbouiCOM.Matrix)(pObjItem.Specific));
            lObjMatrix.Layout = SAPbouiCOM.BoMatrixLayoutType.mlt_Normal;
            lObjMatrix.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Auto;
           lObjMatrix.ValidateBefore += lObjMatrix_ValidateBefore;
            lObjMatrix.ValidateAfter += lObjMatrix_ValidateAfter;

            SAPbouiCOM.ChooseFromListCreationParams lObjCFLCreationParams;
            SAPbouiCOM.Conditions oCons = null;
            SAPbouiCOM.Condition oCon = null;
            lObjCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)SAPbouiCOM.Framework.Application.SBO_Application.CreateObject(BoCreatableObjectType.cot_ChooseFromListCreationParams);

            //Adding a choosefromlist for the column
            lObjCFLCreationParams.MultiSelection = false;
            lObjCFLCreationParams.ObjectType = "4";
            lObjCFLCreationParams.UniqueID = "CFL1";
            SAPbouiCOM.ChooseFromList lObjCFL = pObjCFLs.Add(lObjCFLCreationParams);

            //Add Conditon Rcordova 19-10-2017
            oCons = lObjCFL.GetConditions();

            oCon = oCons.Add();
            oCon.Alias = "SellItem";
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
            oCon.CondVal = "Y";

            lObjCFL.SetConditions(oCons);
            //Add Conditon Rcordova 19-10-2017

            //Adding a choosefromlist for the column
            lObjCFLCreationParams.MultiSelection = false;
            lObjCFLCreationParams.ObjectType = "64";
            lObjCFLCreationParams.UniqueID = "CFL_Ware";
            pObjCFLs.Add(lObjCFLCreationParams);

            //  SAPbouiCOM.UserDataSource udsCardCode = this.UIAPIRawForm.DataSources.UserDataSources.Add("UdsCardCd", BoDataType.dt_SHORT_TEXT, 10);
            return lObjMatrix;
        }

        private void lObjMatrix_ValidateAfter(object sboObject, SBOItemEventArg pVal)
        {
           // int i = 0;
          
        }

        private void lObjMatrix_ValidateBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
          //  int i = 0;
        }

        //Carga los datos al data source de la matriz
        public SAPbouiCOM.DBDataSource LoadMatrixData(Ticket pObjTicket, SAPbouiCOM.DBDataSource pDBDataSourceD, IList<TicketDetail> pLstTicketDetail, string pStrSource)
        {
            int i = 0;
            double lFloQuantity = 0;
            foreach (TicketDetail lObjTicketDetail in pLstTicketDetail.OrderByDescending(x => x.Line))
            {
                ///LE Importe Pesaje simple
                if (pObjTicket.CapType == 4)
                {
                    lFloQuantity = 1;
                }
                else
                {
                    lFloQuantity = lObjTicketDetail.netWeight;
                }
                //mObjQueryManager.GetObjectsList<

                pDBDataSourceD.InsertRecord(i);
                pDBDataSourceD.SetValue("ItemCode", i, lObjTicketDetail.Item);
                pDBDataSourceD.SetValue("Dscription", i, mObjTicketServices.SearchItemName(lObjTicketDetail.Item));
                pDBDataSourceD.SetValue("Price", i, lObjTicketDetail.Price.ToString(CultureInfo.InvariantCulture));
                pDBDataSourceD.SetValue("Weight1", i, lObjTicketDetail.FirstWT.ToString(CultureInfo.InvariantCulture));
                pDBDataSourceD.SetValue("Weight2", i, lObjTicketDetail.SecondWT.ToString(CultureInfo.InvariantCulture));
                pDBDataSourceD.SetValue("Quantity", i, lFloQuantity.ToString(CultureInfo.InvariantCulture));
                pDBDataSourceD.SetValue("LineTotal", i, (lFloQuantity * lObjTicketDetail.Price).ToString(CultureInfo.InvariantCulture));
                pDBDataSourceD.SetValue("U_GLO_BagsBales", i, lObjTicketDetail.BagsBales.ToString());
                pDBDataSourceD.SetValue("WhsCode", i, lObjTicketDetail.WhsCode.ToString());
                pDBDataSourceD.SetValue("FromWhsCod", i, lObjTicketDetail.WhsCode.ToString());
                
                pDBDataSourceD.SetValue("LineNum", i, lObjTicketDetail.BaseLine.ToString());
                string lStrLine = GetTableLine(pObjTicket.DocType, pStrSource);
                string lStrTable = lStrLine.Remove(lStrLine.Length - 1);
                pDBDataSourceD.SetValue("OpenCreQty", i, mObjTicketServices.GetOpenLine("O" + lStrTable, lStrLine, "OpenCreQty", pObjTicket.Number.ToString(), lObjTicketDetail.BaseLine.ToString()));
                pDBDataSourceD.SetValue("DelivrdQty", i, mObjTicketServices.GetDeliveryLine("O" + lStrTable, lStrLine, "DelivrdQty", pObjTicket.Number.ToString(), lObjTicketDetail.BaseLine.ToString()));

            }
            return pDBDataSourceD;
        }
        
        //Obtiene la tabla a buscar la cantidad pendiente/entregada
        public string GetTableLine(int pIntDocType, string pStrSource)
        {
            string lStrLine = pStrSource;
            if (pIntDocType == 1)
            {
                if (pStrSource == "RDR1" || pStrSource == "INV1")
                {
                    lStrLine = "INV1";
                }
                else
                {
                    lStrLine = "PCH1";
                }
            }
            return lStrLine;
            //if (!string.IsNullOrEmpty(mStrLastPeso) && float.Parse(mStrLastPeso) < lObjTicketDetail.SecondWT)
            //{
            //    mStrLastPeso = lObjTicketDetail.SecondWT.ToString();
            //}
            //i++;
        }

        //Pone el checkbox en Verdadero o falso
        public SAPbouiCOM.DBDataSource SetCheckbox(SAPbouiCOM.IMatrix pObjMatrix, IList<TicketDetail> pLstTicketDetail, SAPbouiCOM.DBDataSource pDBDataSourceD)
        {
            SAPbouiCOM.CommonSetting lObjRowCtrl;
            lObjRowCtrl = pObjMatrix.CommonSetting;

            for (int i = 1; i <= pObjMatrix.RowCount; i++)
            {
                if (pLstTicketDetail[i - 1].WeighingM == 1)
                {
                    ((SAPbouiCOM.CheckBox)pObjMatrix.Columns.Item("Check").Cells.Item(i).Specific).Checked = true;
                    lObjRowCtrl.SetCellEditable(i, 5, true);
                    pDBDataSourceD.SetValue("TreeType", i - 1, "Y");
                }
                else
                {
                    ((SAPbouiCOM.CheckBox)pObjMatrix.Columns.Item("Check").Cells.Item(i).Specific).Checked = false;
                    pDBDataSourceD.SetValue("TreeType", i - 1, "N");
                }

            }
            return pDBDataSourceD;
        }


        /// <summary>
        /// Metodo para buscar lineas en la matriz comparando los valores con la lista para marcar o no el checkbox
        /// </summary>
        /// <param name="pObjMatrix"></param>
        /// <param name="pLstTicketDetail"></param>
        /// <param name="pDBDataSourceD"></param>
        /// <returns></returns>
        public SAPbouiCOM.DBDataSource SetCheckbox2(SAPbouiCOM.IMatrix pObjMatrix, IList<TicketDetail> pLstTicketDetail, SAPbouiCOM.DBDataSource pDBDataSourceD)
        {
            SAPbouiCOM.CommonSetting lObjRowCtrl;
            lObjRowCtrl = pObjMatrix.CommonSetting;

            for (int i = 1; i <= pObjMatrix.RowCount; i++)
            {
                string l = ((SAPbouiCOM.EditText)pObjMatrix.Columns.Item("ItemCode").Cells.Item(i).Specific).Value;
                double xe = Convert.ToDouble(((SAPbouiCOM.EditText)pObjMatrix.Columns.Item("PesoN").Cells.Item(i).Specific).Value, CultureInfo.InvariantCulture);
                foreach (var item in pLstTicketDetail.Where(x=> x.Item == l && x.netWeight == xe))
                {
                    if(item.WeighingM == 1)
                    {
                        ((SAPbouiCOM.CheckBox)pObjMatrix.Columns.Item("Check").Cells.Item(i).Specific).Checked = true;
                        lObjRowCtrl.SetCellEditable(i, 5, true);
                        pDBDataSourceD.SetValue("TreeType", i - 1, "Y");
                    }
                    else
                    {
                        ((SAPbouiCOM.CheckBox)pObjMatrix.Columns.Item("Check").Cells.Item(i).Specific).Checked = false;
                        pDBDataSourceD.SetValue("TreeType", i - 1, "N");
                    }
                }

            }


               

            

            //for (int i = 1; i <= pObjMatrix.RowCount; i++)
            //{
            //    if (pLstTicketDetail[i - 1].WeighingM == 1)
            //    {
            //        ((SAPbouiCOM.CheckBox)pObjMatrix.Columns.Item("Check").Cells.Item(i).Specific).Checked = true;
            //        pDBDataSourceD.SetValue("TreeType", i - 1, "Y");
            //    }
            //    else
            //    {
            //        ((SAPbouiCOM.CheckBox)pObjMatrix.Columns.Item("Check").Cells.Item(i).Specific).Checked = false;
            //        pDBDataSourceD.SetValue("TreeType", i - 1, "N");
            //    }

            //}
            return pDBDataSourceD;
        }



        /// <summary>
        /// Elimina las columnas (Para cargar nuevamente la matriz)
        /// </summary>
        public SAPbouiCOM.Matrix DeleteColumns(SAPbouiCOM.Matrix pObjMatrix)
        {
            int lIntRow = pObjMatrix.RowCount;
            if (lIntRow > 0)
            {
                for (int i = lIntRow; i > 0; i--)
                {
                    pObjMatrix.DeleteRow(i);
                }

            }
            int lIntCount = pObjMatrix.Columns.Count;
            for (int i = 0; i < lIntCount; i++)
            {
                pObjMatrix.Columns.Remove(0);
            }

            return pObjMatrix;

        }


        /// <summary>
        /// Agrega un item de venta de pesaje a la matriz
        /// </summary>
        public SAPbouiCOM.DBDataSource AddItemService(SAPbouiCOM.DBDataSource mDBDataSourceD)
        {
            mDBDataSourceD.Clear();
            mDBDataSourceD.InsertRecord(0);
             QueryManager lObjQueryManager = new QueryManager();
             string lSTrItemCode = lObjQueryManager.GetValue("U_Value", "Name", "PL_WEIGHING_SALE", "[@UG_CONFIG]");
             mDBDataSourceD.SetValue("ItemCode", 0, lSTrItemCode); //Ponerlo en configuracion

            TicketDAO lObjTicketDAO = new TicketDAO();
            string lStrPrice = lObjTicketDAO.GetPrice(lSTrItemCode);
            mDBDataSourceD.SetValue("Dscription", 0, "SERVICIO DE BASCULA");
            mDBDataSourceD.SetValue("Price", 0, lStrPrice);

            if (lStrPrice == "0")
            {
                mDBDataSourceD.SetValue("Price", 0, "1");
            }
            mDBDataSourceD.SetValue("Quantity", 0, "1");
            mDBDataSourceD.SetValue("LineTotal", 0, "0");


            return mDBDataSourceD;
        }

        /// <summary>
        /// Agrega un item de venta de pesaje a la matriz
        /// </summary>
        public SAPbouiCOM.DBDataSource AddItemWeigin(SAPbouiCOM.DBDataSource mDBDataSourceD)
        {
            mDBDataSourceD.Clear();
            mDBDataSourceD.InsertRecord(0);
            QueryManager lObjQueryManager = new QueryManager();
            string lSTrItemCode = lObjQueryManager.GetValue("U_Value", "Name", "PL_ITEM_WEIGH", "[@UG_CONFIG]");
            mDBDataSourceD.SetValue("ItemCode", 0, lSTrItemCode); //Ponerlo en configuracion

            TicketDAO lObjTicketDAO = new TicketDAO();
            string lStrPrice = lObjTicketDAO.GetPrice(lSTrItemCode);
            mDBDataSourceD.SetValue("Dscription", 0, "Articulo de pesaje simple");
            mDBDataSourceD.SetValue("Price", 0, lStrPrice);
            if (lStrPrice == "0")
            {
                mDBDataSourceD.SetValue("Price", 0, "0");
            }
            mDBDataSourceD.SetValue("Quantity", 0, "0");
            mDBDataSourceD.SetValue("LineTotal", 0, "0");
            return mDBDataSourceD;
        }

          ///<summary>    Loads matrix data from document. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        ///<param name="pstrAlias"> The pstr alias. </param>
        ///<param name="pstrCond">  The pstr condition. </param>
        public SAPbouiCOM.DBDataSource LoadMatrixConditions(string pstrAlias, string pstrCond, SAPbouiCOM.DBDataSource pDBDataSourceD)
        {
            SAPbouiCOM.Conditions lObjCons = null;
            SAPbouiCOM.Condition lObjCon = null;

            //PL001  1
            lObjCons = new SAPbouiCOM.Conditions();
            lObjCon = lObjCons.Add();
            lObjCon.BracketOpenNum = 1;
            lObjCon.Alias = pstrAlias;
            lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
            lObjCon.CondVal = pstrCond;
            lObjCon.BracketCloseNum = 1;

            ///Validacion para cargar solo las lineas abiertas
            lObjCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

            lObjCon = lObjCons.Add();
            lObjCon.BracketOpenNum = 1;
            lObjCon.Alias = "LineStatus";
            lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
            lObjCon.CondVal = "O";
            lObjCon.BracketCloseNum = 1;
            pDBDataSourceD.Query(lObjCons);

            //Validacion para evitar lineas sin cantidades pendientes
            lObjCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

            lObjCon = lObjCons.Add();
            lObjCon.BracketOpenNum = 1;
            lObjCon.Alias = "OpenQty";
            lObjCon.Operation = SAPbouiCOM.BoConditionOperation.co_GRATER_THAN;
            lObjCon.CondVal = "0";
            lObjCon.BracketCloseNum = 1;
            pDBDataSourceD.Query(lObjCons);

            return pDBDataSourceD;
        }


        ///<summary>    Loads matrix columns. </summary>
        ///<remarks>    Amartinez, 08/05/2017. </remarks>
        ///<param name="pStrDataSource">    The pstr datasource. </param>
        public SAPbouiCOM.Matrix LoadMatrixColumns(SAPbouiCOM.Matrix pObjMatrix, string pStrDataSource, bool pBolIsPurshese, string pStrTypTic)
        {
            SAPbouiCOM.Matrix lObjMatrix = DeleteColumns(pObjMatrix);
            bool lBolColumnEnable = false;

            lBolColumnEnable = pBolIsPurshese;
            //SAPbouiCOM.CommonSetting setting = mObjMatrix.CommonSetting;
            // Add a column for BP Card Code
            SAPbouiCOM.IColumn lObjColumn;
            lObjColumn = lObjMatrix.Columns.Add("ItemCode", SAPbouiCOM.BoFormItemTypes.it_LINKED_BUTTON);
            lObjColumn.TitleObject.Caption = "Artículo";
            lObjColumn.DataBind.SetBound(true, pStrDataSource, "ItemCode");
            lObjColumn.Editable = true;

            // Link the column to the BP master data system form
            var lObjlink = ((SAPbouiCOM.LinkedButton)(lObjColumn.ExtendedObject));
            lObjlink.LinkedObject = SAPbouiCOM.BoLinkedObject.lf_Items;
            lObjColumn.ChooseFromListUID = "CFL1";
            lObjColumn.Editable = true;

            //addColum()
            AddColum(lObjMatrix, "Dscription", SAPbouiCOM.BoFormItemTypes.it_EDIT, "Descripción", pStrDataSource, "Dscription", false);
            AddColum(lObjMatrix, "Price", SAPbouiCOM.BoFormItemTypes.it_EDIT, "Precio", pStrDataSource, "Price", lBolColumnEnable);
            AddColum(lObjMatrix, "Peso1", SAPbouiCOM.BoFormItemTypes.it_EDIT, "Primer peso", pStrDataSource, "Weight1", false);
            AddColum(lObjMatrix, "Peso2", SAPbouiCOM.BoFormItemTypes.it_EDIT, "Segundo peso", pStrDataSource, "Weight2", false);
            AddColum(lObjMatrix, "PesoN", SAPbouiCOM.BoFormItemTypes.it_EDIT, "Peso Neto", pStrDataSource, "Quantity", false);
            AddColum(lObjMatrix, "Importe", SAPbouiCOM.BoFormItemTypes.it_EDIT, "Importe", pStrDataSource, "LineTotal", false);  
           
            if (pStrTypTic != "Venta de pesaje" && pStrTypTic != "Pesaje")
            {
                AddColum(lObjMatrix, "Sacos", SAPbouiCOM.BoFormItemTypes.it_EDIT, "Sacos - Pacas", pStrDataSource, "U_GLO_BagsBales", true);
                AddColum(lObjMatrix, "Check", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX, "Tipo peso", pStrDataSource, "TreeType", true);
                lObjColumn = lObjMatrix.Columns.Add("WhsCode", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                lObjColumn.TitleObject.Caption = "Almacén";

                if (pStrTypTic != "Traslado - Salida")
                {
                    lObjColumn.DataBind.SetBound(true, pStrDataSource, "WhsCode");
                }
                else
                {
                    lObjColumn.DataBind.SetBound(true, pStrDataSource, "FromWhsCod");
                }
                lObjColumn.ChooseFromListUID = "CFL_Ware";
                lObjColumn.Editable = true;
            }
            else
            {
                AddColum(lObjMatrix, "Check", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX, "Tipo peso", pStrDataSource, "TreeType", false);
            }
            AddColum(lObjMatrix, "DelivrdQty", SAPbouiCOM.BoFormItemTypes.it_EDIT, "Cantidad entregada", pStrDataSource, "DelivrdQty", false);
            AddColum(lObjMatrix, "OpenCreQty", SAPbouiCOM.BoFormItemTypes.it_EDIT, "Cantidad pendiente", pStrDataSource, "OpenCreQty", false);

           
            lObjMatrix.AutoResizeColumns();
            return lObjMatrix;
        }

        /// <summary>
        /// Agrega una columna a la matriz
        /// </summary>
        private void AddColum(SAPbouiCOM.IMatrix pObjMatrix, string pStrName, SAPbouiCOM.BoFormItemTypes pObjType, string pStrTitle, string pStrDatasource, string pStrItemSource, bool pBolEdit)
        {
            SAPbouiCOM.IColumn lObjColumn;
            lObjColumn = pObjMatrix.Columns.Add(pStrName, pObjType);
            lObjColumn.Width = 60;
            lObjColumn.TitleObject.Caption = pStrTitle;
            lObjColumn.DisplayDesc = false;
            lObjColumn.DataBind.SetBound(true, pStrDatasource, pStrItemSource);
            lObjColumn.Editable = pBolEdit;
            
        }
    }
}
