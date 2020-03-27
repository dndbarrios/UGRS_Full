// file:	Exceptions\ObjectException.cs
// summary:	Implements the object exception class

using System;
using System.Runtime.Serialization;

namespace Core.SDK.DI.Exceptions
{
    /// <summary> (Serializable) exception for signalling object errors. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    [Serializable]
    public class ObjectException : Exception
    {
        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>

        public ObjectException()
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pStrMessage"> The exception message. </param>
        /// <returns> A Tuple. </returns>

        public ObjectException(string pStrMessage)
            : base(pStrMessage)
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pStrMessage"> The exception message. </param>
        /// <param name="pObjException"> The exception object. </param>
        /// <returns> A Tuple. </returns>

        public ObjectException(string pStrMessage, Exception pObjException)
            : base(pStrMessage, pObjException)
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pObjSerializationInfo"> Information describing the object serialization. </param>
        /// <param name="pObjContext"> Context for the object. </param>

        protected ObjectException(SerializationInfo pObjSerializationInfo, StreamingContext pObjContext)
        {
        }
    }
}
