
using System;
using System.Configuration;
using UGRS.Core.Utility;

namespace UGRS.Core.Services
{
    public static class LogService
    {
        private static bool mBolIsFullLog = false;

        static LogService()
        {
            mBolIsFullLog = true; //ConfigurationManager.AppSettings["FullLog"] == null ? false : ConfigurationManager.AppSettings["FullLog"].Equals("true") ? true : false;
        }

        public static void Filename(string pStrProject)
        {
            LogUtility.FileName(pStrProject);
        }

        public static bool IsFullLog
        {
            get { return mBolIsFullLog; }
        }

        public static void WriteInfo(string pStrMessage)
        {
            LogUtility.Write(string.Format("[INFO] {0}", pStrMessage));
        }

        public static void WriteSuccess(string pStrMessage)
        {
            if (IsFullLog)
            {
                LogUtility.Write(string.Format("[SUCCESS] {0}", pStrMessage));
            }
        }

        public static void WriteWarning(string pStrMessage)
        {
            if (IsFullLog)
            {
                LogUtility.Write(string.Format("[WARNING] {0}", pStrMessage));
            }
        }

        public static void WriteError(string pStrMessage)
        {
            LogUtility.Write(string.Format("[ERROR] {0}", pStrMessage));
        }

        public static void WriteError(Exception pObjException)
        {
            if (IsFullLog)
            {
                WriteError(pObjException.Message);
                WriteError(pObjException.ToString());
            }
            else
            {
                WriteError(pObjException.Message);
            }
        }

    }
}
