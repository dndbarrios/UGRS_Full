using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.UI;
using UGRS.Core.Services;

namespace UGRS.AddOn.EmailSender
{
    public class EmailSenderController
    {
        #region Attributes
        private SAPbouiCOM.Form FormEmail = null;
        #endregion

        #region Constructor
        public EmailSenderController()
        {
            UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }
        #endregion

        #region Events
        /// <summary>
        /// Manejador de eventos dentro de la pantalla de envío de correos de SAP.
        /// </summary>
        /// <param name="FormUID"></param>
        /// <param name="pVal"></param>
        /// <param name="BubbleEvent"></param>
        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (!pVal.FormTypeEx.Equals("188"))
                    return;

                if (!pVal.BeforeAction)
                    return;

                string typeExPO = string.Empty;
                switch (pVal.EventType)
                {
                    case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                        //mBoolFormLoaded = true;

                        //this.FormEmail = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);
                        //AddEmailRecipient();

                        break;
                    case SAPbouiCOM.BoEventTypes.et_FORM_DRAW: //et_FORM_ACTIVATE
                        typeExPO = UIApplication.GetApplication().Forms.ActiveForm.TypeEx;
                        if (typeExPO == "142" || typeExPO == "-142") //PO Form
                        {
                            this.FormEmail = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);
                            AddEmailRecipient();
                        }
                        break;
                    case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                        //SAPbouiCOM.Form formPO = UIApplication.GetApplication().Forms.GetForm("142", pVal.FormTypeCount);
                        //formPO.Items.Item("16").Click();
                        break;
                }
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
                LogService.WriteError(String.Format("[EmailSenderController - SBO_Application_ItemEvent] - {0}", lObjException.Message));
                UIApplication.ShowError(lObjException.Message);
            }

        }
        #endregion

        #region Functions
        private bool isParentForm(string pStrFormTypeEx)
        {
            bool lBolResult = false;
            string lStrTypeEx = string.Empty;
            foreach (SAPbouiCOM.Form form in UIApplication.GetApplication().Forms)
            {
                if (form.TypeEx == "188")
                {
                    if (pStrFormTypeEx == lStrTypeEx)
                    {
                        return true;
                    }
                }

                lStrTypeEx = form.TypeEx;
            }

            return lBolResult;
        }

        private void AddEmailRecipient()
        {
            string lStrEmail = new QueryManager().GetValue("U_Email", "USER_CODE", DIApplication.Company.UserName, "OUSR");
            try
            {
                if (this.FormEmail == null)
                    return;

                if (string.IsNullOrEmpty(lStrEmail))
                    return;

                //Matrix Recipients
                SAPbouiCOM.Matrix mtxRecipients = ((SAPbouiCOM.Matrix)this.FormEmail.Items.Item("3").Specific);
                if (!mtxRecipients.Item.Enabled)
                    return;

                int visualRowCount = mtxRecipients.VisualRowCount;

                //if (visualRowCount > 0)
                //{
                //    mtxRecipients.Columns.Item("1").Cells.Item(visualRowCount).Click(SAPbouiCOM.BoCellClickType.ct_Right);
                //}

                UIApplication.GetApplication().ActivateMenuItem("1292");

                ((SAPbouiCOM.CheckBox)mtxRecipients.Columns.Item("3").Cells.Item(mtxRecipients.VisualRowCount).Specific).Checked = true;
                ((SAPbouiCOM.EditText)mtxRecipients.Columns.Item("4").Cells.Item(mtxRecipients.VisualRowCount).Specific).Value = lStrEmail;
                ((SAPbouiCOM.EditText)mtxRecipients.Columns.Item("1").Cells.Item(mtxRecipients.VisualRowCount).Specific).Value = DIApplication.Company.UserName;
            }
            catch (Exception lObjException)
            {
                if (lObjException.Message.Contains("Form item is not editable") || lObjException.Message.Contains("Cannot activate disabled menu item") || lObjException.Message.Contains("Invalid index"))
                {
                    LogService.WriteError(string.Format("Error al agregar el valor {0} a la columna Hasta: {1}", DIApplication.Company.UserName, lObjException.Message));
                    throw new Exception(string.Format("Error al agregar el valor {0} a la columna Hasta: {1}", DIApplication.Company.UserName, lObjException.Message));
                    //AddEmailRecipientDefault(lStrEmail);
                }
                else
                {
                    LogService.WriteError(string.Format("Error al agregar el correo del usuario {0} como destinatario: {1}", DIApplication.Company.UserName, lObjException.Message));
                    throw new Exception(string.Format("Error al agregar el correo del usuario {0} como destinatario: {1}", DIApplication.Company.UserName, lObjException.Message));
                }
            }
        }

        private void AddEmailRecipientDefault(string pStrEmail)
        {
            try
            {
                //Matrix Recipients
                SAPbouiCOM.Matrix mtxRecipients = ((SAPbouiCOM.Matrix)this.FormEmail.Items.Item("3").Specific);
                if (!mtxRecipients.Item.Enabled)
                    return;

                mtxRecipients.AddRow();
                int visualRowCount = mtxRecipients.VisualRowCount;

                bool value = ((SAPbouiCOM.CheckBox)mtxRecipients.Columns.Item("3").Cells.Item(mtxRecipients.VisualRowCount).Specific).Item.Enabled;
                bool value2 = mtxRecipients.Columns.Item("3").Editable;
                ((SAPbouiCOM.CheckBox)mtxRecipients.Columns.Item("3").Cells.Item(mtxRecipients.VisualRowCount).Specific).Checked = true;
                ((SAPbouiCOM.EditText)mtxRecipients.Columns.Item("4").Cells.Item(mtxRecipients.VisualRowCount).Specific).Value = pStrEmail;
                ((SAPbouiCOM.EditText)mtxRecipients.Columns.Item("1").Cells.Item(mtxRecipients.VisualRowCount).Specific).Value = DIApplication.Company.UserName;
            }
            catch (Exception lObjException)
            {
                if (lObjException.Message.Contains("Item - Form item is not editable"))
                {
                    LogService.WriteError(string.Format("Error al agregar el valor {0} a la columna Hasta: {1}", DIApplication.Company.UserName, lObjException.Message));
                }
                else
                {
                    LogService.WriteError(string.Format("Error al agregar el correo del usuario {0} como destinatario: {1}", DIApplication.Company.UserName, lObjException.Message));
                    //throw new Exception(string.Format("Error al agregar el correo del usuario {0} como destinatario: {1}", DIApplication.Company.UserName, lObjException.Message));
                }
            }
        }
        #endregion
    }
}
