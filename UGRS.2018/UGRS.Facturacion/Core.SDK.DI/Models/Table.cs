// file:	Models\Table.cs
// summary:	Implements the table class

using SAPbobsCOM;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Extension;
using Core.SDK.Attributes;

namespace Core.SDK.DI.Models
{
    /// <summary> A table. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    public class Table : ITable
    {
        /// <summary> Gets or sets the card code. </summary>
        /// <value> The card code. </value>

        [Key]
        public virtual string RowCode { get; set; }

        /// <summary> Gets or sets the name of the card. </summary>
        /// <value> The name of the card. </value>

        [Name]
        public virtual string RowName { get; set; }

        /// <summary> Gets the attributes. </summary>
        /// <remarks> Ranaya, 09/05/2017. </remarks>
        /// <returns> The attributes. </returns>

        public TableAttribute GetAttributes()
        {
            return GetTableAttributes();
        }

        /// <summary> Gets the key. </summary>
        /// <remarks> Ranaya, 18/05/2017. </remarks>
        /// <returns> The key. </returns>

        public string GetKey()
        {
            var lUnkPropertyValue = this.GetPropertyValueByAttribute<KeyAttribute>();
            return lUnkPropertyValue != null? lUnkPropertyValue.ToString() : string.Empty;
        }

        /// <summary> Gets the name. </summary>
        /// <remarks> Ranaya, 18/05/2017. </remarks>
        /// <returns> The name. </returns>

        public string GetName()
        {
            var lUnkPropertyValue = this.GetPropertyValueByAttribute<NameAttribute>();
            return lUnkPropertyValue != null ? lUnkPropertyValue.ToString() : string.Empty;
        }

        /// <summary> Gets user table. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>
        /// <returns> The user table. </returns>

        public UserTablesMD GetUserTable()
        {
            SAPbobsCOM.UserTablesMD lObjUserTable = null;
            lObjUserTable = (SAPbobsCOM.UserTablesMD)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);

            lObjUserTable.TableName = GetAttributes().Name;
            lObjUserTable.TableDescription = GetAttributes().Description;
            lObjUserTable.TableType = GetAttributes().Type;

            return lObjUserTable;
        }

        /// <summary> Gets user fields. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>
        /// <returns> The user fields. </returns>

        public IList<Field> GetFields()
        {
            IList<Field> lLstObjFields = new List<Field>();

            foreach (PropertyInfo lObjProperty in this.GetType().GetProperties())
            {
                if (lObjProperty.GetMethod.IsPublic && !lObjProperty.GetGetMethod().IsVirtual)
                {
                    lLstObjFields.Add(new Field(GetAttributes().Name, lObjProperty, true));
                }
            }

            return lLstObjFields;
        }

        /// <summary> Gets table attributes. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <returns> The table attributes. </returns>

        private TableAttribute GetTableAttributes()
        {
            return this.GetType().GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;
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
