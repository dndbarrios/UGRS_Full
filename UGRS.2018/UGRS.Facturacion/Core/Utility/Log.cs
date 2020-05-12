using System;
using System.IO;
using System.Reflection;

namespace UGRS.Core.Utility
{
    public class Log
    {
        public static void Write(string pStrMessage)
        {
            string lStrApplicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");
            string lStrLogPath = Path.Combine(lStrApplicationPath, "Service.log");
            string lStrDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss ");

            try
            {
                using (StreamWriter lObjWriter = new StreamWriter(lStrLogPath, true))
                {
                    lObjWriter.WriteLine(string.Concat(lStrDate, pStrMessage));
                }
            }
            catch (Exception ex)
            {
                lStrLogPath = Path.Combine(lStrApplicationPath, string.Concat(DateTime.Now.ToString("yyyy-MM-dd-"), Guid.NewGuid().ToString(), ".log"));
                using (StreamWriter lObjWriter = new StreamWriter(lStrLogPath, true))
                {
                    lObjWriter.WriteLine(string.Concat(lStrDate, pStrMessage));
                }
            }
        }
    }
}

