// file:	Exceptions\DAOException.cs
// summary:	Implements the DAO exception class

using System;
using System.Runtime.Serialization;

namespace Core.Exceptions
{
    /// <summary> (Serializable) exception for signalling DAO errors. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    [Serializable]
    public class DAOException : Exception
    {
        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>

        public DAOException()
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="parameter1"> The exception message. </param>
        /// <returns> A Tuple. </returns>

        public DAOException(string pStrMessage)
            : base(pStrMessage)
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="parameter1"> The exception message. </param>
        /// <param name="parameter2"> The exception object. </param>
        /// <returns> A Tuple. </returns>

        public DAOException(string pStrMessage, Exception pObjException)
            : base(pStrMessage, pObjException)
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pObjSerializationInfo"> Information describing the object serialization. </param>
        /// <param name="pObjContext">           Context for the object. </param>

        protected DAOException(SerializationInfo pObjSerializationInfo, StreamingContext pObjContext)
        {
        }
    }
}
