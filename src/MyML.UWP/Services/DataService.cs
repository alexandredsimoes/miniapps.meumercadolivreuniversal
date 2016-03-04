using MyML.UWP.AppStorage;
using MyML.UWP.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization.DateTimeFormatting;
using Windows.Storage;
using MyML.UWP.Models.Mercadolivre;
using SQLite.Net.Async;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System.IO;

namespace MyML.UWP.Services
{
    public class DataService : IDataService
    {
        private readonly SQLiteConnectionWithLock sqliteConnection;
        private string databasePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "myml.universal.db");
        SQLiteAsyncConnection _DbContext;
        public DataService()
        {
            sqliteConnection = new SQLiteConnectionWithLock(new SQLitePlatformWinRT(), new SQLiteConnectionString(databasePath, false));
            _DbContext = new SQLiteAsyncConnection(()=>sqliteConnection);
            //TODO: Rever isso
        }

        public SQLiteAsyncConnection DbContext
        {
            get
            {
                return _DbContext; 
            }
        }

        public Task DeleteConfig(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
                ApplicationData.Current.LocalSettings.Values[key] = null;
            return Task.CompletedTask;
        }

        public string GetMLConfig(string key)
        {
            string result = null;
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
                result = ApplicationData.Current.LocalSettings.Values[key].ToString();
#if DEBUG
            Debug.WriteLine($"{key}={result}");
#endif

            return result;
        }

        public bool IsAuthenticated()
        {
            bool result = false;
            try
            {
                var userId =  GetMLConfig(Consts.ML_CONFIG_KEY_USER_ID);
                var accessToken =  GetMLConfig(Consts.ML_CONFIG_KEY_ACCESS_TOKEN);
                var expiration =  GetMLConfig(Consts.ML_CONFIG_KEY_EXPIRES);
                //var dateLogin = await GetMLConfig(Consts.ML_CONFIG_KEY_LOGIN_DATE);
                
                var dateTimeExpiration = DateTime.MinValue;


                if (!String.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(accessToken))
                {
                    if(DateTime.TryParse(expiration, out dateTimeExpiration))
                    {
                        var data = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                        return dateTimeExpiration > data;
                    }
                }               
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("DataService.IsAuthenticated", ex);
            }
            return result;         
        }

        public async Task<IEnumerable<ProductQuestionContent>> ListQuestions()
        {
            return await _DbContext.Table<ProductQuestionContent>().ToListAsync();
            //return await _context.Questions.ToListAsync();
        }

        public void SaveConfig(string key, string value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;                  
        }

        public async Task<bool> SaveQuestion(ProductQuestionContent question)
        {
            return await _DbContext.InsertAsync(question) > 0;
            return false;
            //_context.Entry(question).State = EntityState.Added;
            //return await _context.SaveChangesAsync() > 0;
        }

        public async void Initialize()
        {
            await _DbContext.CreateTableAsync<ProductQuestionContent>();
            await _DbContext.CreateTableAsync<Answer>();
            await _DbContext.CreateTableAsync<Item>();

        }
    }
}
