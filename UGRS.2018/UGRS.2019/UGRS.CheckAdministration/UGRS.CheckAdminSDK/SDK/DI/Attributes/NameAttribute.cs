using System;

namespace UGRS.CheckAdminSDK.SDK.DI.Attributes {

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NameAttribute : Attribute {
        public NameAttribute() { }
    }
}
