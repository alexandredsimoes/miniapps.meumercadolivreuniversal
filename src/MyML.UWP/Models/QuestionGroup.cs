using MyML.UWP.Models.Mercadolivre;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.Models
{
    public class QuestionGroup
    {
        public Item Produto { get; set; }
        public ObservableCollection<ProductQuestionContent> Perguntas { get; set; }
    }

    public class QuestionGroup2 : ObservableCollection<ProductQuestionContent>
    {
        public Item Key { get; set; }
        //public Item Produto { get; set; }
        //public ObservableCollection<ProductQuestionContent> Perguntas { get; set; }
    }
}
