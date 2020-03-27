using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Core.SDK.DI.Facturacion.Extension
{
    public class Memory
    {
        public static void ReleaseComObject(object ComObject)
        {
            if (ComObject == null)
                return;
            Marshal.ReleaseComObject(ComObject);
            Marshal.FinalReleaseComObject(ComObject);
            ComObject = (object)null;
        }
    }
}
