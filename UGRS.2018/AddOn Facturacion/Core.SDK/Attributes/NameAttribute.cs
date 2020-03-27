// file:	Attributes\NameAttribute.cs
// summary:	Implements the name attribute class

using System;

namespace Core.SDK.Attributes
{
    /// <summary> Attribute for name. </summary>
    /// <remarks> Ranaya, 09/05/2017. </remarks>

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NameAttribute : Attribute
    {
        public NameAttribute()
        {

        }
    }
}
