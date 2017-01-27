using MyML.UWP.Models.Mercadolivre;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.Services
{
    public interface IDataService
    {
        
        string GetMLConfig(string key);
        bool IsAuthenticated();
        void SaveConfig(string key, string value);
        Task DeleteConfig(string key);
        Task<bool> SaveQuestion(ProductQuestionContent question);
        Task<IEnumerable<ProductQuestionContent>> ListQuestions();
    }
}
