// file:	Attributes\KeyAttribute.cs
// summary:	Implements the key attribute class

using System;

namespace Core.SDK.Attributes
{
    /// <summary> Attribute for key. </summary>
    /// <remarks> Ranaya, 09/05/2017. </remarks>

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KeyAttribute : Attribute
    {
        public KeyAttribute()
        {

        }
    }
}
