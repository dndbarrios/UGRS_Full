using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Core.Utility
{
    /// <summary> A log utility. </summary>
    /// <remarks> Ranaya, 24/05/2017. </remarks>

    public class LogUtility
    {
        private static bool mBolFullLog = false;
        private static string mStrFileName;

        static LogUtility()
        {
            mBolFullLog = ConfigurationManager.AppSettings.AllKeys.Contains("FullLog") && (
                          ConfigurationManager.AppSettings["FullLog"].ToString().Equals("true") ||
                          ConfigurationManager.AppSettings["FullLog"].ToString().Equals("True")) ? true : false;

           
        }

        public static void FileName(string pStrName)
        {
             mStrFileName = pStrName;
        }


        public static string FileNameLog
        {
            get { return mStrFileName; }
        }

        public static bool FullLog
        {
            get { return mBolFullLog; }
        }

        /// <summary> Writes. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>
        /// <param name="pStrMessage"> The String message to write. </param>

        public static void Write(string pStrMessage)
        {
            string lStrApplicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");
            string lStrDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ");
            string lStrFilename = FileNameLog + ".log";
            string lStrLogPath = Path.Combine(lStrApplicationPath,lStrFilename);
            

            //try
            //{
            //    using (StreamWriter lObjWriter = new StreamWriter(lStrLogPath, true))
            //    {
            //        lObjWriter.WriteLine(string.Concat(lStrDate, pStrMessage));
            //    }
            //}
            //catch
            //{
                try
                {
                    lStrLogPath = PathUtilities.GetDocuments() + @"\Qualisys\Log";
                    lStrLogPath = Path.Combine(CreateFolder(lStrLogPath), lStrFilename);
                    using (StreamWriter lObjWriter = new StreamWriter(lStrLogPath, true))
                    {
                        lObjWriter.WriteLine(string.Concat(lStrDate, pStrMessage));
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        lStrLogPath = Path.Combine(CreateFolder(@"c:\Qualisys\Log"), lStrFilename);
                        using (StreamWriter lObjWriter = new StreamWriter(lStrLogPath, true))
                        {
                            lObjWriter.WriteLine(string.Concat(lStrDate, pStrMessage));
                        }
                    }
                    catch (Exception)
                    {
                       
                       
                    }

                }
        //}
        }

        private static string CreateFolder(string pStrPath)
        {
            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(pStrPath))
                {
                    return pStrPath;
                }

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(pStrPath);
                

            }
            catch (Exception)
            {

            }
            finally {  }
            return pStrPath;
        }

        public static void WriteInfo(string pStrMessage)
        {
            Write(string.Format("[INFO] {0}", pStrMessage));
        }

        public static void WriteSuccess(string pStrMessage)
        {
            if (FullLog)
            {
                Write(string.Format("[SUCCESS] {0}", pStrMessage));
            }
        }

        public static void WriteTracking(string pStrMessage)
        {
            if (FullLog)
            {
                Write(string.Format("[TRACK] {0}", pStrMessage));
            }
        }

        public static void WriteProcess(string pStrMessage)
        {
            if (FullLog)
            {
                Write(string.Format("[PROCESS] {0}", pStrMessage));
            }
        }

        public static void WriteWarning(string pStrMessage)
        {
            if (FullLog)
            {
                Write(string.Format("[WARNING] {0}", pStrMessage));
            }
        }

        public static void WriteError(string pStrMessage)
        {
            Write(string.Format("[ERROR] {0}", pStrMessage));
        }

        public static void WriteException(Exception pObjException)
        {
            if (FullLog)
            {
                Write(string.Format("[ERROR] {0}", pObjException.ToString()));
            }
            else
            {
                Write(string.Format("[ERROR] {0}", pObjException.Message));
            }
        }
    }
}

