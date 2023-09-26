using eStore.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Data.Repository
{
    public class ProductRepository
    {
        private static ProductRepository _instance;
        private static readonly object _instanceLock = new object();
        private static readonly FStoreDbContext _context = new FStoreDbContext();

        public ProductRepository() { }

        public static ProductRepository Instance
        {
            get
            {
                lock(_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new ProductRepository();
                    }
                    return _instance;
                }
            }
        }

        //get all
        public async Task<IEnumerable<Product>> GetProducts()
        {
            try
            {
                return await _context.Products.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Product> GetAll()
        {
            try
            {
                return _context.Products.AsNoTracking().Include(x => x.Category)
                                        .Include(x => x.OrderDetails);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //get by id
        public async Task<Product> GetProductById(int id)
        {
            try
            {
                var product = await _context.Products.AsNoTracking().Where(x => x.ProductId == id).SingleOrDefaultAsync();
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //insert
        public async Task InsertProduct(Product product)
        {
            try
            {
                await _context.Products.AddAsync(product);
                _context.SaveChanges();
                _context.Entry<Product>(product).State = EntityState.Detached;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //update
        public async Task UpdateProduct(Product product)
        {
            try
            {
                _context.Entry<Product>(product).State = EntityState.Modified;
                _context.SaveChanges();
                _context.Entry<Product>(product).State = EntityState.Detached;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //delete
        public async Task DeleteProduct(Product product)
        {
            try
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
				_context.Entry<Product>(product).State = EntityState.Detached;
				_context.SaveChanges();
			}
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }   
}
