using System;
using System.Collections.Generic;
using SAPbouiCOM.Framework;
using UGRS.Core.Services;
using UGRS.Core.SDK.UI;
using UGRS.Core.SDK.DI.CreditNote;
using UGRS.Core.SDK.UI.ProgressBar;
using UGRS.Core.SDK.DI.CreditNote.Tables;
using System.Linq;
using UGRS.Core.SDK.DI;
using System.Globalization;

namespace UGRS.AddOn.CreditNote.Forms
{
    [FormAttribute("UGRS.AddOn.CreditNote.Forms.frmCreditNote", "Forms/frmCreditNote.b1f")]
    class frmCreditNote : UserFormBase
    {
        #region Properties
        ProgressBarManager mObjProgressBar = null;
        CreditNoteFactory mObjCreditNoteFactory = new CreditNoteFactory();
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
                string pStrId = GetId();
                List<CreditNoteDet> lLstCreditNoteDet = GetMatrixData(pStrId);
                List<CreditNoteDoc> lLstCreditNoteDoc = GetNC_Doc(pStrId, lLstCreditNoteDet);
                CreditNoteT lObjCreditNoteDoc = GetNC_Header(pStrId, lLstCreditNoteDoc);

                SaveInUDT(lObjCreditNoteDoc);
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex);
                UIApplication.ShowMessageBox(ex.Message);
            }
        }
        #endregion

        #region Methods

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
            DtMatrix = this.UIAPIRawForm.DataSources.DataTables.Item("Dt_INV");
            DateTime lDtmDate = DateTime.Now;// Convert.ToDateTime(txtDate.Value);
            DtMatrix.ExecuteQuery(mObjCreditNoteFactory.GetCreditNoteService().GetInvoiceQuery(lDtmDate));
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


        /// <summary>
        /// Obtener los datos de la matriz
        /// </summary>
        private string GetId()
        {
            string lStrId = "";

            lStrId = string.Format("NC_{0}", mObjCreditNoteFactory.GetCreditNoteService().GetLastCode()+1);
            return lStrId;
        }


        /// <summary>
        /// Obtener los datos de la matriz
        /// </summary>
        private List<CreditNoteDet> GetMatrixData(string pStrId)
        {
            List<CreditNoteDet> lLstCN = new List<CreditNoteDet>();
            try
            {
             
                if (DtMatrix != null)
                {
                    mObjProgressBar = new ProgressBarManager(UIApplication.GetApplication(), "Obtencion de registros", DtMatrix.Rows.Count);
                    for (int i = 0; i < DtMatrix.Rows.Count; i++)
                    {
                        lLstCN.Add(GetDTMatrixRow(i, pStrId));
                        mObjProgressBar.NextPosition();
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex);
            }
            finally
            {
                if (mObjProgressBar != null)
                {
                    mObjProgressBar.Dispose();
                }
            }
            return lLstCN;
        }

        /// <summary>
        /// Obtener el renglon de la matriz
        /// </summary>
        private CreditNoteDet GetDTMatrixRow(int pIntRow, string pStrId)
        {
            CreditNoteDet lObjCN_DTO = new CreditNoteDet
            {
                Amount = float.Parse(DtMatrix.GetValue("C_Amount", pIntRow).ToString()),
                Canceled = "N",
                CardCode = DtMatrix.GetValue("C_CardCode", pIntRow).ToString(),
                CardName = DtMatrix.GetValue("C_CardName", pIntRow).ToString(),
                Cert = DtMatrix.GetValue("C_Cert", pIntRow).ToString(),
                CreationDate = DateTime.Now,
                CreationTime = DateTime.Now.ToString("hhmm"),
                DocEntryINV = Convert.ToInt32(DtMatrix.GetValue("C_DocEntry", pIntRow)),
                DocNumINV = DtMatrix.GetValue("C_DocNum", pIntRow).ToString(),
                Line = pIntRow,
                NcId = pStrId,
                Processed = "N",
                QtyExp = Convert.ToInt32(DtMatrix.GetValue("C_HeadExp", pIntRow).ToString()),
                QtyNoCruz = Convert.ToInt32(DtMatrix.GetValue("C_HeadNoC", pIntRow).ToString()),
                QtyInv = Convert.ToInt32(DtMatrix.GetValue("C_InvHead", pIntRow).ToString()),
            };


            return lObjCN_DTO;
        }


        /// <summary>
        /// Obtener el docmento ordenado por cardcode
        /// </summary>
        private List<CreditNoteDoc> GetNC_Doc(string pStrId, List<CreditNoteDet> pLstCreditNoteDet)
        {
            List<CreditNoteDoc> lLstCreditNoteDocuments = new List<CreditNoteDoc>();
            List<CreditNoteDet> lLstCardCode = pLstCreditNoteDet.GroupBy(x => x.CardCode).Select(y => new CreditNoteDet { CardCode = y.First().CardCode }).ToList();
            int i = 1;
            foreach (var lObjCardCode in lLstCardCode)
            {
                CreditNoteDoc lObjCreditNoteDoc = new CreditNoteDoc
                {
                    Amount = pLstCreditNoteDet.Where(y => y.Canceled == "N").Sum(x => x.Amount),
                    NcId = pStrId,
                    CardCode = pLstCreditNoteDet.First().CardCode,
                    CardName = pLstCreditNoteDet.First().CardName,
                    CreationDate = DateTime.Now,
                    CreationTime = DateTime.Now.ToString("hhmm"),
                    DocEntry = "",
                    IVA = 0,
                    Line = i,
                    LstCreditNoteDet = pLstCreditNoteDet,
                    Processed = "N",
                    Canceled = "N",
                    QtyInv = pLstCreditNoteDet.Count(),
                    User = DIApplication.Company.UserName
                };
                lLstCreditNoteDocuments.Add(lObjCreditNoteDoc);
            }

            return lLstCreditNoteDocuments;
        }

        /// <summary>
        /// Obtener el encabezado
        /// </summary>
        private CreditNoteT GetNC_Header(string pStrId, List<CreditNoteDoc> pLstCreditNoteDoc)
        {
            CreditNoteT lObjCreditNoteT = new CreditNoteT
            {
                NcId = pStrId,
                Date = string.IsNullOrEmpty(txtDate.Value) ? DateTime.Now :
                                DateTime.ParseExact(txtDate.Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None),
                CreationDate = DateTime.Now,
                Autorized = "N",
                Canceled = "N",
                Processed = "N",
                CreationTime = DateTime.Now.ToString("hhmm"),
                QtyDoc = pLstCreditNoteDoc.Count(),
                QtyInv = pLstCreditNoteDoc.Sum(x => x.LstCreditNoteDet.Count),
                Total =  pLstCreditNoteDoc.Sum(x => x.Amount),
                User = DIApplication.Company.UserName,
                Attach = "",
                LstCreditNoteDoc = pLstCreditNoteDoc,
            };

            return lObjCreditNoteT;
        }

        /// <summary>
        /// Obtener el encabezado
        /// </summary>
        /// 
        private void SaveInUDT(CreditNoteT pObjCreditNoteT)
        {
            //DIApplication.Company.StartTransaction();

            int lIntResult = mObjCreditNoteFactory.GetCreditNoteTService().Add(pObjCreditNoteT);
            SaveDocInUDT(pObjCreditNoteT.LstCreditNoteDoc);
           
        }

        private void SaveDocInUDT(List<CreditNoteDoc> pLstCreditNoteDoc)
        {
            //Test
            int lIntResult = mObjCreditNoteFactory.GetCreditNoteDocService().Add(pLstCreditNoteDoc.First());

            //foreach (var lObjCreditNoteDoc in pLstCreditNoteDoc)
            //{
            //    mObjCreditNoteFactory.GetCreditNoteDocService().Add(pObjCreditNoteT.LstCreditNoteDoc);
            //}

            SaveDetInUDT(pLstCreditNoteDoc.First().LstCreditNoteDet);
        }

        private void SaveDetInUDT(List<CreditNoteDet> pLstCreditNoteDet)
        {
            int lIntResult = mObjCreditNoteFactory.GetCreditNoteDetService().Add(pLstCreditNoteDet.First());
        }


        private bool CloseTransaction(bool pBolSuccess)
        {
            try
            {
                if (pBolSuccess)
                {
                    DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    pBolSuccess = true;
                    LogService.WriteSuccess("Generado correctamente");
                    UIApplication.ShowSuccess("Generado correctamente");
                   // MenuNewForm();
                }
                else
                {
                    if (DIApplication.Company.InTransaction)
                    {
                        pBolSuccess = false;
                        DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                    }
                }
            }
            catch (Exception ex)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(ex.Message);
                LogService.WriteError("(CloseTransaction): " + ex.Message);
                LogService.WriteError(ex);
            }
            return pBolSuccess;
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
