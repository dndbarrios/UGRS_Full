// file:	Models\IObject.cs
// summary:	Declares the IObject interface

using SAPbobsCOM;
using System.Collections.Generic;
using Core.SDK.Attributes;

namespace Core.SDK.DI.Models
{
    /// <summary> Interface for object. </summary>
    /// <remarks> Ranaya, 02/05/2017. </remarks>

    public interface IObject
    {
        /// <summary> Gets the attributes. </summary>
        /// <returns> The attributes. </returns>

        ObjectAttribute GetAttributes();

        /// <summary> Gets the key. </summary>
        /// <returns> The key. </returns>

        string GetKey();

        /// <summary> Gets the name. </summary>
        /// <returns> The name. </returns>

        string GetName();

        /// <summary> Gets user object. </summary>
        /// <returns> The user object. </returns>

        UserObjectsMD GetUserObject();

        /// <summary> Gets a child tables. </summary>
        /// <returns> The child tables. </returns>

        IList<string> GetChildTables();

        /// <summary> Gets user table. </summary>
        /// <returns> The user table. </returns>

        UserTablesMD GetUserTable();

        /// <summary> Gets user fields. </summary>
        /// <returns> The user fields. </returns>

        IList<Field> GetFields();
    }
}
