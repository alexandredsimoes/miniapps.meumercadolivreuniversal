using MyML.UWP.Models.Mercadolivre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.ViewModels.Design
{
    public class CategoriaPageDesignViewModel
    {
        public CategoriaPageDesignViewModel()
        {

            CategoryDetail = new MLCategorySearchResult()
            {
                children_categories = new List<ChildrenCategory>()
                {
                    new ChildrenCategory()
                    {
                        id = "123456",
                        name = "Acessórios",
                        total_items_in_this_category = 125663
                    },
                    new ChildrenCategory()
                    {
                        id = "123456",
                        name = "Cama mesa e banho",
                        total_items_in_this_category = 3
                    },
                    new ChildrenCategory()
                    {
                        id = "123456",
                        name = "Outras categorias",
                        total_items_in_this_category = 566
                    },
                    new ChildrenCategory()
                    {
                        id = "123456",
                        name = "Acessórios",
                        total_items_in_this_category = 12
                    }

                }
            };
        }

        public MLCategorySearchResult CategoryDetail { get; set; }
    }
}
