using eStore.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Data.Repository
{
    public class OrderRepository
    {
        private static OrderRepository _instance;
        private static readonly object _instanceLock = new object();
        private static readonly FStoreDbContext _context = new FStoreDbContext();

        private OrderRepository() { }

        public static OrderRepository Instance
        {
            get
            {
                lock(_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new OrderRepository();
                    }
                    return _instance;
                }
            }
        }

        //Get all
        public async Task<IEnumerable<Order>> GetOrders()
        {
            try
            {
                return await _context.Orders.AsNoTracking().Include(x => x.OrderDetails)
                                .Include(x => x.Member)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Order> GetAll()
        {
            try
            {
                return _context.Orders.AsNoTracking()
                                .Include(x => x.OrderDetails)
                                .Include(x => x.Member);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task InsertOrder(Order order)
        {
            try
            {
                await _context.Orders.AddAsync(order);
                _context.SaveChanges();
                _context.Entry<Order>(order).State = EntityState.Detached;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
