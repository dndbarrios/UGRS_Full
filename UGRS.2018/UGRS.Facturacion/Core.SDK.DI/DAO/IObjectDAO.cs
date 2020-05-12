// file:	dao\iobjectdao.cs
//
// summary:	Implements the iobjectdao class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.SDK.DI.Models;

namespace Core.SDK.DI.DAO
{
    /// <summary> Interface for object dao. </summary>
    /// <remarks> Ranaya, 26/05/2017. </remarks>
    /// <typeparam name="T"> Generic type parameter. </typeparam>

    public interface IObjectDAO<T> where T : IObject
    {
        // <summary> Adds or updates a record. </summary>

        /// <summary> Adds pObjRecord. </summary>
        /// <param name="pObjRecord"> The Object record to add or update. </param>

        void Add(T pObjRecord);

        /// <summary> Updates the given pObjRecord. </summary>
        /// <param name="pObjRecord"> The Object record to add or update. </param>

        void Update(T pObjRecord);

        /// <summary> Removes the record with docentry given. </summary>
        /// <param name="pStrDocEntry"> The String Document entry to remove. </param>

        void Remove(string pStrDocEntry);
    }
}
