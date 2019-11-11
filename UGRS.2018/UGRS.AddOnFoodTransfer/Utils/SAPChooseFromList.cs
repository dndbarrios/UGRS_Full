/*
 * Autor: LCC Abraham SaÚL Sandoval Meneses
 * Description: SAP B1 Exception
 * Date: 11/09/2018
 */

using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.SDK.DI.FoodTransfer.DAO;
using UGRS.Core.SDK.UI;


namespace UGRS.AddOnFoodTransfer.Utils {

    public class SAPChooseFromList {

        /// <summary>
        /// Initialize choose from list by the given id
        /// </summary>
        /// <param name="isMultiSelection"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="frm"></param>
        /// <returns></returns>
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
            catch(Exception ex) {
                UIApplication.ShowMessageBox(String.Format("InitCustomerChooseFromListException: {0}", ex.Message));
            }
            return lObjoCFL;
        }

        /// <summary>
        /// Get the choose from list event value by the given position (column)
        /// </summary>
        /// <param name="oValEvent"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static string GetValue(ItemEvent oValEvent, int position) {

            DataTable dataTable = null;

            if(oValEvent.Action_Success) {

                IChooseFromListEvent oCFLEvento = (IChooseFromListEvent)oValEvent;
                dataTable = oCFLEvento.SelectedObjects;

                if(oCFLEvento.SelectedObjects == null)
                    return String.Empty;
            }
            return Convert.ToString(dataTable.GetValue(position, 0));
        }

        public static string GetValue(ItemEvent oValEvent, string column) {

            DataTable dataTable = null;

            if(oValEvent.Action_Success) {

                IChooseFromListEvent oCFLEvento = (IChooseFromListEvent)oValEvent;
                dataTable = oCFLEvento.SelectedObjects;

                if(oCFLEvento.SelectedObjects == null)
                    return String.Empty;
            }
            return Convert.ToString(dataTable.GetValue(column, 0));
        }

        /// <summary>
        /// Add conditions to the choose from list
        /// </summary>
        /// <param name="oChooseFromList"></param>
        /// <param name="conditions"></param>
        public static void AddConditions(ChooseFromList oChooseFromList, Dictionary<string, string> conditions) {

            Condition oCondition = null;
            Conditions oConditions = new Conditions();

            for(int i = 0; i < conditions.Count; i++) {
                oCondition = oConditions.Add();
                oCondition.Alias = conditions.ElementAt(i).Key;
                oCondition.Operation = BoConditionOperation.co_EQUAL;
                oCondition.CondVal = conditions.ElementAt(i).Value;

                if(i < conditions.Count - 1) {
                    oCondition.Relationship = BoConditionRelationship.cr_AND;
                }

            }

            oChooseFromList.SetConditions(oConditions);
        }

        public static void AddConditions2(ChooseFromList oChooseFromList, Dictionary<string, string> conditions) {

            Condition oCondition = null;
            Conditions oConditions = new Conditions();

            for(int i = 0; i < conditions.Count; i++) {
                oCondition = oConditions.Add();
                oCondition.Alias = conditions.ElementAt(i).Key;
                oCondition.Operation = BoConditionOperation.co_EQUAL;
                oCondition.CondVal = conditions.ElementAt(i).Value;

                if(i < conditions.Count - 1) {
                    oCondition.Relationship = BoConditionRelationship.cr_AND;
                }
            }

            oCondition.Relationship = BoConditionRelationship.cr_OR;

            var conditions2 = new Dictionary<string, string>() { { "Series", new FoodTransferDAO().GetSeries("CUAP", "59", "Series").ToString() }, { "U_GLO_InMo", "E-PROD" }, { "U_GLO_Status", "O" } };

            for(int i = 0; i < conditions2.Count; i++) {
                oCondition = oConditions.Add();
                oCondition.Alias = conditions2.ElementAt(i).Key;
                oCondition.Operation = BoConditionOperation.co_EQUAL;
                oCondition.CondVal = conditions2.ElementAt(i).Value;

                if(i < conditions2.Count - 1) {
                    oCondition.Relationship = BoConditionRelationship.cr_AND;
                }
            }

            oChooseFromList.SetConditions(oConditions);
        }

        /// <summary>
        /// Add conditions to choose from list by given field's values
        /// </summary>
        /// <param name="oChooseFromList"></param>
        /// <param name="fieldAlias"></param>
        /// <param name="values"></param>
        public static void AddConditionValues(ChooseFromList oChooseFromList, string fieldAlias, string[] values) {

            Condition oCondition = null;
            Conditions oConditions = new Conditions();

            try {

                if(values != null && values.Length > 0) {
                    for(int i = 1; i < values.Length + 1; i++) {
                        oCondition = oConditions.Add();
                        oCondition.Alias = fieldAlias;
                        oCondition.Operation = BoConditionOperation.co_EQUAL;
                        oCondition.CondVal = values[i - 1];

                        if(values.Length > i) {
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
            catch(Exception ex) {
                SAPException.Handle(ex, "AddConditionValues");
            }
        }

        /// <summary>
        /// Bind TextBox to ChooseFromList
        /// </summary>
        /// <param name="id"></param>
        /// <param name="txt"></param>
        public static void Bind(string id, EditText txt) {
            txt.DataBind.SetBound(true, "", id);
            txt.ChooseFromListUID = id;
        }
    }
}

