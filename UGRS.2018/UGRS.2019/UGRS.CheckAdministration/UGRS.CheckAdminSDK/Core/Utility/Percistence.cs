using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace UGRS.Core.Utility {

    //Single Responsibility
    class Percistence {

        private static ReaderWriterLockSlim readWriteLock = new ReaderWriterLockSlim();
        public void SaveToFile(string message, string filePath) {

            try {
                readWriteLock.EnterWriteLock();
                WriteToFile(message, filePath);
            }
            catch {
                var appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");
                WriteToFile(message, Path.Combine(appPath, Path.GetFileName(filePath)));
            }
            finally {
                readWriteLock.ExitWriteLock();
            }
        }

        public void WriteToFile(string message, string path) {
            using (StreamWriter streamWriter = new StreamWriter(path, true))
                streamWriter.WriteLine(String.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss "), message));
        }
    }
}
