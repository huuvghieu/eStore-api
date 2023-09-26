using eStore.Service.Models.RequestModels;
using eStore.Service.Models.ResponseModels;
using eStore.Service.Service.InterfaceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eStore.API.Controllers
{
    [Route("api/members")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _service;

        public MembersController(IMemberService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<MemberReponseModel>>> GetMembers()
        {
            var rs = await _service.GetMembers();
            return Ok(rs);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MemberReponseModel>> GetMemberById(int id)
        {
            var rs = await _service.GetMemberById(id);
            return Ok(rs);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MemberReponseModel>> CreateMember([FromBody] CreateMemberRequestModel memberRequest)
        {
            var rs = await _service.InsertMember(memberRequest);
            return Ok(rs);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<MemberReponseModel>> UpdateMember(int id, [FromBody] UpdateMemberRequestModel memberRequest)
        {
            var rs = await _service.UpdateMember(id, memberRequest);
            return Ok(rs);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MemberReponseModel>> DeleteMember(int id)
        {
            var rs = await _service.DeleteMember(id);
            return Ok(rs);
        }
    }
}
