using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.Extension
{
    public static class RecordSetExtension
    {
        public static DataTable ToDataTable(this SAPbobsCOM.Recordset pObjRecordSet)
        {
            DataTable lObjDataTable = new DataTable();
            DataColumn lObjNewColumn = null;
            DataRow lObjNewRow = null;

            try
            {
                //Add each field as column to data table
                for (int i = 0; i < pObjRecordSet.Fields.Count; i++)
                {
                    lObjNewColumn = new DataColumn(pObjRecordSet.Fields.Item(i).Name, GetSystemDataType(pObjRecordSet.Fields.Item(i).Type));
                    lObjDataTable.Columns.Add(lObjNewColumn);
                }

                //Add each record as row to data table
                do
                {
                    lObjNewRow = lObjDataTable.NewRow();

                    for (int i = 0; i < pObjRecordSet.Fields.Count; i++)
                    {
                        lObjNewRow[pObjRecordSet.Fields.Item(i).Name] = pObjRecordSet.Fields.Item(i).Value;
                    }

                    lObjDataTable.Rows.Add(lObjNewRow);
                    pObjRecordSet.MoveNext();
                }
                while (pObjRecordSet.EoF);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error converting SAP Recordset to DataTable:\n{0}", e.ToString()));
            }

            return lObjDataTable;
        }

        public static T GetTableObject<T>(this SAPbobsCOM.Recordset pObjRecordSet) where T : Table
        {
            T lObtResult = (T)Activator.CreateInstance(typeof(T));

            lObtResult.RowCode = pObjRecordSet.Fields.Item("Code").Value.ToString();
            lObtResult.RowName = pObjRecordSet.Fields.Item("Name").Value.ToString();

            foreach (PropertyInfo lObjProperty in lObtResult.GetType().GetProperties().Where(x => x.GetMethod.IsPublic && !x.GetMethod.IsVirtual))
            {
                string lStrFieldName = string.Format("U_{0}", lObjProperty.Name);
                Type lObjFieldType = lObjProperty.PropertyType;
                object lUnkFieldValue = pObjRecordSet.Fields.Item(lStrFieldName).Value;

                if (lObjFieldType == typeof(bool))
                {
                    lObjProperty.SetValue(lObtResult, lUnkFieldValue.ToString() == "Y" || lUnkFieldValue.ToString() == "S" ? true : false);
                }
                else
                {
                    lObjProperty.SetValue(lObtResult, Convert.ChangeType(lUnkFieldValue, lObjFieldType));
                }
            }

            return lObtResult;
        } 

        private static System.Type GetSystemDataType(SAPbobsCOM.BoFieldTypes pEnumFieldType)
        {
            string lStrDataType = "System.";

            switch (pEnumFieldType)
            {
                case SAPbobsCOM.BoFieldTypes.db_Alpha:
                    lStrDataType += "String";
                    break;

                case SAPbobsCOM.BoFieldTypes.db_Date:
                    lStrDataType += "DateTime";
                    break;

                case SAPbobsCOM.BoFieldTypes.db_Float:
                    lStrDataType += "Double";
                    break;

                case SAPbobsCOM.BoFieldTypes.db_Memo:
                    lStrDataType += "String";
                    break;

                case SAPbobsCOM.BoFieldTypes.db_Numeric:
                    lStrDataType += "Decimal";
                    break;

                default:
                    lStrDataType += "String";
                    break;
            }

            return System.Type.GetType(lStrDataType);
        }


        /// <summary>
        /// Gets the value of a column, converted to the desired value.
        /// </summary>
        /// <typeparam name="T">The type of the column.</typeparam>
        /// <param name="pRecordSet">The recordeset.</param>
        /// <param name="pColumn">The name of the column.</param>
        /// <returns>The value of the column.</returns>
        public static T GetColumnValue<T>(this SAPbobsCOM.Recordset pRecordSet, string pColumn) where T : IConvertible
        {
            object lObjValue = pRecordSet.Fields.Item(pColumn).Value;
            if (lObjValue == null)
            {
                return default(T);
            }
            return (T)Convert.ChangeType(lObjValue, typeof(T), CultureInfo.GetCultureInfo("en-US"));
        }
    }
}
