using System;
using System.Collections.Generic;
using SAPbouiCOM.Framework;
using UGRS.Core.Services;
using UGRS.Core.SDK.UI;
using UGRS.Core.SDK.DI.CreditNote;
using UGRS.Core.SDK.DI.CreditNote.Tables;
using System.Linq;
using UGRS.Core.SDK.DI;
using UGRS.AddOn.CreditNote.Services;

namespace UGRS.AddOn.CreditNote.Forms
{
    [FormAttribute("UGRS.AddOn.CreditNote.Forms.frmCreditNote", "Forms/frmCreditNote.b1f")]
    class frmCreditNote : UserFormBase
    {
        #region Properties
       
        
        #endregion

        #region Constructor
        public frmCreditNote()
        {

        }
        #endregion

        #region Initialize


        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.txtDate = ((SAPbouiCOM.EditText)(this.GetItem("txtDate").Specific));
            this.btnNC = ((SAPbouiCOM.Button)(this.GetItem("btnNC").Specific));
            this.btnNC.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btnNC_ClickAfter);
            this.btnReport = ((SAPbouiCOM.Button)(this.GetItem("btnReport").Specific));
            this.mtxInv = ((SAPbouiCOM.Matrix)(this.GetItem("mtxInv").Specific));
            this.lblDate = ((SAPbouiCOM.StaticText)(this.GetItem("lblDate").Specific));
            this.btnSearch = ((SAPbouiCOM.Button)(this.GetItem("btnSearch").Specific));
            this.btnSearch.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSearch_ClickBefore);
            this.UD_Date = this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_Date");
            this.OnCustomInitialize();
        }

       

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }

        private void OnCustomInitialize()
        {
          
        }

        #endregion

        #region Events

        private void btnSearch_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            SearchValue();
        }

        private void btnNC_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                SaveNC_UDT lObjSaveNC = new SaveNC_UDT();
                GetCN_List lObjGetList = new GetCN_List(DtMatrix, txtDate.Value);
                CreditNoteT lObjCreditNoteTSaved = new CreditNoteT();
                ////Obtener datos para guardar reporte
                //string pStrId = lObjGetList.GetId();
                //List<CreditNoteDet> lLstCreditNoteDet = lObjGetList.GetMatrixData(pStrId);
                //List<CreditNoteDoc> lLstCreditNoteDoc = lObjGetList.GetNC_Doc(pStrId, lLstCreditNoteDet);
                //CreditNoteT lObjCreditNoteT = lObjGetList.GetNC_Header(pStrId, lLstCreditNoteDoc);

                ////Guardado de reporte
                //lObjSaveNC.SaveInUDT(lObjCreditNoteT);

                //Guardado de borrador
                 //lObjCreditNoteTSaved = lObjGetList.GetCreditNoteTSaved(pStrId);
                //lObjSaveNC.SaveCreditNoteDoc(lObjCreditNoteTSaved.LstCreditNoteDoc);

                //Actualizacion de borrador
                lObjCreditNoteTSaved = lObjGetList.GetCreditNoteTSaved("NC_4");
                lObjSaveNC.UpdateDocRel(lObjCreditNoteTSaved.LstCreditNoteDoc.Where(x => x.FolioDoc == "NC_4_23").ToList());


                //Actualiza UDT status 
                 lObjCreditNoteTSaved = lObjGetList.GetCreditNoteTSaved("NC_4");
                List<string> lLstError = lObjSaveNC.ValidateDraftRelation(lObjGetList.GetDraftReference("NC_4"), lObjCreditNoteTSaved);
                if (lLstError.Count() > 0)
                {
                    ShowMessageboxList("Algunos facturas no fueron relacionadas correctamente:", lLstError);
                }
                else
                {
                    //Guarda Nota de credito desde borrador
                    List<string> lLstErrorDoc = lObjSaveNC.SaveDraftToDocument(lObjCreditNoteTSaved);
                    if (lLstErrorDoc.Count() > 0)
                    {
                        ShowMessageboxList("No fue posible generar algunos documentos", lLstErrorDoc);
                    }
                }


             

            }
            catch (Exception ex)
            { 
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
        }
        #endregion

        #region Methods

        private void ShowMessageboxList(string pStrMessage, List<string> pLstError)
        {
            string lStrMessageError = string.Format(pStrMessage + "\n{0}",
                       string.Join("\n", pLstError.Select(x => string.Format("{0}", x)).ToArray()));
            LogService.WriteError(lStrMessageError);
            Application.SBO_Application.MessageBox(lStrMessageError);
        }


        /// <summary>
        /// Realiza la busqueda
        /// </summary>
        private void SearchValue()
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);
                SetDataTableValues();
                BindMatrix();
            }
            catch (Exception ex)
            {
                this.UIAPIRawForm.Freeze(false);
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
                //mObjProgressBar.Dispose();
            }
        }

        /// <summary>
        /// Carga los datos desde una consulta al datatable
        /// </summary>
        private void SetDataTableValues()
        {
            CreditNoteFactory lObjCreditNoteFactory = new CreditNoteFactory();
            DtMatrix = this.UIAPIRawForm.DataSources.DataTables.Item("Dt_INV");
            DateTime lDtmDate = DateTime.Now;// Convert.ToDateTime(txtDate.Value);
            DtMatrix.ExecuteQuery(lObjCreditNoteFactory.GetCreditNoteService().GetInvoiceQuery(lDtmDate, lDtmDate));
        }

        /// <summary>
        /// Asigna las columnas del datatable a la matriz
        /// </summary>
        private void BindMatrix()
        {
            foreach (SAPbouiCOM.Column item in mtxInv.Columns)
            {
                mtxInv.Columns.Item(item.UniqueID).DataBind.Bind("Dt_INV", item.UniqueID);
            }
            mtxInv.LoadFromDataSource();
            mtxInv.AutoResizeColumns();
        }



      


       
        #endregion

        #region Controls
        private SAPbouiCOM.DataTable DtMatrix;
        private SAPbouiCOM.EditText txtDate;
        private SAPbouiCOM.Button btnNC;
        private SAPbouiCOM.Button btnReport;
        private SAPbouiCOM.Matrix mtxInv;
        private SAPbouiCOM.StaticText lblDate;
        private SAPbouiCOM.Button btnSearch;
        private SAPbouiCOM.UserDataSource UD_Date;
        #endregion

    }
}
