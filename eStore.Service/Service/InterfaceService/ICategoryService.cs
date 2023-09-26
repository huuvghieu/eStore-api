using eStore.Service.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Service.Service.InterfaceService
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseModel>> GetCategories();
    }
}
