using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using Core.SDK.DI;
using Core.SDK.UI;
using Core.SDK.DI.Facturacion.DAO;
using Core.Utility;
using System.Xml;
using Core.SDK.DI.Facturacion.DTO;
using Core.SDK.DI.Facturacion;

namespace Addon.CFDI_Facturacion
{
    public class BtnTimbradoControl
    {
        private SAPbouiCOM.Form mFrmInvoice;
        //private SAPbouiCOM.Item mBtnTimbrado;

        private string mStrObjType = string.Empty;
        private string mStrDocEntry = string.Empty;
        private string mStrDocNum = string.Empty;
        private bool mBolFindMode = false;

        private TimbradoFactory mObjTimbradoFactory = null;

        public BtnTimbradoControl(SAPbouiCOM.Form pForm)
        {
            UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
            UIApplication.GetApplication().MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(SBO_Application_ApplicationMenuEvent);
            UIApplication.GetApplication().FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(SBO_Application_DataFormEvent);

            mObjTimbradoFactory = new TimbradoFactory();
            (mFrmInvoice as SAPbouiCOM.Framework.UserFormBase).CloseAfter += new SAPbouiCOM.Framework.FormBase.CloseAfterHandler(Form_CloseAfter);

            this.mFrmInvoice = pForm;
            //AddDefaultButton();
            AddTimbradoButton();
        }

        public BtnTimbradoControl()
        {
            UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
            //UIApplication.GetApplication().MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(SBO_Application_ApplicationMenuEvent);
            //UIApplication.GetApplication().FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(SBO_Application_DataFormEvent);

            mObjTimbradoFactory = new TimbradoFactory();
        }

        private void Form_CloseAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            UIApplication.GetApplication().ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
            UIApplication.GetApplication().MenuEvent -= new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(SBO_Application_ApplicationMenuEvent);
        }

