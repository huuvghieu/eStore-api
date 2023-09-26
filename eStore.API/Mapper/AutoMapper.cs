using AutoMapper;
using eStore.Data.Entity;
using eStore.Service.Models.RequestModels;
using eStore.Service.Models.ResponseModels;

namespace eStore.API.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Product
            CreateMap<Product, ProductResponseModel>().ReverseMap();
            CreateMap<CreateProductRequestModel, Product>();
            CreateMap<UpdateProductRequestModel, Product>();
            #endregion

            #region Category
            CreateMap<Category, CategoryResponseModel>().ReverseMap();
            #endregion

            #region Member
            CreateMap<Member, MemberReponseModel>().ReverseMap();
            CreateMap<CreateMemberRequestModel, Member>();
            CreateMap<UpdateMemberRequestModel, Member>();
            #endregion

            #region Order
            CreateMap<Order, OrderResponseModel>().ReverseMap();
            CreateMap<CreateOrderRequestModel, Order>();
            #endregion

            #region OrderDetail
            CreateMap<OrderDetail, OrderDetailResponseModel>().ReverseMap();
            CreateMap<OrderDetailRequestModel, OrderDetail>();
            #endregion
        }
    }
}
