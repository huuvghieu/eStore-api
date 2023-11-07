using eStore.Service.Models.RequestModels;
using eStore.Service.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Service.Service.InterfaceService
{
    public interface IProductService
    {
        public Task<IEnumerable<ProductResponseModel>> GetProducts();

        public Task<ProductResponseModel> GetProductById(int id);

        public Task<ProductResponseModel> InsertProduct(CreateProductRequestModel productRequest);

        public Task<ProductResponseModel> UpdateProduct(int productId, UpdateProductRequestModel productRequest);

        public Task<ProductResponseModel> DeleteProduct(int productId);

        Task<OrderDetailRequestModel> GetCartItemsByProductId(int id);

        public Task<IEnumerable<ProductResponseModel>> SearchProduct(string searchString);
    }
}
