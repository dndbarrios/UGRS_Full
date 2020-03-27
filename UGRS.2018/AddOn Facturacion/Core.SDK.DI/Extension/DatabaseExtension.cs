using System;
using System.IO;

namespace Core.SDK.DI.Extension
{
    public static class DatabaseExtension
    {
        private static string mStrDatabaseType = null;

        public static string GetSQL(this Object pObjCurrentObject, string pStrResource)
        {
            if (mStrDatabaseType == null)
            {
                mStrDatabaseType = (DIApplication.Company.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB) ? "HANA" : "SQL";
            }

            Type lObjBaseType = (typeof(Type).IsAssignableFrom(pObjCurrentObject.GetType())) ? (Type)pObjCurrentObject : pObjCurrentObject.GetType();

            if (lObjBaseType.Assembly.IsDynamic)
                lObjBaseType = lObjBaseType.BaseType;

            string lStrNamespace = lObjBaseType.Namespace;
           
           
            using (var lObjStream = lObjBaseType.Assembly.GetManifestResourceStream(lStrNamespace + "." + mStrDatabaseType + "." + pStrResource + ".sql"))
            {
                if (lObjStream != null)
                {
                    using (var lObjStreamReader = new StreamReader(lObjStream))
                    {
                        return lObjStreamReader.ReadToEnd();
                    }
                }
            }
            return string.Empty;
        }
    }
}
