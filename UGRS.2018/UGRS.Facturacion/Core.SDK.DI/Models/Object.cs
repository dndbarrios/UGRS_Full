// file:	Models\Object.cs
// summary:	Implements the object class

using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.SDK.Attributes;
using Core.Extension;

namespace Core.SDK.DI.Models
{
    /// <summary> An object. </summary>
    /// <remarks> Ranaya, 02/05/2017. </remarks>

    public class Object : IObject
    {
        /// <summary> Gets the attributes. </summary>
        /// <remarks> Ranaya, 09/05/2017. </remarks>
        /// <returns> The attributes. </returns>

        public ObjectAttribute GetAttributes()
        {
            return GetObjectAttributes();
        }

        /// <summary> Gets the key. </summary>
        /// <remarks> Ranaya, 18/05/2017. </remarks>
        /// <returns> The key. </returns>

        public string GetKey()
        {
            return this.GetPropertyValueByAttribute<KeyAttribute>().ToString();
        }

        /// <summary> Gets the name. </summary>
        /// <remarks> Ranaya, 18/05/2017. </remarks>
        /// <returns> The name. </returns>

        public string GetName()
        {
            return this.GetPropertyValueByAttribute<NameAttribute>().ToString();
        }

        /// <summary> Gets user object. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>
        /// <returns> The user object. </returns>

        public UserObjectsMD GetUserObject()
        {
            SAPbobsCOM.UserObjectsMD lObjUserObject = null;
            lObjUserObject = (SAPbobsCOM.UserObjectsMD)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);

            lObjUserObject.CanCancel = GetAttributes().CanCancel ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
            lObjUserObject.CanClose = GetAttributes().CanClose ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
            lObjUserObject.CanCreateDefaultForm = GetAttributes().CanCreateDefaultForm ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
            lObjUserObject.CanDelete = GetAttributes().CanDelete ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
            lObjUserObject.CanFind = GetAttributes().CanFind ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
            lObjUserObject.CanYearTransfer = GetAttributes().CanYearTransfer ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
            lObjUserObject.ManageSeries = GetAttributes().ManageSeries ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
            lObjUserObject.Code = GetAttributes().ObjectCode;
            lObjUserObject.Name = GetAttributes().ObjectDescription;
            lObjUserObject.ObjectType = GetAttributes().Type;
            lObjUserObject.TableName = GetAttributes().Name;

            foreach (string lStrChildTable in GetChildTables())
            {
                lObjUserObject.ChildTables.TableName = lStrChildTable;
                lObjUserObject.ChildTables.Add();
            }

            return lObjUserObject;
        }

        /// <summary> Gets a child tables. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>
        /// <returns> The child tables. </returns>

        public IList<string> GetChildTables()
        {
            IList<string> lLstStrChildTables = null;
            lLstStrChildTables = new List<string>();

            foreach (PropertyInfo lObjProperty in this.GetType().GetProperties(BindingFlags.Public))
            {
                if (lObjProperty.GetGetMethod().IsVirtual)
                {
                    FieldAttribute lObjAttribute = lObjProperty.GetCustomAttributes(typeof(FieldAttribute), true).FirstOrDefault() as FieldAttribute;
                    lLstStrChildTables.Add(lObjAttribute.Name);
                }
            }

            return lLstStrChildTables;
        }

        /// <summary> Gets user table. </summary>
        /// <remarks> Ranaya, 02/05/2017. </remarks>
        /// <returns> The user table. </returns>

        public UserTablesMD GetUserTable()
        {
            SAPbobsCOM.UserTablesMD lObjUserTable = null;
            lObjUserTable = (SAPbobsCOM.UserTablesMD)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);

            lObjUserTable.TableName = GetAttributes().Name;
            lObjUserTable.TableDescription = GetAttributes().Description;
            lObjUserTable.TableType = (SAPbobsCOM.BoUTBTableType)((int)GetAttributes().Type);

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
                if (!lObjProperty.GetMethod.IsPublic && !lObjProperty.GetGetMethod().IsVirtual)
                {
                    lLstObjFields.Add(new Field(GetName(), lObjProperty, true));
                }
            }

            return lLstObjFields;
        }

        /// <summary> Gets object attributes. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <returns> The object attributes. </returns>

        private ObjectAttribute GetObjectAttributes()
        {
            return this.GetType().GetCustomAttributes(typeof(ObjectAttribute), true).FirstOrDefault() as ObjectAttribute;
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
