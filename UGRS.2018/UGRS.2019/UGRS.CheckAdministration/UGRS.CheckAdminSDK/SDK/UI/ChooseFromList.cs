using UGRS.Core.Utility;
using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;


namespace UGRS.CheckAdminSDK.SDK.UI {

    public class UIChooseFromList {
        public static ChooseFromList Init(bool isMultiSelection, string type, string id, FormBase frm) {

            ChooseFromList lObjoCFL = null;

            try {
                ChooseFromListCreationParams oCFLCreationParams = null;
                oCFLCreationParams = (ChooseFromListCreationParams)UIApplication.GetApplication().CreateObject(BoCreatableObjectType.cot_ChooseFromListCreationParams);
                oCFLCreationParams.MultiSelection = isMultiSelection;
                oCFLCreationParams.ObjectType = type;
                oCFLCreationParams.UniqueID = id;

                lObjoCFL = frm.UIAPIRawForm.ChooseFromLists.Add(oCFLCreationParams);
                frm.UIAPIRawForm.DataSources.UserDataSources.Add(id, BoDataType.dt_SHORT_TEXT, 254);
            }
            catch (Exception ex) {
                UIApplication.ShowMessageBox(String.Format("InitCustomerChooseFromListException: {0}", ex.Message));
                LogEntry.WriteException(ex);
            }
            return lObjoCFL;
        }
        public static string GetValue(ItemEvent oValEvent, int position) {

            DataTable dataTable = null;

            if (oValEvent.Action_Success) {

                IChooseFromListEvent oCFLEvento = (IChooseFromListEvent)oValEvent;
                dataTable = oCFLEvento.SelectedObjects;

                if (oCFLEvento.SelectedObjects == null)
                    return String.Empty;
            }
            return Convert.ToString(dataTable.GetValue(position, 0));
        }

        public static string GetValue(ItemEvent oValEvent, string column) {

            DataTable dataTable = null;

            if (oValEvent.Action_Success) {

                IChooseFromListEvent oCFLEvento = (IChooseFromListEvent)oValEvent;
                dataTable = oCFLEvento.SelectedObjects;

                if (oCFLEvento.SelectedObjects == null)
                    return String.Empty;
            }
            return Convert.ToString(dataTable.GetValue(column, 0));
        }

        public static void AddConditions(ChooseFromList oChooseFromList, Dictionary<string, string> conditions) {

            Condition oCondition = null;
            Conditions oConditions = new Conditions();

            for (int i = 0; i < conditions.Count; i++) {
                oCondition = oConditions.Add();
                oCondition.Alias = conditions.ElementAt(i).Key;
                oCondition.Operation = BoConditionOperation.co_EQUAL;
                oCondition.CondVal = conditions.ElementAt(i).Value;

                if (i < conditions.Count - 1) {
                    oCondition.Relationship = BoConditionRelationship.cr_AND;
                }

            }

            oChooseFromList.SetConditions(oConditions);
        }


        public static void AddConditionValues(ChooseFromList oChooseFromList, string fieldAlias, string[] values) {

            Condition oCondition = null;
            Conditions oConditions = new Conditions();

            try {

                if (values != null && values.Length > 0) {
                    for (int i = 1; i < values.Length + 1; i++) {
                        oCondition = oConditions.Add();
                        oCondition.Alias = fieldAlias;
                        oCondition.Operation = BoConditionOperation.co_EQUAL;
                        oCondition.CondVal = values[i - 1];

                        if (values.Length > i) {
                            oCondition.Relationship = SAPbouiCOM.BoConditionRelationship.cr_OR;
                        }
                    }
                }
                else {
                    oCondition = oConditions.Add();
                    oCondition.Alias = fieldAlias;
                    oCondition.Operation = BoConditionOperation.co_EQUAL;
                    oCondition.CondVal = "none";

                }
                oChooseFromList.SetConditions(oConditions);

            }
            catch (Exception ex) {
                LogEntry.WriteException(ex);
            }
        }


        public static void Bind(string id, EditText txt) {
            txt.DataBind.SetBound(true, "", id);
            txt.ChooseFromListUID = id;
        }
    }
}


