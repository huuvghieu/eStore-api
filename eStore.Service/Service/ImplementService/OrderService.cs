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
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        public OrderService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderResponseModel>> GetOrders()
        {
            try
            {
                var orders = OrderRepository.Instance.GetAll();
                List<OrderResponseModel> result = new List<OrderResponseModel>();
                foreach (var order in orders)
                {
                    var orderDetail = _mapper.Map<List<OrderDetailResponseModel>>(order.OrderDetails);
                    var memberResult = _mapper.Map<Member, MemberReponseModel>(order.Member);
                    var orderResult = new OrderResponseModel()
                    {
                        OrderId = order.OrderId,
                        MemberId = order.MemberId,
                        Freight = order.Freight,
                        Member = memberResult,
                        OrderDate = order.OrderDate,
                        OrderDetails = orderDetail,
                        RequiredDate = order.RequiredDate,
                        ShippedDate = order.ShippedDate,
                    };
                    result.Add(orderResult);
                }
                return result;
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get all products failed!", ex?.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<OrderResponseModel> GetOrderById(int id)
        {
            try
            {
                var order = OrderRepository.Instance.GetAll()
                    .Where(x => x.OrderId == id).SingleOrDefault();
                if (order == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found order with id", id.ToString());
                }

                var orderDetail = _mapper.Map<List<OrderDetailResponseModel>>(order.OrderDetails);
                var memberResult = _mapper.Map<Member, MemberReponseModel>(order.Member);
                var orderResult = new OrderResponseModel()
                {
                    OrderId = order.OrderId,
                    MemberId = order.MemberId,
                    Freight = order.Freight,
                    Member = memberResult,
                    OrderDate = order.OrderDate,
                    OrderDetails = orderDetail,
                    RequiredDate = order.RequiredDate,
                    ShippedDate = order.ShippedDate,
                };

                return orderResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<OrderResponseModel> CreateOrder(CreateOrderRequestModel request)
        {
            try
            {

                Order order = new Order();
                _mapper.Map<CreateOrderRequestModel, Order>(request, order);

                #region Order
                order.OrderDate = DateTime.Now;
                order.ShippedDate = DateTime.Now.AddDays(5);
                Random random = new Random();
                order.OrderId = random.Next(1000, 10000);
                order.Freight = random.Next(10000, 100000);
                //check id
                var checkOrder = OrderRepository.Instance.GetAll().Where(x => x.OrderId == order.OrderId)
                                                               .FirstOrDefault();
                while (checkOrder != null)
                {
                    order.OrderId = random.Next(1000, 10000);
                    checkOrder = OrderRepository.Instance.GetAll().Where(x => x.OrderId == order.OrderId)
                                                               .FirstOrDefault();
                }
                #endregion

                #region OrderDetail
                List<OrderDetail> orderDetails = new List<OrderDetail>();
                foreach (var orderDetailRequest in request.OrderDetails)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    _mapper.Map<OrderDetailRequestModel, OrderDetail>(orderDetailRequest, orderDetail);
                    orderDetail.OrderId = order.OrderId;

                    var product = ProductRepository.Instance.GetAll().Where(x => x.ProductId == orderDetail.ProductId).SingleOrDefault();
                    orderDetail.UnitPrice = product.UnitPrice;
                    orderDetails.Add(orderDetail);
                    //update quatity of product

                    product.UnitsInStock = product.UnitsInStock - orderDetail.Quantity;
                    await ProductRepository.Instance.UpdateProduct(product);
                    order.OrderDetails = orderDetails;
                }

                await OrderRepository.Instance.InsertOrder(order);

                var orderDetailResult = _mapper.Map<List<OrderDetailResponseModel>>(order.OrderDetails);
                var memberResult = _mapper.Map<Member, MemberReponseModel>(order.Member);

                var orderResult = new OrderResponseModel()
                {
                    OrderId = order.OrderId,
                    MemberId = order.MemberId,
                    Freight = order.Freight,
                    Member = memberResult,
                    OrderDate = order.OrderDate,
                    OrderDetails = orderDetailResult,
                    RequiredDate = order.RequiredDate,
                    ShippedDate = order.ShippedDate,
                };
                #endregion

                return orderResult;
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
    }
}
