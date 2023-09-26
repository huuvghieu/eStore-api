using eStore.Service.Models.RequestModels;
using eStore.Service.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Service.Service.InterfaceService
{
    public interface IOrderService
    {
        public Task<IEnumerable<OrderResponseModel>> GetOrders();

        public Task<OrderResponseModel> GetOrderById(int id);

        public Task<OrderResponseModel> CreateOrder(CreateOrderRequestModel request);
    }
}
