// file:	models\field.cs
// summary:	Implements the field class

using SAPbobsCOM;
using System.Linq;
using System.Reflection;
using UGRS.CheckAdminSDK.SDK.DI.Attributes;

namespace UGRS.CheckAdminSDK.SDK.DI.DAO.Models {

    public class Field : Interfaces.IField {

        public string TableName { get; set; }

        private FieldAttribute mObjAttributes;

        public FieldAttribute GetAttributes() {
            return mObjAttributes;
        }

        public Field(string pStrTableName, PropertyInfo pObjProperty, bool pBolUserTable) {
            TableName = pBolUserTable ? string.Format("@{0}", pStrTableName) : pStrTableName;
            mObjAttributes = GetFieldAttributes(pObjProperty);
            mObjAttributes.Name = !string.IsNullOrEmpty(mObjAttributes.Name) ? mObjAttributes.Name : pObjProperty.Name;
            mObjAttributes.Description = !string.IsNullOrEmpty(mObjAttributes.Description) ? mObjAttributes.Description : pObjProperty.Name;
        }


        public UserFieldsMD GetUserField() {
            SAPbobsCOM.UserFieldsMD lObjUserField = null;
            lObjUserField = (SAPbobsCOM.UserFieldsMD)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);

            lObjUserField.TableName = TableName;
            lObjUserField.Name = GetAttributes().Name;
            lObjUserField.Description = GetAttributes().Description;
            lObjUserField.Type = GetAttributes().Type;
            lObjUserField.SubType = GetAttributes().SubType;
            lObjUserField.Size = GetAttributes().Size;
            lObjUserField.EditSize = GetAttributes().Type == BoFieldTypes.db_Alpha ? GetAttributes().Size : GetAttributes().SubSize;

            if (!string.IsNullOrEmpty(GetAttributes().LinkedTable)) {
                lObjUserField.LinkedTable = GetAttributes().LinkedTable;
            }

            if (!string.IsNullOrEmpty(GetAttributes().LinkedUDO)) {
                lObjUserField.LinkedUDO = GetAttributes().LinkedUDO;
            }

            return lObjUserField;
        }

        private FieldAttribute GetFieldAttributes(PropertyInfo property) => property.GetCustomAttributes(typeof(FieldAttribute), true).FirstOrDefault() as FieldAttribute;


    }
}
