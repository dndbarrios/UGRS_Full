// file:	Exceptions\RowNotFoundException.cs
// summary:	Implements the row not found exception class

using System;
using System.Runtime.Serialization;

namespace Core.SDK.DI.Exceptions
{
    /// <summary> (Serializable) exception for signalling row not found errors. </summary>
    /// <remarks> Ranaya, 08/05/2017. </remarks>

    [Serializable]
    public class RowNotFoundException : Exception
    {
        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>

        public RowNotFoundException()
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pStrMessage"> The exception message. </param>
        /// <returns> A Tuple. </returns>

        public RowNotFoundException(string pStrMessage)
            : base(pStrMessage)
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pStrMessage"> The exception message. </param>
        /// <param name="pObjException"> The exception object. </param>
        /// <returns> A Tuple. </returns>

        public RowNotFoundException(string pStrMessage, Exception pObjException)
            : base(pStrMessage, pObjException)
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pObjSerializationInfo"> Information describing the object serialization. </param>
        /// <param name="pObjContext"> Context for the object. </param>

        protected RowNotFoundException(SerializationInfo pObjSerializationInfo, StreamingContext pObjContext)
        {
        }
    }
}
