using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyML.UWP.AppStorage
{
    public class UserStorage
    {
        public static async Task<string> ReadTextFromFile(string fileName)
        {
            try
            {                
                if (await CheckFileExists(fileName) == false) return string.Empty;

                var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(fileName);
                if (file != null)
                {
                    using (StreamReader reader = new StreamReader(await file.OpenStreamForReadAsync()))
                    {
                        return await reader.ReadToEndAsync();                       
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return String.Empty;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("UserStorage.ReadTextFromFile", ex);
            }
            return String.Empty;
        }

        public static async Task WriteText(string fileName, string content)
        {
            try
            {

                var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile file = null;
                if (await CheckFileExists(fileName) == false)
                {
                    file = await folder.CreateFileAsync(fileName);        
                }
                else
                    file = await folder.GetFileAsync(fileName);

                using (StreamWriter writer = new StreamWriter(await file.OpenStreamForWriteAsync()))
                {
                    await writer.WriteAsync(content);
                }
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("UserStorage.WriteText", ex);
                return;
            }                      
        }

        public static async Task DeleteFileIfExists(string fileName)
        {
            try
            {
                var folder = Windows.Storage.ApplicationData.Current.LocalFolder;                
                var file = await folder.GetFileAsync(fileName);
                if (file != null)
                {
                    await file.DeleteAsync();
                }
            }
            catch (FileNotFoundException)
            {
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("UserStorage.DeleteFileIfExists", ex);
            }
        }

        public static async Task AppendLineToFile(string fileName, string line)
        {
            try
            {
                var old = await ReadTextFromFile(fileName);

                var folder = Windows.Storage.ApplicationData.Current.LocalFolder;

                StorageFile file = null;
                if (await CheckFileExists(fileName) == false)
                {
                    file = await folder.CreateFileAsync(fileName);
                }
                else
                    file = await folder.GetFileAsync(fileName);


                if (file != null)
                {
                    using (StreamWriter writer = new StreamWriter(await file.OpenStreamForWriteAsync()))
                    {
                        await writer.WriteAsync(old + Environment.NewLine + line);
                    }
                }
            }
            catch { /* Avoid any exception at this point. */ }
        }

        public static async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                //var old = ReadTextFromFile(fileName);

                var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(fileName);
                return true;              
            }
            catch(FileNotFoundException f) { return false; }
        }
    }
}