        private void AddTimbradoButton()
        {
            //string lStrCancel = "2"; // boton cancelar (salir) 
            //SAPbouiCOM.Form lObjForm = UIApplication.GetApplication().Forms.ActiveForm;
            try
            {
                if (!ItemExist("BtnTimb"))
                {
                    SAPbouiCOM.Item mBtnTimbrado = this.mFrmInvoice.Items.Add("BtnTimb", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                    mBtnTimbrado.Top = this.mFrmInvoice.Items.Item("2").Top;
                    mBtnTimbrado.Left = this.mFrmInvoice.Items.Item("2").Left + 83;
                    mBtnTimbrado.FromPane = 0;
                    mBtnTimbrado.ToPane = 0;
                    mBtnTimbrado.LinkTo = "2";
                    mBtnTimbrado.Width = 145;
                    mBtnTimbrado.Height = 22;
                    mBtnTimbrado.Enabled = true;
                    (mBtnTimbrado.Specific as SAPbouiCOM.Button).Caption = "Timbrar Documento";
                    //(mBtnTimbrado.Specific as SAPbouiCOM.Button).ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(mItmBtnCard_ClickBefore);
                }

                ////if (lObjForm.Type < 0)
                ////{
                ////    lObjForm = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(lObjForm.Type * (-1), lObjForm.TypeCount);
                ////}
                //if (mBtnTimbrado == null)
                //{
                //    mBtnTimbrado = this.mFrmInvoice.Items.Add("BtnTimb", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                //    mBtnTimbrado.Top = this.mFrmInvoice.Items.Item(lStrCancel).Top;
                //    mBtnTimbrado.Left = this.mFrmInvoice.Items.Item(lStrCancel).Left + 83;
                //    mBtnTimbrado.FromPane = 0;
                //    mBtnTimbrado.ToPane = 0;
                //    mBtnTimbrado.LinkTo = "2";
                //    mBtnTimbrado.Width = 145;
                //    mBtnTimbrado.Height = 22;
                //    mBtnTimbrado.Enabled = false;
                //    (mBtnTimbrado.Specific as SAPbouiCOM.Button).Caption = "Timbrar Documento";
                //    (mBtnTimbrado.Specific as SAPbouiCOM.Button).ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(mItmBtnCard_ClickBefore);
                //}

                //if (this.mFrmInvoice.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                //{
                //    if (mBtnTimbrado != null)
                //    {
                //        //mBtnTimbrado.Visible = false;
                //        mBtnTimbrado.Enabled = false;
                //    }

                //    return;
                //}

                //this.mFrmInvoice.Freeze(true);

                //if (string.IsNullOrEmpty(this.mStrDocEntry))
                //    return;

                //var lObjFactura = mObjTimbradoFactory.GetTimbradoCFDi().GetFacturaPorTimbrar(int.Parse(this.mStrDocEntry));
                //if (lObjFactura == null)
                //{
                //    if (mBtnTimbrado != null)
                //    {
                //        //mBtnTimbrado.Visible = false;
                //        mBtnTimbrado.Enabled = false;
                //    }
                //}
                //else if (!ItemExist("BtnTimb") && (lObjFactura != null && lObjFactura.Timbrado == 0))
                //{
                //    if (mBtnTimbrado == null)
                //    {
                //        mBtnTimbrado = this.mFrmInvoice.Items.Add("BtnTimb", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                //        mBtnTimbrado.Top = this.mFrmInvoice.Items.Item(lStrCancel).Top;
                //        mBtnTimbrado.Left = this.mFrmInvoice.Items.Item(lStrCancel).Left + 83;
                //        mBtnTimbrado.FromPane = 0;
                //        mBtnTimbrado.ToPane = 0;
                //        mBtnTimbrado.LinkTo = "2";
                //        mBtnTimbrado.Width = 145;
                //        mBtnTimbrado.Height = 22;
                //        mBtnTimbrado.Enabled = true;
                //        (mBtnTimbrado.Specific as SAPbouiCOM.Button).Caption = "Timbrar Documento";
                //        (mBtnTimbrado.Specific as SAPbouiCOM.Button).ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(mItmBtnCard_ClickBefore);
                //    }
                //    else
                //    {
                //        //mBtnTimbrado.Visible = true;
                //        mBtnTimbrado.Enabled = true;
                //    }
                //}

            }
            catch (Exception ex)
            {
                LogUtility.WriteError(string.Format("[Timbrado - AddTimbradoButton] Error al agregar el botón de Timbrado: {0}", ex.Message));
                UIApplication.ShowMessageBox(string.Format("Error al agregar el botón de Timbrado: {0}", ex.Message));
            }
            finally
            {
                this.mFrmInvoice.Freeze(false);
            }
        }

        #region EventsHandle
        /// <summary>
        /// SBO_Application_ItemEvent
        /// Metodo para controlar los eventos de la pantalla.
        /// @Author FranciscoFimbres
        /// </summary>
        /// <param name="FormUID"></param>
        /// <param name="pVal"></param>
        /// <param name="BubbleEvent"></param>
        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                //SAPbouiCOM.Form lObjForm = UIApplication.GetApplication().Forms.ActiveForm;
                //if (lObjForm.Type < 0)
                //{
                //    lObjForm = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(lObjForm.Type * (-1), lObjForm.TypeCount);
                //}

                if (pVal.FormType != 133 && pVal.FormType != 60091 /*&& lObjForm.Type != 14 && lObjForm.Type != 24*/)
                    return;

                if (pVal.FormType == -133)
                    return;

                if (!pVal.BeforeAction)
                {
                    switch (pVal.EventType)
                    {
                        case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                            mFrmInvoice = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);

                            /*UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
                            UIApplication.GetApplication().MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(SBO_Application_ApplicationMenuEvent);*/
                            if (!ItemExist("BtnTimb"))
                            {
                                SAPbouiCOM.Item mBtnTimbrado = this.mFrmInvoice.Items.Add("BtnTimb", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                                mBtnTimbrado.Top = this.mFrmInvoice.Items.Item("2").Top;
                                mBtnTimbrado.Left = this.mFrmInvoice.Items.Item("2").Left + 83;
                                mBtnTimbrado.FromPane = 0;
                                mBtnTimbrado.ToPane = 0;
                                mBtnTimbrado.LinkTo = "2";
                                mBtnTimbrado.Width = 145;
                                mBtnTimbrado.Height = 22;
                                mBtnTimbrado.Enabled = true;
                                (mBtnTimbrado.Specific as SAPbouiCOM.Button).Caption = "Timbrar Documento";
                                //(mBtnTimbrado.Specific as SAPbouiCOM.Button).ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(mItmBtnCard_ClickBefore);
                            }
                            break;
                        case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE:
                            if (!ItemExist("BtnTimb"))
                                return;

                            if (mFrmInvoice == null)
                                return;

                            SAPbouiCOM.Button mBtnTimbrado2 = (SAPbouiCOM.Button)mFrmInvoice.Items.Item("BtnTimb").Specific;
                            if (mBtnTimbrado2 != null)
                            {
                                //mBtnTimbrado = this.mFrmInvoice.Items.Add("BtnTimb", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                                mBtnTimbrado2.Item.Top = this.mFrmInvoice.Items.Item("2").Top;
                                mBtnTimbrado2.Item.Left = this.mFrmInvoice.Items.Item("2").Left + 83;
                                mBtnTimbrado2.Item.FromPane = 0;
                                mBtnTimbrado2.Item.ToPane = 0;
                                mBtnTimbrado2.Item.LinkTo = "2";
                                /*mBtnTimbrado.Width = 145;
                                mBtnTimbrado.Height = 22;
                                mBtnTimbrado.Enabled = false;
                                (mBtnTimbrado.Specific as SAPbouiCOM.Button).Caption = "Timbrar Documento";
                                (mBtnTimbrado.Specific as SAPbouiCOM.Button).ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(mItmBtnCard_ClickBefore);*/
                            }
                            break;
                        case SAPbouiCOM.BoEventTypes.et_FORM_ACTIVATE:
                            mFrmInvoice = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);
                            break;
                        case SAPbouiCOM.BoEventTypes.et_CLICK:
                            if (pVal.ItemUID == "BtnTimb")
                            {
                                TimbrarDocumento();
                            }
                            break;
                        case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST:
                            break;
                        case SAPbouiCOM.BoEventTypes.et_FORMAT_SEARCH_COMPLETED:
                            break;
                        case SAPbouiCOM.BoEventTypes.et_VALIDATE:
                            break;
                        case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                            //UIApplication.GetApplication().ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
                            //UIApplication.GetApplication().MenuEvent -= new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(SBO_Application_ApplicationMenuEvent);
                            //UIApplication.GetApplication().FormDataEvent -= new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(SBO_Application_DataFormEvent);
                            break;
                    }
                }
            }
            catch (Exception pObjException)
            {
                LogUtility.WriteError(string.Format("[Timbrado - SBO_Application_ItemEvent] Error: {0}", pObjException.Message));
                UIApplication.ShowError(string.Format("ItemEvent: {0}", pObjException.Message));
            }
        }

        /// <summary>
        /// Menu event handler
        /// </summary>
        /// <param name="pVal"></param>
        /// <param name="BubbleEvent"></param>
        private void SBO_Application_ApplicationMenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                if (!pVal.BeforeAction)
                {
                    if (UIApplication.GetApplication().Forms.ActiveForm.Type < 0)
                    {
                        this.mFrmInvoice = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(UIApplication.GetApplication().Forms.ActiveForm.Type * (-1), UIApplication.GetApplication().Forms.ActiveForm.TypeCount);
                    }
                    else
                    {
                        this.mFrmInvoice = UIApplication.GetApplication().Forms.ActiveForm;
                    }

                    /*if (UIApplication.GetApplication().Forms.ActiveForm.UniqueID != mFrmInvoice.UniqueID)
                        return;*/

                    switch (pVal.MenuUID)
                    {
                        case "1281": // Search Record
                            //AddTimbradoButton();
                            //if (mBtnTimbrado != null)
                            //{
                            //    //mBtnTimbrado.Visible = false;
                            //    mBtnTimbrado.Enabled = false;
                            //}
                            mBolFindMode = true;
                            break;

                        case "1282": // Add New Record
                            AddTimbradoButton();
                            break;

                        case "1288": // Next Record
                            AddTimbradoButton();
                            break;

                        case "1289": // Pevious Record
                            AddTimbradoButton();
                            break;

                        case "1290": // First Record
                            AddTimbradoButton();
                            break;

                        case "1291": // Last record
                            AddTimbradoButton();
                            break;

                        case "1287": // Duplicate Document
                            AddTimbradoButton();
                            break;
                        case "1304": // Refresh record
                            AddTimbradoButton();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.WriteError(string.Format("[Timbrado - SBO_Application_ApplicationMenuEvent] Error: {0}", ex.Message));
                UIApplication.ShowError(string.Format("MenuEventException: {0}", ex.Message));
            }
            finally
            {
                //this.UIAPIRawForm.Freeze(false);
            }
        }

        private void SBO_Application_DataFormEvent(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                //if (BusinessObjectInfo.BeforeAction)
                //    return;

                //if (UIApplication.GetApplication().Forms.ActiveForm.Type < 0)
                //{
                //    this.mFrmInvoice = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(UIApplication.GetApplication().Forms.ActiveForm.Type * (-1), UIApplication.GetApplication().Forms.ActiveForm.TypeCount);
                //}
                //else
                //{
                //    this.mFrmInvoice = UIApplication.GetApplication().Forms.ActiveForm;
                //}

                //if ((this.mFrmInvoice.BusinessObject.Type == "13"
                //|| this.mFrmInvoice.BusinessObject.Type == "14"
                //|| this.mFrmInvoice.BusinessObject.Type == "24"
                //)
                //&& BusinessObjectInfo.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD
                //&& BusinessObjectInfo.ActionSuccess)
                //{
                //    this.mStrObjType = this.mFrmInvoice.BusinessObject.Type;
                //    this.mStrDocEntry = GetDocEntry(this.mFrmInvoice.BusinessObject.Key);

                //    if (mBolFindMode)
                //    {
                //        AddTimbradoButton();
                //        mBolFindMode = false;
                //    }
                //    //AddTimbradoButton();
                //}
            }
            catch (Exception ex)
            {
                LogUtility.WriteError(string.Format("[Timbrado - SBO_Application_DataFormEvent] Error: {0}", ex.Message));
                UIApplication.ShowError(string.Format("DataFormEvent: {0}", ex.Message));
            }

            //SAPbouiCOM.Form lObjForm = UIApplication.GetApplication().Forms.ActiveForm;
            //if (lObjForm.Type < 0)
            //{
            //    lObjForm = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(lObjForm.Type * (-1), lObjForm.TypeCount);
            //}

            //if (!BusinessObjectInfo.BeforeAction)
            //{
            //    if (this.mFrmInvoice.Type != 133 /*&& lObjForm.Type != 14 && lObjForm.Type != 24*/)
            //        return;

            //    this.mStrObjType = BusinessObjectInfo.Type;
            //    this.mStrDocEntry = GetDocEntry(BusinessObjectInfo.ObjectKey);
            //    this.mStrDocNum = ((SAPbouiCOM.EditText)this.mFrmInvoice.Items.Item("8").Specific).Value;

            //    if (mBolFindMode)
            //    {
            //        AddTimbradoButton();
            //        mBolFindMode = false;
            //    }

            //    //if (BusinessObjectInfo.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD)
            //    //{
            //    //    AddTimbradoButton();
            //    //}

            //    //TODO: Habilitar/Deshabilitar botón cuando no esté cargado un documento válido...
            //}
        }

        private void TimbrarDocumento()
        {
            try
            {
                //if (lObjForm.Type < 0)
                //{
                //    lObjForm = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(lObjForm.Type * (-1), lObjForm.TypeCount);
                //}

                if (UIApplication.GetApplication().MessageBox("¿Esta seguro de timbrar el Documento?", 1, "Aceptar", "Cancelar", "") != 1)
                    return;

                if (this.mFrmInvoice.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                {
                    UIApplication.ShowError("No puede timbrar un Documento que no este creado");
                    return;
                }


                //this.mFrmInvoice = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(pVal., pVal.FormTypeCount);

                this.mFrmInvoice.Freeze(true);

                this.mStrObjType = this.mFrmInvoice.BusinessObject.Type;
                this.mStrDocEntry = GetDocEntry(this.mFrmInvoice.BusinessObject.Key);
                this.mStrDocNum = ((SAPbouiCOM.EditText)this.mFrmInvoice.Items.Item("8").Specific).Value;

                if (String.IsNullOrEmpty(this.mStrDocEntry) && String.IsNullOrEmpty(this.mStrObjType))
                    return;

                var lObjFactura = mObjTimbradoFactory.GetTimbradoCFDi().GetFacturaPorTimbrar(int.Parse(this.mStrDocEntry));
                if (lObjFactura == null)
                {
                    UIApplication.ShowMessageBox(string.Format("Factura {0} ya timbrada o no válida para timbrado", this.mStrDocNum));
                    return;
                }
                else if (lObjFactura.Timbrado != 0)
                {
                    UIApplication.ShowMessageBox(string.Format("La factura {0} ya se encuentra timbrada", this.mStrDocNum));
                    return;
                }

                string lStrResult = TimbrarDocumento(int.Parse(this.mStrDocEntry), int.Parse(this.mStrObjType));
                if (!string.IsNullOrEmpty(lStrResult))
                {
                    if (lStrResult == "OK")
                    {
                        UIApplication.ShowSuccess(string.Format("Documento con DocNum {0} timbrado", this.mStrDocNum));
                        LogUtility.WriteSuccess(string.Format("Documento con DocEntry {0} timbrado", this.mStrDocEntry));
                    }
                    else
                    {
                        UIApplication.ShowError(lStrResult);
                        LogUtility.WriteError(lStrResult);
                    }

                    if (!UIApplication.GetApplication().Menus.Item("6913").Checked)
                    {
                        UIApplication.GetApplication().ActivateMenuItem("6913");//2050
                    }
                    UIApplication.GetApplication().ActivateMenuItem("1304");
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.WriteError(string.Format("[Timbrado - mItmBtnCard_ClickBefore] Error al timbrar el documento con DocEntry {0}: {1}", this.mStrDocEntry, lObjException.Message));
                UIApplication.ShowMessageBox(string.Format("Error al timbrar el documento: {0}", lObjException.Message));
            }
            finally
            {
                //mBtnTimbrado.Enabled = true;
                this.mFrmInvoice.Freeze(false);
            }
        }

        private void mItmBtnCard_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            //SAPbouiCOM.Form lObjForm = UIApplication.GetApplication().Forms.ActiveForm;
            try
            {
                //if (lObjForm.Type < 0)
                //{
                //    lObjForm = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(lObjForm.Type * (-1), lObjForm.TypeCount);
                //}

                if (UIApplication.GetApplication().MessageBox("¿Esta seguro de timbrar el Documento?", 1, "Aceptar", "Cancelar", "") != 1)
                    return;

                if (this.mFrmInvoice.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                {
                    UIApplication.ShowError("No puede timbrar un Documento que no este creado");
                    return;
                }


                //this.mFrmInvoice = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(pVal., pVal.FormTypeCount);

                this.mFrmInvoice.Freeze(true);

                this.mStrObjType = this.mFrmInvoice.BusinessObject.Type;
                this.mStrDocEntry = GetDocEntry(this.mFrmInvoice.BusinessObject.Key);
                this.mStrDocNum = ((SAPbouiCOM.EditText)this.mFrmInvoice.Items.Item("8").Specific).Value;

                if (String.IsNullOrEmpty(this.mStrDocEntry) && String.IsNullOrEmpty(this.mStrObjType))
                    return;

                var lObjFactura = mObjTimbradoFactory.GetTimbradoCFDi().GetFacturaPorTimbrar(int.Parse(this.mStrDocEntry));
                if (lObjFactura == null)
                {
                    UIApplication.ShowMessageBox(string.Format("Factura {0} ya timbrada o no válida para timbrado", this.mStrDocNum));
                    return;
                }
                else if (lObjFactura.Timbrado != 0)
                {
                    UIApplication.ShowMessageBox(string.Format("La factura {0} ya se encuentra timbrada", this.mStrDocNum));
                    return;
                }

                string lStrResult = TimbrarDocumento(int.Parse(this.mStrDocEntry), int.Parse(this.mStrObjType));
                if (!string.IsNullOrEmpty(lStrResult))
                {
                    if (lStrResult == "OK")
                    {
                        UIApplication.ShowSuccess(string.Format("Documento con DocNum {0} timbrado", this.mStrDocNum));
                        LogUtility.WriteSuccess(string.Format("Documento con DocEntry {0} timbrado", this.mStrDocEntry));
                    }
                    else
                    {
                        UIApplication.ShowError(lStrResult);
                        LogUtility.WriteError(lStrResult);
                    }

                    if (!UIApplication.GetApplication().Menus.Item("6913").Checked)
                    {
                        UIApplication.GetApplication().ActivateMenuItem("6913");//2050
                    }
                    UIApplication.GetApplication().ActivateMenuItem("1304");
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.WriteError(string.Format("[Timbrado - mItmBtnCard_ClickBefore] Error al timbrar el documento con DocEntry {0}: {1}", this.mStrDocEntry, lObjException.Message));
                UIApplication.ShowMessageBox(string.Format("Error al timbrar el documento: {0}", lObjException.Message));
            }
            finally
            {
                //mBtnTimbrado.Enabled = true;
                this.mFrmInvoice.Freeze(false);
            }
        }
        #endregion

        public void AddDefaultButton()
        {
            //mBtnTimbrado = this.mFrmInvoice.Items.Add("BtnTimb", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
            //mBtnTimbrado.Top = this.mFrmInvoice.Items.Item("2").Top;
            //mBtnTimbrado.Left = this.mFrmInvoice.Items.Item("2").Left + 83;
            //mBtnTimbrado.FromPane = 0;
            //mBtnTimbrado.ToPane = 0;
            //mBtnTimbrado.Width = 145;
            //mBtnTimbrado.Height = 22;
            //mBtnTimbrado.Enabled = false;
            //(mBtnTimbrado.Specific as SAPbouiCOM.Button).Caption = "Timbrar Documento";
            //(mBtnTimbrado.Specific as SAPbouiCOM.Button).ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(mItmBtnCard_ClickBefore);
        }

        /// <summary>
        /// lee el xml que se obtiene por sap para obtener el doc entry
        /// </summary>
        /// <param name="pStrObjectKey"></param>
        /// <returns></returns>
        private string GetDocEntry(string pStrObjectKey)
        {
            string lStrDocEntry = string.Empty;
            XmlDocument lObjDocument = new XmlDocument();
            lObjDocument.LoadXml(pStrObjectKey);

            foreach (XmlElement item in lObjDocument.ChildNodes.Item(1).ChildNodes)
            {
                lStrDocEntry = item.InnerText;
            }

            return lStrDocEntry;
        }

        /// verificar si existe un item dentro del form
        private bool ItemExist(string pStrItemName)
        {
            try
            {
                //UIApplication.GetApplication().Forms.ActiveForm.Items.Item(pStrItemName);
                mFrmInvoice.Items.Item(pStrItemName);
                //if (!mBtnTimbrado.Enabled) //mBtnTimbrado.Visible
                //    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string TimbrarDocumento(int pIntDocEntry, int pIntDocType)
        {
            try
            {
                string lStrResult = string.Empty;

                lStrResult = mObjTimbradoFactory.GetTimbradoCFDi().TimbrarXMLs(pIntDocEntry);

                return lStrResult;
            }
            catch (Exception pObjException)
            {
                throw pObjException;
            }
        }
    }
}
