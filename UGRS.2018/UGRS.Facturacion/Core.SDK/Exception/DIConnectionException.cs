// file:	Exceptions\DIConnectionException.cs
// summary:	Implements the DI connection exception class

using System;
using System.Runtime.Serialization;

namespace Core.SDK.Exceptions
{
    /// <summary> (Serializable) exception for signalling DI connection errors. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    [Serializable]
    public class DIConnectionException : Exception
    {
        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>

        public DIConnectionException()
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pStrMessage"> The exception message. </param>
        /// <returns> A Tuple. </returns>

        public DIConnectionException(string pStrMessage)
            : base(pStrMessage)
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pStrMessage"> The exception message. </param>
        /// <param name="pObjException"> The exception object. </param>
        /// <returns> A Tuple. </returns>

        public DIConnectionException(string pStrMessage, Exception pObjException)
            : base(pStrMessage, pObjException)
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pObjSerializationInfo"> Information describing the object serialization. </param>
        /// <param name="pObjContext"> Context for the object. </param>

        protected DIConnectionException(SerializationInfo pObjSerializationInfo, StreamingContext pObjContext)
        {
        }
    }
}
