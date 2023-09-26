using AutoMapper;
using eStore.Data.Entity;
using eStore.Data.Repository;
using eStore.Service.Exceptions;
using eStore.Service.Models.RequestModels;
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
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;

        public ProductService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<ProductResponseModel> DeleteProduct(int productId)
        {
            try
            {
                Product product = ProductRepository.Instance.GetAll().Where(x => x.ProductId == productId)
                                                           .SingleOrDefault();
                if (product == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found product with id!!!", productId.ToString());
                }
                if (product.OrderDetails.Count > 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Can not delete product that in order!!", productId.ToString());
                }
                await ProductRepository.Instance.DeleteProduct(product);
                return _mapper.Map<Product, ProductResponseModel>(product);

            }
            catch (CrudException ex)
            {
                throw new CrudException(ex.StatusCode, ex.Message, ex?.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProductResponseModel> GetProductById(int id)
        {
            try
            {
                var product = await ProductRepository.Instance.GetProductById(id);
                if(product == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found product with {id}", id.ToString());
                }
                return _mapper.Map<Product, ProductResponseModel>(product);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<ProductResponseModel>> GetProducts()
        {
            try
            {
                var products = await ProductRepository.Instance.GetProducts();
                return _mapper.Map<IEnumerable<Product>, IEnumerable<ProductResponseModel>>(products);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get all products failed!!!", ex.Message);
            }
        }

        public async Task<ProductResponseModel> InsertProduct(CreateProductRequestModel productRequest)
        {
            try
            {
                var checkProduct = ProductRepository.Instance.GetAll().Where(x => x.ProductName.Equals(productRequest.ProductName))
                                                             .SingleOrDefault();
                if(checkProduct != null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Product is already exist!!!", productRequest.ProductName);
                }
                var product = _mapper.Map<CreateProductRequestModel, Product>(productRequest);
                product.ProductId = ProductRepository.Instance.GetAll().Max(x => x.ProductId) + 1;
                await ProductRepository.Instance.InsertProduct(product);
                return _mapper.Map<Product, ProductResponseModel>(product);
            }
            catch (CrudException ex)
            {
                throw new CrudException(ex.StatusCode, ex.Message, ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<ProductResponseModel>> SearchProduct(string searchString)
        {
            try
            {
                var products = ProductRepository.Instance.GetAll().Where(x => x.ProductName.Contains(searchString) ||
                                                                       x.UnitPrice.ToString().Contains(searchString)).ToList();

                if (products == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found products!!!", searchString);
                }
                return _mapper.Map<IEnumerable<Product>, IEnumerable<ProductResponseModel>>(products);
            }
            catch (CrudException ex)
            {
                throw new CrudException(ex.StatusCode, ex.Message, ex?.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProductResponseModel> UpdateProduct(int productId, UpdateProductRequestModel productRequest)
        {
            try
            {
                Product? product = null;
                product = ProductRepository.Instance.GetAll().Where(x => x.ProductId == productId)
                                             .SingleOrDefault();
                if (product == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found product with {productId}!!!", productId.ToString());
                }
                _mapper.Map<UpdateProductRequestModel, Product>(productRequest, product);
                await ProductRepository.Instance.UpdateProduct(product);
                return _mapper.Map<Product, ProductResponseModel>(product);
            }
            catch (CrudException ex)
            {
                throw new CrudException(ex.StatusCode, ex.Message, ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
