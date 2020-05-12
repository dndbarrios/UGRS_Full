// file:	DAO\ITableDAO.cs
// summary:	Declares the ITableDAO interface

using System.Collections.Generic;
using Core.SDK.DI.Models;

namespace Core.SDK.DI.DAO
{
    /// <summary> Interface for table DAO. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>
    /// <typeparam name="T"> Generic type parameter. </typeparam>

    public interface ITableDAO<T> where T : ITable
    {
        // <summary> Adds or updates a record. </summary>
        /// <param name="pObjRecord"> The Object record to add or update. </param>
        /// <returns> An int. </returns>

        int Add(T pObjRecord);

        /// <summary> Updates the given pObjRecord. </summary>
        /// <param name="pObjRecord"> The Object record to add or update. </param>
        /// <returns> An int. </returns>

        int Update(T pObjRecord);

        /// <summary> Removes the record with docentry given. </summary>
        /// <param name="pStrDocEntry"> The String Document entry to remove. </param>
        /// <returns> An int. </returns>

        int Remove(string pStrDocEntry);
    }
}
