// file:	Exceptions\UIConnectionException.cs
// summary:	Implements the UI connection exception class

using System;
using System.Runtime.Serialization;

namespace Core.SDK.Exceptions
{
    /// <summary> (Serializable) exception for signalling UI connection errors. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    [Serializable]
    public class UIConnectionException : Exception
    {
        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>

        public UIConnectionException()
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pStrMessage"> The exception message. </param>
        /// <returns> A Tuple. </returns>

        public UIConnectionException(string pStrMessage)
            : base(pStrMessage)
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pStrMessage"> The exception message. </param>
        /// <param name="pObjException"> The exception object. </param>
        /// <returns> A Tuple. </returns>

        public UIConnectionException(string pStrMessage, Exception pObjException)
            : base(pStrMessage, pObjException)
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pObjSerializationInfo"> Information describing the object serialization. </param>
        /// <param name="pObjContext"> Context for the object. </param>

        protected UIConnectionException(SerializationInfo pObjSerializationInfo, StreamingContext pObjContext)
        {
        }
    }
}
