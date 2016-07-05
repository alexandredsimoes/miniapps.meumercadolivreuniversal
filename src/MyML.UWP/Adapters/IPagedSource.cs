using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyML.UWP.Adapters
{
    public interface IPagedSource<T>
    {
        Task<IPagedResponse<T>> GetPage(string query, int pageIndex, int pageSize, bool searchByName);
        Task<IPagedResponse<T>> GetPage(string query, int pageIndex, int pageSize, bool searchByName, bool? highResolutionImages);
    }
}
