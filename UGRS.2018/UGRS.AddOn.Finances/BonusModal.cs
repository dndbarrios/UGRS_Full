using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.AddOn.Finances.Utils;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Finances.DAO;
using UGRS.Core.SDK.DI.Finances.DTO;
using UGRS.Core.SDK.UI;
using UGRS.Core.Utility;

namespace UGRS.AddOn.Finances
{
    class BonusModal
    {
        private SAPbouiCOM.Company mObjCompany;
        //Invoice Form Items
        private SAPbouiCOM._IApplicationEvents_ItemEventEventHandler mObjEventHandler;
        private SAPbouiCOM.Form mFrmInvoice;
        private SAPbouiCOM.EditText mEdtDocNum;

        //new Invoice Form
        private SAPbouiCOM.Form mFrmCreditMemo;

        private SAPbouiCOM.Item mItmBtnBonus;

        // Modal object and elements
        private SAPbouiCOM.Form mFrmDialog;
        private SAPbouiCOM.ComboBox mCmbType;
        private SAPbouiCOM.EditText mEdtAmount;
        private SAPbouiCOM.Button mBtnGen;

        private DocumentDAO mInvoiceDAO = new DocumentDAO();

        private QueryManager mObjQueryManager = new QueryManager();
        private PaymentDAO mCreditMemoDao = new PaymentDAO();

        public BonusModal(SAPbouiCOM.Company pCompany, SAPbouiCOM.Form pForm)
        {
            mObjCompany = pCompany;
            mFrmInvoice = pForm;
            AddBonusButton();
        }

        /// <summary>
        /// Injects the Bonus to the SAP Form.
        /// </summary>
        private void AddBonusButton()
        {
            string lStrCopyId = "10000330";
            mFrmInvoice.Freeze(true);
            try
            {
                if (mItmBtnBonus == null)
                {
                    mItmBtnBonus = mFrmInvoice.Items.Add("btnBonus", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                    mItmBtnBonus.Top = mFrmInvoice.Items.Item(lStrCopyId).Top;
                    mItmBtnBonus.Left = mFrmInvoice.Items.Item(lStrCopyId).Left - 150;
                    mItmBtnBonus.Width = 150;
                    mItmBtnBonus.LinkTo = lStrCopyId;
                    (mItmBtnBonus.Specific as SAPbouiCOM.Button).Caption = "Devolución/Bonificación";
                    (mItmBtnBonus.Specific as SAPbouiCOM.Button).ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.mItmBtnBonus_ClickBefore);
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.WriteError(string.Format("[BonusModal - AddBonusButton] Error al agregar el botón de Bonificacion: {0}", lObjException.Message));
                UIApplication.ShowMessageBox(string.Format("Error al agregar el botón de Bonificación: {0}", lObjException.Message));
            }
            finally
            {
                mFrmInvoice.Freeze(false);
            }
        }

        /// <summary>
        /// Event for the Bonus button.
        /// </summary>
        private void mItmBtnBonus_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pval, out bool BubbleEvent)
        {
            BubbleEvent = true;
            LoadModal();
            BindElements();
            InitElements();

        }

        /// <summary>
        /// Loads and open the modal from the XML file
        /// </summary>
        private void LoadModal()
        {
            mFrmDialog = Application.SBO_Application.Forms.AddEx(XmlLoader.LoadFromXml("fmBonus"));
            mFrmDialog.Title = "Bonificación/Devolución";
            mFrmDialog.Top = (UIApplication.GetApplication().Desktop.Height - mFrmDialog.Height) / 2;
            mFrmDialog.Left = (UIApplication.GetApplication().Desktop.Width - mFrmDialog.Width) / 2;
            mFrmDialog.Visible = true;
        }

        /// <summary>
        /// Saves references to items in the modal
        /// </summary>
        private void BindElements()
        {
            mCmbType = ((SAPbouiCOM.ComboBox)mFrmDialog.Items.Item("cmbType").Specific);
            mEdtAmount = ((SAPbouiCOM.EditText)mFrmDialog.Items.Item("edtAmnt").Specific);
            mBtnGen = ((SAPbouiCOM.Button)mFrmDialog.Items.Item("btnGen").Specific);

            mEdtDocNum = mFrmInvoice.Items.Item("8").Specific as SAPbouiCOM.EditText;
        }

