using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyML.UWP.Services
{
    public static class CacheHelper
    {
        public static void AddCache(string sectionName)
        {            
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(sectionName))            
                ApplicationData.Current.LocalSettings.Values[sectionName] = DateTime.Now.ToString();            
            else
                ApplicationData.Current.LocalSettings.Values.Add(sectionName, DateTime.Now.ToString());
        }

        public static bool IsExpired(string sectionName)
        {            
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(sectionName))
            {
                var date = DateTime.MinValue;
                if(DateTime.TryParse(ApplicationData.Current.LocalSettings.Values[sectionName].ToString(), out date))
                {
                    var result = DateTime.Now.Subtract(date).TotalSeconds >= TimeSpan.FromSeconds(30).TotalSeconds;

                    if (result)
                        AddCache(sectionName);//Atualiza o cache

                    return result;
                }
                return true;
            }
            else
            {
                AddCache(sectionName);
                return true;
            }            
        }
    }
}
