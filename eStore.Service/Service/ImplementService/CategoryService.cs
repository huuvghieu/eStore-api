using AutoMapper;
using eStore.Data.Entity;
using eStore.Data.Repository;
using eStore.Service.Exceptions;
using eStore.Service.Models.ResponseModels;
using eStore.Service.Service.InterfaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Service.Service.ImplementService
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;

        public CategoryService(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public async Task<IEnumerable<CategoryResponseModel>> GetCategories()
        {
            try
            {
                var categories = await CategoryRepository.Instance.GetCategories();
                return _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryResponseModel>>(categories);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get all categories failed!!!", ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
