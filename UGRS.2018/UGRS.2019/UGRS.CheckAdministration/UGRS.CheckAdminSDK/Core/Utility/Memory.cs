using System.Runtime.InteropServices;

namespace UGRS.Core.Utility {
    public class MemoryUtility {



        public static void ReleaseComObject(object pObjComObject) {
            if (pObjComObject == null)
                return;

            Marshal.ReleaseComObject(pObjComObject);
            Marshal.FinalReleaseComObject(pObjComObject);


            pObjComObject = (object)null;
        }

        public static void ReleaseComObject(params object[] pArrObjComObject) {
            for (int index = 0; index < pArrObjComObject.Length; ++index) {
                if (pArrObjComObject[index] != null) {
                    Marshal.ReleaseComObject(pArrObjComObject[index]);
                    Marshal.FinalReleaseComObject(pArrObjComObject[index]);

                    pArrObjComObject[index] = (object)null;
                }
            }
        }
    }
}