// file:	Models\ITable.cs
// summary:	Declares the ITable interface

using SAPbobsCOM;
using System.Collections.Generic;
using Core.SDK.Attributes;

namespace Core.SDK.DI.Models
{
    /// <summary> Interface for table. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    public interface ITable
    {
        /// <summary> Gets the attributes. </summary>
        /// <returns> The attributes. </returns>

        TableAttribute GetAttributes();

        /// <summary> Gets the key. </summary>
        /// <returns> The key. </returns>

        string GetKey();

        /// <summary> Gets the name. </summary>
        /// <returns> The name. </returns>

        string GetName();

        /// <summary> Gets user table. </summary>
        /// <returns> The user table. </returns>

        UserTablesMD GetUserTable();

        /// <summary> Gets user fields. </summary>
        /// <returns> The user fields. </returns>

        IList<Field> GetFields();
    }
}
