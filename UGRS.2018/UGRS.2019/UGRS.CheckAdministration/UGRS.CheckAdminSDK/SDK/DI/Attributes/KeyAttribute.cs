
using System;

namespace UGRS.CheckAdminSDK.SDK.DI.Attributes {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KeyAttribute : Attribute {
        public KeyAttribute() {

        }
    }
}
