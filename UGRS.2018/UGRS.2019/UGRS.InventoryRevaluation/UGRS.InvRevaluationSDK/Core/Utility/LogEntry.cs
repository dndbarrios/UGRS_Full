using System;
using System.IO;
using System.Configuration;

namespace UGRS.Core.Utility {
    public class LogEntry {

        public static void Write(string message) {

            var fileName = $"UGRS.InventoryRevaluation_{DateTime.Now.ToString("yyyy-MM-dd")}.log";
            var logPath = ConfigurationManager.AppSettings["log_path"];
            var filePath = Path.Combine(logPath, fileName);

            if (!Directory.Exists(logPath)) {
                Directory.CreateDirectory(logPath);
            }

            var percistence = new Percistence();
            percistence.SaveToFile(message, filePath);
        }

        public static void WriteInfo(string message) => Write($"[INFO] {message}");

        public static void WriteSuccess(string message) => Write($"[SUCCESS] {message}");

        public static void WriteError(string message) => Write($"[ERROR] {message}");

        public static void WriteException(Exception ex) => Write($"[EXCEPTION] {ex.Message} - {ex.StackTrace}");

    }
}

