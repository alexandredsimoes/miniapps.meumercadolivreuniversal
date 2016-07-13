using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Windows.ApplicationModel;

namespace MyML.UWP.AppStorage
{
    public class AppLogs
    {
        public static async Task WriteError(string source, Exception exception)
        {
            string appVersion =
                $"Versão: {Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";

            var ex = exception;
            int pass = 0;
            var newLine = String.Empty;
            while (ex != null)
            {

                if (pass > 0)
                    newLine = Environment.NewLine;

                if(pass == 0)
                    await WriteLog($"{newLine}{appVersion} - {source}", $"{ex.Message} - {ex.StackTrace}", "Error");
                else
                    await WriteLog("-----------------> ", $"{ex.Message} - {ex.StackTrace}", "");

                ex = ex.InnerException;
                pass++;
            }
            
        }

        public static async Task WriteError(string source, string message)
        {
            string appVersion =
                $"Versão: {Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";

            await WriteLog($"{appVersion} - {source}", message, "Error");
        }

        public static async Task WriteWarning(string source, string message)
        {
            string appVersion =
                $"Versão: {Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";
            await WriteLog($"{appVersion} - {source}", message, "Warning");
            //await WriteLog(source, message, "Warning");
        }

        public static async Task WriteInfo(string source, string message)
        {
            string appVersion =
                $"Versão: {Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";
            await WriteLog($"{appVersion} - {source}", message, "Info");
            //await WriteLog(source, message, "Info");
        }

        public static async Task WriteLog(string source, string message, string messageType)
        {
            try
            {
                message = CleanLogMessage(message);
                string logMessage =
                    $"{DateTime.Now.Ticks}\t{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\t{messageType}\t{source}: {message}\r\n";
                await UserStorage.AppendLineToFile("AppLogs.txt", logMessage);
            }
            catch { /* Avoid any exception at this point. */ }
        }

        private static string CleanLogMessage(string message)
        {
            return message.Replace("\r", string.Empty)
                          .Replace("\n", string.Empty)
                          .Replace("\t", string.Empty);
        }

        public static async Task Clear()
        {
            try
            {
                await UserStorage.DeleteFileIfExists("AppLogs.txt");
            }
            catch { /* Avoid any exception at this point. */ }
        }
    }
}
