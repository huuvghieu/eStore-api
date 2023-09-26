using eStore.Service.Models.RequestModels;
using eStore.Service.Models.ResponseModels;
using eStore.Service.Service.InterfaceService;
using Microsoft.AspNetCore.Mvc;

namespace eStore.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthUserController : ControllerBase
    {
        private readonly IMemberService _memberService;
        private readonly IAuthenService _authenService;

        public AuthUserController(IMemberService memberService, IAuthenService authenService)
        {
            _memberService = memberService;
            _authenService = authenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseModel>> Login([FromBody] LoginRequestModel request)
        {
            var rs = await _authenService.Login(request);
            return Ok(rs);
        }

        [HttpPost("registeration")]
        public async Task<ActionResult<MemberReponseModel>> Register([FromBody] CreateMemberRequestModel request)
        {
            var rs = await _authenService.Register(request);
            return Ok(rs);
        }
    }
}
