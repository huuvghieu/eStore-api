using eStore.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Data.Repository
{
    public class CategoryRepository
    {
        private static CategoryRepository _instance;
        private static readonly object _instanceLock = new object();
        private static readonly FStoreDbContext _context = new FStoreDbContext();

        public CategoryRepository() { }
        public static CategoryRepository Instance
        {
            get
            {
                lock(_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new CategoryRepository();
                    }
                    return _instance;
                }
            }
        }

        //get all
        public async Task<IEnumerable<Category>> GetCategories()
        {
            try
            {
                return await _context.Categories.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Category> GetAll()
        {
            try
            {
                return _context.Categories.AsNoTracking();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
