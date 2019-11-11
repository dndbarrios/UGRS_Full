using System;
using System.IO;
using System.Text;

namespace UGRS.Core.Extensions {
    public static class DatabaseExtension {
        private static string dbType = null;
        public static string GetSQL(this Object @object, string resource) {

            if (dbType == null) {
                dbType = "SQL";
            }

            var baseType = (typeof(Type).IsAssignableFrom(@object.GetType())) ? (Type)@object : @object.GetType();

            if (baseType.Assembly.IsDynamic)
                baseType = baseType.BaseType;

            using (var stream = baseType.Assembly.GetManifestResourceStream(baseType.Namespace + "." + dbType + "." + resource + ".sql")) {
                if (stream != null) {
                    using (var streamReader = new StreamReader(stream, Encoding.Default)) {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            return string.Empty;
        }
    }
}

