using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UGRS.CheckAdminSDK.Models;
using UGRS.CheckAdminSDK.SDK.Tables;
using UGRS.CheckAdminSDK.SDK.UI;
using UGRS.CheckAdminSDK.Services;
using UGRS.Core.Utility;

namespace CheckAdministration {
    [FormAttribute("CheckAdministration.Form1", "CheckAdminForm.b1f")]

    class CheckAdmonForm : UserFormBase {
        #region Properties
        Dictionary<string, BoFieldsType> columns;
        SapB1 sapB1 = new SapB1();
        User user;
        public static Object padlock = new Object();
        #endregion

        #region OnInitializeComponent
        public override void OnInitializeFormEvents() {
            this.CloseAfter += new SAPbouiCOM.Framework.FormBase.CloseAfterHandler(this.Form_CloseAfter);
            this.UnloadBefore += new UnloadBeforeHandler(this.Form_UnloadBefore);
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent += new _IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }
        private void OnCustomInitialize() { }
        public override void OnInitializeComponent() {
            this.lblStatus = ((SAPbouiCOM.StaticText)(this.GetItem("lblStatus").Specific));
            this.lblAccount = ((SAPbouiCOM.StaticText)(this.GetItem("lblAccount").Specific));
            this.lblFrom = ((SAPbouiCOM.StaticText)(this.GetItem("lblFrom").Specific));
            this.lblTo = ((SAPbouiCOM.StaticText)(this.GetItem("lblTo").Specific));
            this.lblCheck = ((SAPbouiCOM.StaticText)(this.GetItem("lblNumber").Specific));
            this.txtAcctNum = ((SAPbouiCOM.EditText)(this.GetItem("txtCheckAc").Specific));
            this.txtFrom = ((SAPbouiCOM.EditText)(this.GetItem("txtFrom").Specific));
            this.txtTo = ((SAPbouiCOM.EditText)(this.GetItem("txtTo").Specific));
            this.txtNumber = ((SAPbouiCOM.EditText)(this.GetItem("txtNumber").Specific));
            this.btnSearch = ((SAPbouiCOM.Button)(this.GetItem("btnSearch").Specific));
            this.btnSearch.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.BtnSearch_ClickBefore);
            this.mtx0 = ((SAPbouiCOM.Matrix)(this.GetItem("mtx0").Specific));
            this.mtx0.ClickAfter += new SAPbouiCOM._IMatrixEvents_ClickAfterEventHandler(this.mtx0_ClickAfter);
            this.btnSave = ((SAPbouiCOM.Button)(this.GetItem("btnSave").Specific));
            this.btnSave.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.BtnSave_ClickBefore);
            this.btnNotify = ((SAPbouiCOM.Button)(this.GetItem("btnNotify").Specific));
            this.btnAuto = ((SAPbouiCOM.Button)(this.GetItem("btnAuto").Specific));
            this.btnAuto.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.BtnAuto_ClickBefore);
            this.btnReject = ((SAPbouiCOM.Button)(this.GetItem("btnReject").Specific));
            this.btnReject.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.BtnReject_ClickBefore);
            this.btnClear = ((SAPbouiCOM.Button)(this.GetItem("btnClear").Specific));
            this.btnClear.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.BtnClear_ClickBefore);
            this.cbxStatus = ((SAPbouiCOM.ComboBox)(this.GetItem("cbxStatus").Specific));
            this.chkAll = ((SAPbouiCOM.CheckBox)(this.GetItem("chkAll").Specific));
            this.chkAll.ClickAfter += new SAPbouiCOM._ICheckBoxEvents_ClickAfterEventHandler(this.chkAll_ClickAfter);
            this.lblArea = ((SAPbouiCOM.StaticText)(this.GetItem("lblArea").Specific));
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_3").Specific));
            this.cbxArea = ((SAPbouiCOM.ComboBox)(this.GetItem("cbxArea").Specific));
            this.cbxArea.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cbxArea_ComboSelectAfter);
            this.cbxStatus2 = ((SAPbouiCOM.ComboBox)(this.GetItem("cbxStatus2").Specific));
            this.cbxStatus2.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cbxStatus2_ComboSelectAfter);
            this.OnCustomInitialize();

        }
        #endregion

        #region Form Controls
        private ComboBox cbxStatus;
        private StaticText lblStatus;
        private StaticText lblAccount;
        private StaticText lblFrom;
        private StaticText lblTo;
        private StaticText lblCheck;
        private EditText txtAcctNum;
        private EditText txtFrom;
        private EditText txtTo;
        private EditText txtNumber;
        private Button btnSearch;
        private Button btnSave;
        private Button btnNotify;
        private Button btnAuto;
        private Button btnReject;
        private Button btnClear;
        private Matrix mtx0;
        private DataTable dt0;
        private CheckBox chkAll;
        private StaticText lblArea;
        private StaticText StaticText1;
        private ComboBox cbxArea;
        private ComboBox cbxStatus2;

        #endregion

        #region Constructor
        public CheckAdmonForm() {
            user = new User();
            this.UIAPIRawForm.Title = $"{this.UIAPIRawForm.Title} - {user.Phase}";
            btnNotify.Item.Visible = false;
            InitMatrix();
            if (!user.IsAdmin) {
                mtx0.Columns.Item("C_Area").Editable = false;
                DisableComboBox();
            }
            EnableButtons(false);
            DisableComboBox();
            LoadChooseFromList("CFL_AC", "1", null, txtAcctNum);


        }
        #endregion


        private void LoadChooseFromList(string id, string code, Dictionary<string, string> conditions, EditText txt) {
            var oCFLFolio = UIChooseFromList.Init(false, code, id, this);
            if (!Object.ReferenceEquals(conditions, null) && conditions.Count > 0) {
                UIChooseFromList.AddConditions(oCFLFolio, conditions);
            }
            UIChooseFromList.Bind(id, txt);
        }

        #region Matrix
        private void InitMatrix() {
            columns = GetColumns();
            dt0 = UIMatrix.CreateDataTable("DT0", columns, this);
            AddMatrixValidaValues();

            this.UIAPIRawForm.Freeze(true);
            HideResultColumn(false);
            this.UIAPIRawForm.Freeze(false);
        }

        private void AddMatrixValidaValues() {
            var areas = GetAreas();
            var statusDics = GetStatusDic();
            var areasDict = GetAreasDic(areas);

            Parallel.Invoke(
                () => UIMatrix.AddColumnComboBoxValues(mtx0, "C_Area", areasDict),
                () => UIMatrix.AddColumnComboBoxValues(mtx0, "C_Status", statusDics)
            );

            foreach (var status in statusDics) {
                cbxStatus2.ValidValues.Add(status.Value, status.Key);
            }

            foreach (var area in areasDict) {
                cbxArea.ValidValues.Add(area.Value, area.Key);
            }
        }

        private int[] GetSelectedRowsFromMatrix(string condition, bool? autorized = null) {
            var selectedRows = UIMatrix.GetSelectedRowsCondition(mtx0, "C_Status", condition);

            if (user.IsBank && autorized == null) {
                selectedRows = selectedRows.Concat(UIMatrix.GetSelectedRowsCondition(mtx0, "C_Status", "Entregado a ARCHIVO")).ToArray();
            }
            return selectedRows;
        }

        private void BindResultColumn(List<Result> results) {

            Parallel.For(0, results.Count, i => {
                dt0.SetValue("C_Result", results[i].Row - 1, results[i].Msg);
            });

            this.UIAPIRawForm.Freeze(true);
            mtx0.LoadFromDataSource();
            HideResultColumn(true);
            this.UIAPIRawForm.Freeze(false);
            EnableButtons(false);
        }

        private void HideResultColumn(bool hidden) {
            mtx0.Columns.Item("C_Result").Visible = hidden;
            mtx0.AutoResizeColumns();
        }

        #endregion

        #region Events
        private void BtnClear_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            var selectedRows = UIMatrix.GetSelectedRows(mtx0);
            if (selectedRows.Length > 0) {
                UIMatrix.RemoveRowSelections(mtx0, selectedRows);
            }
        }

        private void BtnSave_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;

            if (btnSave.Item.Enabled)
                ProcessCheckAdmins($"Entregado a {user.DeliverTo}");
        }

        private void BtnAuto_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;

            if (btnAuto.Item.Enabled)
                ProcessCheckAdmins($"Pendiente de recibo {user.Phase}", true);
        }

        private void BtnReject_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            if (btnReject.Item.Enabled)
                ProcessCheckAdmins($"Pendiente de recibo {user.Phase}", false);
        }

        private void BtnSearch_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            Task.Factory.StartNew(() => SearchChecksData()).Wait();
        }
        #endregion

        #region Status Handlers
        private void ValidateStatusAndArea(int row) {
            if (row > 0 && !chkAll.Checked) {
                bool select;
                var area = UIMatrix.GetComboBoxValue(mtx0, "C_Area", row);
                var status = UIMatrix.GetComboBoxValue(mtx0, "C_Status", row);

                if (String.IsNullOrEmpty(area) || String.IsNullOrEmpty(status))
                    select = false;
                else if (user.IsBank && (status.Equals($"Pendiente de recibo {user.DeliverTo}") || status.Equals("Pendiente de recibo ARCHIVO")))
                    select = false;
                else if (!user.IsAdmin && status.Equals($"Pendiente de recibo {user.DeliverTo}"))
                    select = false;
                else if (user.IsArchive && status.Equals($"Entregado a {user.Phase}"))
                    select = false;
                else if (status.Contains("Rechazado"))
                    select = false;
                else
                    select = mtx0.IsRowSelected(row) ? false : true;

                mtx0.SelectRow(row, select, true);
            }
        }

        private string HandleStatus(string status, bool? autorized = null) {
            if (autorized == null && user.IsAdmin) {
                if (status.Equals("Entregado a AREA"))
                    status = "Pendiente de recibo AREA";

                else if (status.Equals("Entregado a ARCHIVO"))
                    status = "Pendiente de recibo ARCHIVO";

                return status;
            }

            if (autorized != null && !autorized.Value)
                return $"Rechazado por {user.Phase}";

            if (autorized != null && autorized.Value && !user.IsAdmin)
                return "Entregado a AREA";

            switch (status) {
                case "Entregado a BANCO":
                    status = "Pendiente de recibo BANCO";
                    break;
                case "Pendiente de recibo BANCO":
                    status = "Entregado a BANCO";
                    break;
                case "Pendiente de recibo ARCHIVO":
                    status = "Entregado a ARCHIVO";
                    break;
            }
            return status;
        }

        private string GetPhase(string status) {
            switch (status) {
                case "Pendiente de recibo AREA":
                    return "Cheque de Banco a Area";
                case "Pendiente de recibo BANCO":
                    return "Cheque de Area a Banco";
                case "Pendiente de recibo ARCHIVO":
                    return "Cheque de Banco a Archivo";
                case "Entregado a AREA":
                    return "Area autorizo a Banco";
                case "Entregado a BANCO":
                    return "Banco autorizo a Area";
                case "Entregado a ARCHIVO":
                    return "Archivo autorizo a Banco";
                default:
                    return status;
            }
        }
        #endregion

        #region Other Methods

        private void ProcessCheckAdmins(string condition = "", bool? autorized = null) {
            try {
                var timer = Stopwatch.StartNew();
                var selectedRows = GetSelectedRowsFromMatrix(condition, autorized);
                if (selectedRows.Length <= 0)
                    return;
                UIApplication.ShowSuccess("Procesando cheques");
                var results = new List<Task<Result>>();
                foreach (var row in selectedRows) {
                    results.Add(Task<Result>.Factory.StartNew((Object obj) => {
                        var checkAdmin = obj as CheckAdmin;
                        var result = new Result();
                        lock (padlock) {
                            checkAdmin.Phase = GetPhase(checkAdmin.Status);
                            result = SaveCheckAdmin(checkAdmin);
                            result.Row = row;
                        }
                        if (result.Success && autorized == null) {
                            SendAlertMessage(checkAdmin);
                        }
                        return result;
                    }, CreateCheckAdmin(row, user.IsAdmin, autorized))
                    );
                }

                BindResultColumn(results.Select(t => t.Result).ToList());
            }
            catch (Exception ex) {
                LogEntry.WriteException(ex);
            }
        }

        private void SendAlertMessage(CheckAdmin checkAdmin) {
            var userCode = "";
            switch (checkAdmin.Status) {
                case "Pendiente de recibo BANCO":
                    userCode = user.BankUserCode;
                    break;
                case "Pendiente de recibo AREA":
                    userCode = sapB1.DAO.GetUserToNotify(checkAdmin.Area);
                    break;
                case "Pendiente de recibo ARCHIVO":
                    userCode = user.ArchiveUserCode;
                    break;
            }

            if (!String.IsNullOrEmpty(userCode)) {
                sapB1.AlertMessage.Insert(userCode, $"Cheque Pendiente por Autorizar: {checkAdmin.CheckNum}");
            }
            else {
                LogEntry.WriteInfo("No se encontro al usuario para recibo de notificaciones");
            }
        }

        private Result SaveCheckAdmin(CheckAdmin checkAdmin) {
            var operation = GetCheckAdminOperation(checkAdmin.CheckNum);
            var result = operation.Invoke(checkAdmin);
            return result;
        }

        private CheckAdmin CreateCheckAdmin(int row, bool isAdmin, bool? autorized) {
            var number = dt0.GetValue("C_CheckNum", row - 1).ToString();
            var area = UIMatrix.GetComboBoxValue(mtx0, "C_Area", row);
            var status = HandleStatus(UIMatrix.GetComboBoxValue(mtx0, "C_Status", row), autorized);
            dt0.SetValue("C_Area", row - 1, area);
            dt0.SetValue("C_Status", row - 1, status);
            return new CheckAdmin(number, area, status);
        }

        private void SearchChecksData() {
            lock (padlock) {
                var checks = sapB1.DAO.GetChecks(Filter.BuildFilter(GetEditTexts(), cbxStatus), user);
                if (checks != null && checks.Length > 0) {
                    UIMatrix.Fill(mtx0, dt0, columns.Keys.ToList(), checks);

                    if (!user.IsArchive) {
                        btnSave.Item.Enabled = true;
                    }

                    btnReject.Item.Enabled = true;
                    btnAuto.Item.Enabled = true;
                    btnNotify.Item.Enabled = true;

                    HideResultColumn(false);
                }
                else {
                    EnableButtons(false);
                    UIMatrix.ClearMtx(mtx0);
                }

                if (user.IsBank) {
                    mtx0.Columns.Item("C_Area").Editable = true;
                    mtx0.Columns.Item("C_Status").Editable = true;
                }
                else if (!user.IsAdmin) {
                    cbxStatus2.Item.Enabled = true;
                }

                if (checks != null && checks.Length > 0) {
                    UIMatrix.EnableColumnCells(mtx0, 5, user.IsAdmin, user.IsArchive, checks.Where((c, i) => !String.IsNullOrEmpty(c.Status)).Select((c, i) => i + 1).ToArray());
                }
            }
        }

        private void EnableButtons(bool enable) {
            btnSave.Item.Enabled = enable;
            btnReject.Item.Enabled = enable;
            btnAuto.Item.Enabled = enable;
            btnNotify.Item.Enabled = enable;
        }
        private Func<CheckAdmin, Result> GetCheckAdminOperation(string number) {
            if (!sapB1.DAO.ExistCheck(number)) {
                return sapB1.CheckAdministration.Insert;
            }
            else {
                return sapB1.CheckAdministration.Update;
            }
        }

        private Dictionary<string, string> GetStatusDic() {
            if (user.IsBank)
                return new Dictionary<string, string>() { { "A", "Entregado a AREA" }, { "B", "Entregado a ARCHIVO" } };
            else
                return new Dictionary<string, string>() { { "C", "Entregado a BANCO" } };
        }

        private void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            try {
                if (this.UIAPIRawForm != null && FormUID.Equals(this.UIAPIRawForm.UniqueID)) {
                    if (!pVal.BeforeAction) {
                        switch (pVal.EventType) {
                            case BoEventTypes.et_CHOOSE_FROM_LIST:
                                AsignCFLValueToEditTxt(pVal);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex) {
                LogEntry.WriteException(ex);
            }
        }
        private void AsignCFLValueToEditTxt(ItemEvent pVal) => this.UIAPIRawForm.DataSources.UserDataSources.Item("CFL_AC").Value = UIChooseFromList.GetValue(pVal, 0);
        private string[] GetAreas() => sapB1.DAO.GetAreas().Distinct().AsParallel().ToArray();
        private void mtx0_ClickAfter(object sboObject, SBOItemEventArg pVal) => ValidateStatusAndArea(pVal.Row);
        private EditText[] GetEditTexts() => new EditText[] { txtFrom, txtTo, txtNumber, txtAcctNum };
        private Dictionary<string, string> GetAreasDic(string[] areas) => Enumerable.Range(0, areas.Length).AsParallel().ToDictionary(a => a.ToString(), a => areas[a]);
        private Dictionary<string, BoFieldsType> GetColumns() => new Dictionary<string, BoFieldsType>() { { "#", BoFieldsType.ft_ShortNumber }, { "CheckNum", BoFieldsType.ft_AlphaNumeric }, { "AcctName", BoFieldsType.ft_AlphaNumeric }, { "CheckAc", BoFieldsType.ft_AlphaNumeric }, { "Area", BoFieldsType.ft_AlphaNumeric }, { "Status", BoFieldsType.ft_AlphaNumeric }, { "Result", BoFieldsType.ft_AlphaNumeric } };
        #endregion

        private void Form_CloseAfter(SBOItemEventArg pVal) { }

        private void Form_UnloadBefore(SBOItemEventArg pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent -= new _IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }

        private void EnableComboBox() {
            cbxArea.Item.Enabled = true;
            cbxStatus2.Item.Enabled = true;
        }

        private void DisableComboBox() {
            cbxArea.Item.Enabled = false;
            cbxStatus2.Item.Enabled = false;
        }

        private void chkAll_ClickAfter(object sboObject, SBOItemEventArg pVal) {

            if (btnSave.Item.Enabled || user.IsArchive) {
                var check = sboObject as CheckBox;
                try {

                    if (mtx0.RowCount > 0) {
                        if (!check.Checked) {
                            this.UIAPIRawForm.Freeze(true);
                            UIApplication.ShowSuccess("Selecionando lineas");
                            UIMatrix.SelectAllRows(mtx0);
                            if (user.IsBank) {
                                EnableComboBox();
                            }
                            else if (!user.IsAdmin) {
                                cbxStatus2.Item.Enabled = true;
                            }
                        }
                        else {
                            var selectedRows = UIMatrix.GetSelectedRows(mtx0);
                            if (selectedRows.Length > 0) {
                                UIMatrix.RemoveRowSelections(mtx0, selectedRows);
                            }
                            cbxArea.Active = false;
                            cbxStatus2.Active = false;
                            DisableComboBox();
                        }
                    }
                }
                catch (Exception ex) {
                    UIApplication.ShowMessageBox(ex.Message);
                    LogEntry.WriteException(ex);
                }
                finally {
                    this.UIAPIRawForm.Freeze(false);
                }
            }
        }

        private void cbxArea_ComboSelectAfter(object sboObject, SBOItemEventArg pVal) {
            FillComboboxValues("C_Area", cbxArea.Value);
        }

        private void cbxStatus2_ComboSelectAfter(object sboObject, SBOItemEventArg pVal) {
            FillComboboxValues("C_Status", cbxStatus2.Value);
        }

        private void FillComboboxValues(string column, string value) {
            try {
                this.UIAPIRawForm.Freeze(true);
                UIMatrix.FillComboBoxColumn(mtx0, dt0, column, value);
                mtx0.AutoResizeColumns();
            }
            catch (Exception ex) {
                LogEntry.WriteException(ex);
                UIApplication.ShowMessageBox(ex.Message);

            }
            finally {
                this.UIAPIRawForm.Freeze(false);
            }
        }
    }
}
