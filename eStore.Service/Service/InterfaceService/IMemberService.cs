using eStore.Service.Models.RequestModels;
using eStore.Service.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Service.Service.InterfaceService
{
    public interface IMemberService
    {
        public Task<IEnumerable<MemberReponseModel>> GetMembers();
        public Task<MemberReponseModel> GetMemberById(int id);

        public Task<MemberReponseModel> InsertMember(CreateMemberRequestModel memberRequest);
        public Task<MemberReponseModel> UpdateMember(int id, UpdateMemberRequestModel memberRequest);

        public Task<MemberReponseModel> DeleteMember(int id);

    }
}