        /// <summary>
        /// Initializes elements in the form.
        /// </summary>
        private void InitElements()
        {
            mCmbType.Item.DisplayDesc = true;
            mCmbType.ValidValues.Add("NCB", "NC Bonificación");
            mCmbType.ValidValues.Add("NCD", "NC Devolución");
            mCmbType.ValidValues.Add("NC", "Nota de crédito");
            mCmbType.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.mCmbType_ComboSelectAfter);
            mEdtAmount.ValidateBefore += mEdtAmount_ValidateBefore;
            mBtnGen.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.mBtnGen_ClickBefore);
            ValidateButtonState();
        }

        /// <summary>
        /// Event called when the amount field is edited. Checks if the submit button shoul be enabled.
        /// </summary>
        void mEdtAmount_ValidateBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            ValidateButtonState();
        }

        /// <summary>
        /// Click event for the generate button. Creates the selected document if the conditions are met.
        /// </summary>
        private void mBtnGen_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (!(sboObject as SAPbouiCOM.Button).Item.Enabled)
            {
                return;
            }

            try
            {
                double lDblBonusAmount = Convert.ToDouble(mEdtAmount.Value.ToString());

                // Get the DocEntry of the currently opened document
                string lStrDocEntry = mObjQueryManager.GetValue("DocEntry", "DocNum", mEdtDocNum.Value, "OINV");
                if (lStrDocEntry == null || lStrDocEntry == "")
                {
                    UIApplication.ShowMessageBox("El documento actual no esta creado.");
                    return;
                }

                // Get the original document
                SAPbobsCOM.Documents lOriginal = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                lOriginal.GetByKey(Convert.ToInt32(lStrDocEntry));

                string lStrAmountVencido = (mFrmInvoice.Items.Item("33").Specific as SAPbouiCOM.EditText).Value;

                // Document has to be Open if it's a bonus invoice
                if (string.IsNullOrEmpty(lStrAmountVencido) && mCmbType.Value == "NCB") //lOriginal.DocumentStatus != SAPbobsCOM.BoStatus.bost_Open
                {
                    UIApplication.ShowMessageBox("El documento debe tener saldo vencido para crear una bonificación.");
                    return;
                }

                if (!string.IsNullOrEmpty(lStrAmountVencido) && mCmbType.Value == "NC") //lOriginal.DocumentStatus != SAPbobsCOM.BoStatus.bost_Close
                {
                    UIApplication.ShowMessageBox("El documento debe estar cerrado para crear una nota de crédito.");
                    return;
                }

                // Amount can't be higher than document's total
                if (lDblBonusAmount > (lOriginal.DocTotal - lOriginal.DownPaymentAmount)) //lOriginal.VatSum
                {
                    UIApplication.ShowMessageBox("La cantidad introducida no puede ser mayor que el valor del documento.");
                    return;
                }

                if (mCmbType.Value == "NCB")
                {
                    CreateBonusDraft(lOriginal, lDblBonusAmount);
                }
                if (mCmbType.Value == "NCD")
                {
                    string lStrStatus = mObjQueryManager.GetValue("DocStatus", "DocEntry", lStrDocEntry, "OINV");
                    if (lOriginal.Cancelled == SAPbobsCOM.BoYesNoEnum.tYES)
                    {
                        UIApplication.ShowError("El documento no debe estar cancelado para crear una NC Devolución");
                        return;
                    }

                    if (lStrStatus != "C")
                    {
                        UIApplication.ShowError("El documento debe estar cerrado para crear una NC Devolución");
                        return;
                    }

                    CreateReturnDraft(lOriginal);
                }
                if (mCmbType.Value == "NC")
                {
                    CreateBonusDraft(lOriginal, lOriginal.DocTotal - lOriginal.VatSum, true);
                }
                mFrmDialog.Close();
            }
            catch (Exception lObjException)
            {
                LogUtility.WriteError(string.Format("[BonusModal - mBtnGen_ClickBefore] Error al agregar la bonificacion: {0}", lObjException.Message));
                //UIApplication.ShowMessageBox(string.Format("Error al agregar la bonificacion: {0}", lObjException.Message));
            }

        }

        /// <summary>
        /// Creates and displays a Credit Memo draft.
        /// </summary>
        /// <param name="pOriginal">The original document the credit memo is based of.</param>
        /// <param name="pAmount">The value of the credit note.</param>
        /// <param name="pCreditNote">Whether it is a Credit Note or a Bonus Credit Note.</param>
        private void CreateBonusDraft(SAPbobsCOM.Documents pOriginal, double pAmount, bool pCreditNote = false)
        {
            try
            {
                // Get the document lines, grouped by tax code, and costing codes
                IList<InvoiceRowDTO> mLstObjInvoiceRows = mInvoiceDAO.GetInvoiceLinesByGroup(pOriginal.DocEntry);

                // Bonus Item Code
                string mStrBonusCode = mObjQueryManager.GetValue("U_VALUE", "Name", Constants.STR_ENTRY_BONUS, Constants.STR_CONFIG_TABLE);

                // Create new draft document
                SAPbobsCOM.Documents lDraft = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                lDraft.DocObjectCode = SAPbobsCOM.BoObjectTypes.oCreditNotes;
                lDraft.CardCode = pOriginal.CardCode;
                lDraft.GroupNumber = -1;
                lDraft.PaymentMethod = pOriginal.PaymentMethod;
                lDraft.DocCurrency = pOriginal.DocCurrency;
                lDraft.UserFields.Fields.Item("U_B1SYS_MainUsage").Value = pOriginal.UserFields.Fields.Item("U_B1SYS_MainUsage").Value;
                if (pCreditNote)
                {
                    lDraft.UserFields.Fields.Item("U_CO_TypeInvoice").Value = "NC";
                }
                else
                {
                    lDraft.UserFields.Fields.Item("U_CO_TypeInvoice").Value = "NCB";
                }
                // Create a Bonus entry for evrery group in the source document lines
                foreach (InvoiceRowDTO lObjInvoiceRow in mLstObjInvoiceRows)
                {
                    double lDblTaxRate = double.Parse(new QueryManager().GetValue("Rate", "Code", lObjInvoiceRow.TaxCode, "OSTA")) / 100;
                    double lDblUnitPrice = pOriginal.DocCurrency == "MXP" ? ((lObjInvoiceRow.LineTotal / (pOriginal.DocTotal - pOriginal.VatSum) * pAmount) / (1 + lDblTaxRate))
                        : ((lObjInvoiceRow.LineTotal / (pOriginal.DocTotalFc - pOriginal.VatSumFc) * pAmount) / (1 + lDblTaxRate));

                    lDraft.Lines.ItemCode = mStrBonusCode;
                    lDraft.Lines.Quantity = 1;
                    // The unit price is based on the proportion between the group's value and the document's total value
                    lDraft.Lines.UnitPrice = lDblUnitPrice;
                    lDraft.Lines.TaxCode = lObjInvoiceRow.TaxCode;
                    lDraft.Lines.CostingCode = lObjInvoiceRow.OcrCode;
                    lDraft.Lines.CostingCode2 = lObjInvoiceRow.OcrCode2;
                    lDraft.Lines.CostingCode3 = lObjInvoiceRow.OcrCode3;
                    lDraft.Lines.UserFields.Fields.Item("U_GLO_BagsBales").Value = lObjInvoiceRow.BagsBales;

                    lDraft.Lines.Add();
                }
                int intError = lDraft.Add();
                string lStrErrMsg;
                if (intError != 0)
                {
                    DIApplication.Company.GetLastError(out intError, out lStrErrMsg);
                    UIApplication.ShowError(lStrErrMsg);
                }
                else
                {
                    string lStrLastDoc = DIApplication.Company.GetNewObjectKey().ToString();
                    //Insert original document in references
                    mCreditMemoDao.InsertDocumentReference(Convert.ToInt32(lStrLastDoc), (int)SAPbobsCOM.BoObjectTypes.oCreditNotes, 1, pOriginal.DocEntry, pOriginal.DocNum, (int)pOriginal.DocObjectCode, "01", pOriginal.CreationDate.ToString("yyyyMMdd"));
                    // Openb Draft Form
                    Application.SBO_Application.OpenForm((SAPbouiCOM.BoFormObjectEnum)112, "", lStrLastDoc);
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.WriteError(string.Format("[BonusModal - CreateBonusDraft] Error al crear el borrador de la nota de crédito: {0}", lObjException.Message));
                UIApplication.ShowMessageBox(string.Format("Error al crear el borrador de la nota de crédito: {0}", lObjException.Message));
            }
        }

        /// <summary>
        /// Creates a Return Credit Note
        /// </summary>
        /// <param name="pOriginal">The original document the credit note is based of.</param>
        private void CreateReturnDraft(SAPbobsCOM.Documents pOriginal)
        {
            try
            {
                // Create new draft document
                SAPbobsCOM.Documents lDraft = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                lDraft.DocObjectCode = SAPbobsCOM.BoObjectTypes.oCreditNotes;
                lDraft.CardCode = pOriginal.CardCode;
                lDraft.GroupNumber = -1;
                lDraft.PaymentMethod = pOriginal.PaymentMethod;
                lDraft.UserFields.Fields.Item("U_B1SYS_MainUsage").Value = pOriginal.UserFields.Fields.Item("U_B1SYS_MainUsage").Value;
                lDraft.UserFields.Fields.Item("U_CO_TypeInvoice").Value = "NCD";
                for (int i = 0; i < pOriginal.Lines.Count; i++)
                {
                    pOriginal.Lines.SetCurrentLine(i);
                    lDraft.Lines.ItemCode = pOriginal.Lines.ItemCode;
                    lDraft.Lines.Quantity = pOriginal.Lines.Quantity;
                    lDraft.Lines.UnitPrice = pOriginal.Lines.UnitPrice;
                    lDraft.Lines.TaxCode = pOriginal.Lines.TaxCode;
                    lDraft.Lines.WarehouseCode = pOriginal.Lines.WarehouseCode;
                    lDraft.Lines.CostingCode = pOriginal.Lines.CostingCode;
                    lDraft.Lines.CostingCode2 = pOriginal.Lines.CostingCode2;
                    lDraft.Lines.CostingCode3 = pOriginal.Lines.CostingCode3;
                    lDraft.Lines.UserFields.Fields.Item("U_GLO_BagsBales").Value = pOriginal.Lines.UserFields.Fields.Item("U_GLO_BagsBales").Value;
                    lDraft.Lines.Add();
                }
                int intError = lDraft.Add();
                string lStrErrMsg;
                if (intError != 0)
                {
                    DIApplication.Company.GetLastError(out intError, out lStrErrMsg);
                    UIApplication.ShowError(lStrErrMsg);
                }
                else
                {
                    string lStrLastDoc = DIApplication.Company.GetNewObjectKey().ToString();
                    //Insert original document in references
                    mCreditMemoDao.InsertDocumentReference(Convert.ToInt32(lStrLastDoc), (int)SAPbobsCOM.BoObjectTypes.oCreditNotes, 1, pOriginal.DocEntry, pOriginal.DocNum, (int)pOriginal.DocObjectCode, "03", pOriginal.CreationDate.ToString("yyyyMMdd"));
                    Application.SBO_Application.OpenForm((SAPbouiCOM.BoFormObjectEnum)112, "", lStrLastDoc);
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.WriteError(string.Format("[BonusModal - CreateReturnDraft] Error al crear el borrador de la devolución de la nota de crédito: {0}", lObjException.Message));
                UIApplication.ShowMessageBox(string.Format("Error al crear el borrador de la devolución de la nota de crédito: {0}", lObjException.Message));
            }
        }

        /// <summary>
        /// Checks if the generate button should be eanbled or not.
        /// </summary>
        private void ValidateButtonState()
        {
            // Only enable button if a type is selected AND a value above zero is entered or the type is not a return.
            mBtnGen.Item.Enabled = mCmbType.Value != "" & (Convert.ToDouble(mEdtAmount.Value) > 0 || mCmbType.Value != "NCB");
            // Disable Amount field if type is not a Bonus.
            mEdtAmount.Item.Enabled = mCmbType.Value == "NCB";
        }

        /// <summary>
        /// Event for selecting a value from the combobox. Validates if the button should be enabled.
        /// </summary>
        private void mCmbType_ComboSelectAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            ValidateButtonState();
        }
    }
}
