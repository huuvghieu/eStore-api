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
    public class MemberService : IMemberService
    {
        private readonly IMapper _mapper;

        public MemberService(IMapper mapper)
        {
            _mapper = mapper;
        }


        public async Task<MemberReponseModel> DeleteMember(int id)
        {
            try
            {
                Member member = MemberRepository.Instance.GetAll().Where(x => x.MemberId == id)
                                                           .SingleOrDefault();
                if (member == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found member with {id}!!!", id.ToString());
                }
                await MemberRepository.Instance.DeleteMember(member);
                return _mapper.Map<Member, MemberReponseModel>(member);

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

        public async Task<MemberReponseModel> GetMemberById(int id)
        {
            var member = await MemberRepository.Instance.GetMemberById(id);
            if (member == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, $"Not found member with {id}", id.ToString());
            }
            return _mapper.Map<Member, MemberReponseModel>(member);
        }

        public async Task<IEnumerable<MemberReponseModel>> GetMembers()
        {
            try
            {
                var members = await MemberRepository.Instance.GetMembers();
                return _mapper.Map<IEnumerable<Member>, IEnumerable<MemberReponseModel>>(members);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get all members failed!!!", ex?.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<MemberReponseModel> InsertMember(CreateMemberRequestModel memberRequest)
        {
            try
            {
                var checkMember = MemberRepository.Instance.GetAll().Where(x => x.Email.Equals(memberRequest.Email))
                                                             .FirstOrDefault();
                if (checkMember != null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Member is already exits!!!", memberRequest.Email);
                }
                var member = _mapper.Map<CreateMemberRequestModel, Member>(memberRequest);
                member.MemberId = MemberRepository.Instance.GetAll().Max(x => x.MemberId) + 1;
                await MemberRepository.Instance.InsertMember(member);
                return _mapper.Map<Member, MemberReponseModel>(member);
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

        public async Task<MemberReponseModel> UpdateMember(int id, UpdateMemberRequestModel memberRequest)
        {
            try
            {
                Member? member = null;
                member = MemberRepository.Instance.GetAll().Where(x => x.MemberId == id)
                                             .SingleOrDefault();
                if (member == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found member with {id}!!!", id.ToString());
                }
                _mapper.Map<UpdateMemberRequestModel, Member>(memberRequest, member);
                await MemberRepository.Instance.UpdateMember(member);
                return _mapper.Map<Member, MemberReponseModel>(member);
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
