using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyML.UWP.Models.Mercadolivre;


namespace MyML.UWP.Services
{
    public class DesignDataService : IDataService
    {
        public string GetMLConfig(string key)
        {
            return String.Empty;
        }

        public bool IsAuthenticated()
        {
            return true;
        }

        public void SaveConfig(string key, string value)
        {
            
        }

        public Task DeleteConfig(string key)
        {
            return Task.CompletedTask;
        }

        public Task<bool> SaveQuestion(ProductQuestionContent question)
        {
            return Task.FromResult<bool>(false);
        }

        public Task<IEnumerable<ProductQuestionContent>> ListQuestions()
        {
            return Task.FromResult<IEnumerable<ProductQuestionContent>>(new List<ProductQuestionContent>());
        }
    }
}