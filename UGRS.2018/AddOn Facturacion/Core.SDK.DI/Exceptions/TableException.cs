// file:	Exceptions\TableException.cs
// summary:	Implements the table exception class

using System;
using System.Runtime.Serialization;

namespace Core.SDK.DI.Exceptions
{
    /// <summary> (Serializable) exception for signalling table errors. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    [Serializable]
    public class TableException : Exception
    {
        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>

        public TableException()
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pStrMessage"> The exception message. </param>
        /// <returns> A Tuple. </returns>

        public TableException(string pStrMessage)
            : base(pStrMessage)
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pStrMessage"> The exception message. </param>
        /// <param name="pObjException"> The exception object. </param>
        /// <returns> A Tuple. </returns>

        public TableException(string pStrMessage, Exception pObjException)
            : base(pStrMessage, pObjException)
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pObjSerializationInfo"> Information describing the object serialization. </param>
        /// <param name="pObjContext"> Context for the object. </param>

        protected TableException(SerializationInfo pObjSerializationInfo, StreamingContext pObjContext)
        {
        }
    }
}
