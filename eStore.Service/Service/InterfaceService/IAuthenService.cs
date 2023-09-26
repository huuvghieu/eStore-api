using eStore.Service.Models.RequestModels;
using eStore.Service.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Service.Service.InterfaceService
{
    public interface IAuthenService
    {
        public Task<LoginResponseModel> Login(LoginRequestModel loginRequest);

        public Task<MemberReponseModel> Register(CreateMemberRequestModel createMemberRequest);

    }
}
