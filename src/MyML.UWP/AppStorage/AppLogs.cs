using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Windows.ApplicationModel;

namespace MyML.UWP.AppStorage
{
    public class AppLogs
    {
        static public async void WriteError(string source, Exception exception)
        {
            string appVersion = string.Format("Versão: {0}.{1}.{2}.{3}",
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision);

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

        static public async void WriteError(string source, string message)
        {
            string appVersion = string.Format("Versão: {0}.{1}.{2}.{3}",
                        Package.Current.Id.Version.Major,
                        Package.Current.Id.Version.Minor,
                        Package.Current.Id.Version.Build,
                        Package.Current.Id.Version.Revision);

            await WriteLog($"{appVersion} - {source}", message, "Error");
        }

        static public async void WriteWarning(string source, string message)
        {
            string appVersion = string.Format("Versão: {0}.{1}.{2}.{3}",
                        Package.Current.Id.Version.Major,
                        Package.Current.Id.Version.Minor,
                        Package.Current.Id.Version.Build,
                        Package.Current.Id.Version.Revision);
            await WriteLog($"{appVersion} - {source}", message, "Warning");
            //await WriteLog(source, message, "Warning");
        }

        static public async Task WriteInfo(string source, string message)
        {
            string appVersion = string.Format("Versão: {0}.{1}.{2}.{3}",
                                    Package.Current.Id.Version.Major,
                                    Package.Current.Id.Version.Minor,
                                    Package.Current.Id.Version.Build,
                                    Package.Current.Id.Version.Revision);
            await WriteLog($"{appVersion} - {source}", message, "Info");
            //await WriteLog(source, message, "Info");
        }

        static public async Task WriteLog(string source, string message, string messageType)
        {
            try
            {

                message = CleanLogMessage(message);
                string logMessage = String.Format("{0}\t{1}\t{2}\t{3}: {4}\r\n", DateTime.Now.Ticks, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), messageType, source, message);
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

        static public async Task Clear()
        {
            try
            {
                await UserStorage.DeleteFileIfExists("AppLogs.txt");
            }
            catch { /* Avoid any exception at this point. */ }
        }
    }
}
