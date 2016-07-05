using GalaSoft.MvvmLight.Ioc;
using MyML.UWP.Models.Mercadolivre;
using MyML.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.Adapters.Search
{
    public class QuestionDataSource : IPagedSource<ProductQuestionContent>
    {
        private IMercadoLivreService _mercadoLivreServices;

        public QuestionDataSource()
        {
            _mercadoLivreServices = SimpleIoc.Default.GetInstance<IMercadoLivreService>();
        }

        public Task<IPagedResponse<ProductQuestionContent>> GetPage(string query, int pageIndex, int pageSize, string status)
        {
            throw new NotImplementedException();
        }

        public async Task<IPagedResponse<ProductQuestionContent>> GetPage(string query, int pageIndex, int pageSize, bool searchByName)
        {
            try
            {
                var items = new ProductQuestion();

                items = await _mercadoLivreServices.ListQuestionsByProduct(query, pageIndex, pageSize);

                bool success = items == null ? false : items.questions.Count > 0;

                if (success)
                {
                    int virtualCount;
                    virtualCount = items.total ?? 0;
                    return new QuestionsResponse(items.questions.AsEnumerable(), virtualCount);
                }

                return null;
                // throw new WebException("Sem conexão de internet");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<IPagedResponse<ProductQuestionContent>> GetPage(string query, int pageIndex, int pageSize, bool searchByName, bool? highResolutionImages)
        {
            throw new NotImplementedException();
        }
    }

    public class QuestionsResponse : IPagedResponse<ProductQuestionContent>
    {
        public QuestionsResponse(IEnumerable<ProductQuestionContent> items, int virtualCount)
        {
            this.Items = items;
            this.VirtualCount = virtualCount;
        }

        public int VirtualCount { get; private set; }
        public IEnumerable<ProductQuestionContent> Items { get; private set; }
        public IList<AvailableFilter> Filters { get; set; }
        public IList<string> SortStrings { get; set; }
        public IList<AvailableSort> Sorts { get; set; }

        public Paging Paging
        {
            get;
        }
    }
}
