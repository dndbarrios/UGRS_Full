// file:	Exceptions\HandleException.cs
// summary:	Implements the handle exception class

using Core.Exceptions;

namespace Core.SDK.DI.Exceptions
{
    /// <summary> Exception for signalling handle errors. </summary>
    /// <remarks> Ranaya, 05/05/2017. </remarks>

    public class HandleException
    {
        /// <summary> Tables. </summary>
        /// <remarks> Ranaya, 05/05/2017. </remarks>
        /// <exception cref="ObjectException"> Thrown when an Object error condition occurs. </exception>
        /// <param name="pIntCode"> The int code. </param>

        public static void Table(int pIntCode)
        {
            if (pIntCode != 0)
            {
                throw new ObjectException(string.Format("Error code: {0} \nError message: {1}", pIntCode, DIApplication.Company.GetLastErrorDescription()));
            }
        }

        /// <summary> Objects. </summary>
        /// <remarks> Ranaya, 05/05/2017. </remarks>
        /// <exception cref="TableException"> Thrown when a Table error condition occurs. </exception>
        /// <param name="pIntCode"> The int code. </param>

        public static void Object(int pIntCode)
        {
            if (pIntCode != 0)
            {
                throw new TableException(string.Format("Error code: {0} \nError message: {1}", pIntCode, DIApplication.Company.GetLastErrorDescription()));
            }
        }

        /// <summary> Fields. </summary>
        /// <remarks> Ranaya, 05/05/2017. </remarks>
        /// <exception cref="FieldException"> Thrown when a Field error condition occurs. </exception>
        /// <param name="pIntCode"> The int code. </param>

        public static void Field(int pIntCode)
        {
            if (pIntCode != 0)
            {
                throw new FieldException(string.Format("Error code: {0} \nError message: {1}", pIntCode, DIApplication.Company.GetLastErrorDescription()));
            }
        }

        /// <summary> Fields. </summary>
        /// <remarks> Ranaya, 05/05/2017. </remarks>
        /// <exception cref="DAOException"> Thrown when a DAO error condition occurs. </exception>
        /// <param name="pIntCode"> The int code. </param>

        public static void DAO(int pIntCode)
        {
            if (pIntCode != 0)
            {
                throw new DAOException(string.Format("Error code: {0} \nError message: {1}", pIntCode, DIApplication.Company.GetLastErrorDescription()));
            }
        }

        /// <summary> SapBo. </summary>
        /// <remarks> Ranaya, 05/05/2017. </remarks>
        /// <exception cref="SapBoException"> Thrown when a SapBo error condition occurs. </exception>
        /// <param name="pIntCode"> The int code. </param>

        public static void SapBo(int pIntCode)
        {
            if (pIntCode != 0)
            {
                throw new SapBoException(string.Format("Error code: {0} \nError message: {1}", pIntCode, DIApplication.Company.GetLastErrorDescription()));
            }
        }
    }
}
