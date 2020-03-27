// file:	Utility\MemoryUtility.cs
// summary:	Implements the memory utility class

using System.Runtime.InteropServices;

namespace Core.Utility
{
    /// <summary>
    /// A memory utility.
    /// </summary>
    /// <remarks>
    /// Ranaya, 26/05/2017.
    /// </remarks>

    public class MemoryUtility
    {
        /// <summary>
        /// Releases the com object described by pArrObjComObject.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <param name="pObjComObject">
        /// The object com object.
        /// </param>

        public static void ReleaseComObject(object pObjComObject)
        {
            if (pObjComObject == null)
                return;

            Marshal.ReleaseComObject(pObjComObject);
            Marshal.FinalReleaseComObject(pObjComObject);

            pObjComObject = (object)null;
        }

        /// <summary>
        /// Releases the com object described by pArrObjComObject.
        /// </summary>
        /// <remarks>
        /// Ranaya, 26/05/2017.
        /// </remarks>
        /// <param name="pArrObjComObject">
        /// A variable-length parameters list containing array object com object.
        /// </param>

        public static void ReleaseComObject(params object[] pArrObjComObject)
        {
            for (int index = 0; index < pArrObjComObject.Length; ++index)
            {
                if (pArrObjComObject[index] != null)
                {
                    Marshal.ReleaseComObject(pArrObjComObject[index]);
                    Marshal.FinalReleaseComObject(pArrObjComObject[index]);

                    pArrObjComObject[index] = (object)null;
                }
            }
        }
    }
}
