using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.SDK.Attributes;

namespace Core.SDK.DI.Models
{
    public class SAPObject : ISAPObject
    {
        public SAPObjectAttribute GetAttributes()
        {
            return GetSAPObjectAttributes();
        }

        public IList<Field> GetFields()
        {
            IList<Field> lLstObjFields = new List<Field>();

            foreach (PropertyInfo lObjProperty in this.GetType().GetProperties())
            {
                if (lObjProperty.GetMethod.IsPublic && !lObjProperty.GetGetMethod().IsVirtual)
                {
                    lLstObjFields.Add(new Field(GetAttributes().TableName, lObjProperty, false));
                }
            }

            return lLstObjFields;
        }

        private SAPObjectAttribute GetSAPObjectAttributes()
        {
            return this.GetType().GetCustomAttributes(typeof(SAPObjectAttribute), true).FirstOrDefault() as SAPObjectAttribute;
        }
    }
}
