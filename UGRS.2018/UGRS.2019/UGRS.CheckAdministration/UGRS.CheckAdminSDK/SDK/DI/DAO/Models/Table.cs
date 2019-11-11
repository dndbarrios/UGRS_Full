// file:	Models\Table.cs
// summary:	Implements the table class


using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UGRS.CheckAdminSDK.SDK.DI.Attributes;
using UGRS.CheckAdminSDK.SDK.DI.DAO.Interfaces;
using UGRS.Core.Extensions;

namespace UGRS.CheckAdminSDK.SDK.DI.DAO.Models {
    public class Table : ITable {

        [Key]
        public virtual string RowCode { get; set; }

        [Name]
        public virtual string RowName { get; set; }

        public TableAttribute GetAttributes() => GetTableAttributes();


        public string GetKey() {

            var value = this.GetPropertyValueByAttribute<KeyAttribute>();
            return value != null ? value.ToString() : string.Empty;
        }

        public string GetName() {
            var value = this.GetPropertyValueByAttribute<NameAttribute>();
            return value != null ? value.ToString() : string.Empty;
        }

        public SAPbobsCOM.UserTablesMD GetUserTable() {

            SAPbobsCOM.UserTablesMD userTable = null;
            userTable = (SAPbobsCOM.UserTablesMD)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
            userTable.TableName = GetAttributes().Name;
            userTable.TableDescription = GetAttributes().Description;
            userTable.TableType = GetAttributes().Type;

            return userTable;
        }

        public List<Field> GetFields() {

            List<Field> fields = new List<Field>();

            foreach (PropertyInfo property in this.GetType().GetProperties()) {
                if (property.GetMethod.IsPublic && !property.GetGetMethod().IsVirtual) {
                    fields.Add(new Field(GetAttributes().Name, property, true));
                }
            }
            return fields;
        }

        private TableAttribute GetTableAttributes() => this.GetType().GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;
        private FieldAttribute GetFieldAttributes(PropertyInfo pObjProperty) => pObjProperty.GetCustomAttributes(typeof(FieldAttribute), true).FirstOrDefault() as FieldAttribute;

    }
}
