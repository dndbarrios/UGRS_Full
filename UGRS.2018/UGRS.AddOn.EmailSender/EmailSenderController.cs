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

                if (pVal.BeforeAction)
                    return;

                switch (pVal.EventType)
                {
                    case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                        //mBoolFormLoaded = true;

                        this.FormEmail = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);
                        AddEmailRecipient();

                        break;
                    case SAPbouiCOM.BoEventTypes.et_VALIDATE:

                        break;
                    case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:

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
        private void AddEmailRecipient()
        {
            try
            {
                if (this.FormEmail == null)
                    return;

                string lStrEmail = new QueryManager().GetValue("U_Email", "USER_CODE", DIApplication.Company.UserName, "OUSR");
                if (string.IsNullOrEmpty(lStrEmail))
                    return;

                //Matrix Recipients
                SAPbouiCOM.Matrix mtxRecipients = ((SAPbouiCOM.Matrix)this.FormEmail.Items.Item("3").Specific);
                if (!mtxRecipients.Item.Enabled)
                    return;

                mtxRecipients.AddRow();
                ((SAPbouiCOM.EditText)mtxRecipients.Columns.Item("1").Cells.Item(mtxRecipients.VisualRowCount).Specific).Value = DIApplication.Company.UserName;
                ((SAPbouiCOM.CheckBox)mtxRecipients.Columns.Item("3").Cells.Item(mtxRecipients.VisualRowCount).Specific).Checked = true;
                ((SAPbouiCOM.EditText)mtxRecipients.Columns.Item("4").Cells.Item(mtxRecipients.VisualRowCount).Specific).Value = lStrEmail;
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(string.Format("Error al agregar el correo del usuario {0} como destinatario: {1}", DIApplication.Company.UserName, lObjException.Message));
                throw new Exception(string.Format("Error al agregar el correo del usuario {0} como destinatario: {1}", DIApplication.Company.UserName, lObjException.Message));
            }
        }
        #endregion
    }
}
