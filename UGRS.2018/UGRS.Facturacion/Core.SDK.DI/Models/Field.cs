// file:	models\field.cs
// summary:	Implements the field class

using SAPbobsCOM;
using System.Linq;
using System.Reflection;
using Core.SDK.Attributes;

namespace Core.SDK.DI.Models
{
    /// <summary> A field. </summary>
    /// <remarks> Ranaya, 05/05/2017. </remarks>

    public class Field : IField
    {
        /// <summary> Gets or sets the name of the table. </summary>
        /// <value> The name of the table. </value>

        public string TableName {get; set;}

        private FieldAttribute mObjAttributes;

        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 09/05/2017. </remarks>

        public FieldAttribute GetAttributes()
        {
            return mObjAttributes;
        }

        /// <summary> Constructor. </summary>
        /// <remarks> Ranaya, 05/05/2017. </remarks>
        /// <param name="pObjProperty"> The object property. </param>

        public Field(string pStrTableName, PropertyInfo pObjProperty, bool pBolUserTable)
        {
            TableName = pBolUserTable ? string.Format("@{0}", pStrTableName) : pStrTableName;
            mObjAttributes = GetFieldAttributes(pObjProperty);
            mObjAttributes.Name = !string.IsNullOrEmpty(mObjAttributes.Name) ? mObjAttributes.Name : pObjProperty.Name;
            mObjAttributes.Description = !string.IsNullOrEmpty(mObjAttributes.Description) ? mObjAttributes.Description : pObjProperty.Name;
        }

        /// <summary> Gets user field. </summary>
        /// <remarks> Ranaya, 05/05/2017. </remarks>
        /// <returns> The user field. </returns>

        public UserFieldsMD GetUserField()
        {
            SAPbobsCOM.UserFieldsMD lObjUserField = null;
            lObjUserField = (SAPbobsCOM.UserFieldsMD)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);

            lObjUserField.TableName = TableName;
            lObjUserField.Name = GetAttributes().Name;
            lObjUserField.Description = GetAttributes().Description;
            lObjUserField.Type = GetAttributes().Type;
            lObjUserField.SubType = GetAttributes().SubType;
            lObjUserField.Size = GetAttributes().Size;
            lObjUserField.EditSize = GetAttributes().Type == BoFieldTypes.db_Alpha?  GetAttributes().Size : GetAttributes().SubSize;

            if (!string.IsNullOrEmpty(GetAttributes().LinkedTable))
            {
                lObjUserField.LinkedTable = GetAttributes().LinkedTable;
            }

            if (!string.IsNullOrEmpty(GetAttributes().LinkedUDO))
            {
                lObjUserField.LinkedUDO = GetAttributes().LinkedUDO;
            }

            return lObjUserField;
        }

        /// <summary> Gets field attributes. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pObjProperty"> The object property. </param>
        /// <returns> The field attributes. </returns>

        private FieldAttribute GetFieldAttributes(PropertyInfo pObjProperty)
        {
            return pObjProperty.GetCustomAttributes(typeof(FieldAttribute), true).FirstOrDefault() as FieldAttribute;
        }

    }
}
