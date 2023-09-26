using AutoMapper;
using eStore.Data.Entity;
using eStore.Data.Repository;
using eStore.Service.Exceptions;
using eStore.Service.Models.RequestModels;
using eStore.Service.Models.ResponseModels;
using eStore.Service.Service.InterfaceService;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Service.Service.ImplementService
{
    public class AuthenService : IAuthenService
    {
        private readonly IMapper _mapper;
        private string _secretKey;
        private IConfiguration _config;


        public AuthenService(IMapper mapper, IConfiguration config)
        {
            _mapper = mapper;
            _config = config;
        }

        public async Task<LoginResponseModel> Login(LoginRequestModel loginRequest)
        {
            try
            {
                //check admin
                var admin = _config.GetSection("Admin");
                var emailAdmin = admin["email"];
                var passwordAdmin = admin["password"];

                bool isAdmin = loginRequest.Email.Equals(emailAdmin) && loginRequest.Password.Equals(passwordAdmin);

                if (isAdmin)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var secretKey = _config.GetSection("ApiSetting");
                    _secretKey = secretKey["Secret"];

                    var key = Encoding.ASCII.GetBytes(_secretKey);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Email, emailAdmin),
                            new Claim(ClaimTypes.Role, "Admin")
                        }),

                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new( new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature )
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    LoginResponseModel loginResponse = new LoginResponseModel
                    {
                        Token = tokenHandler.WriteToken(token),
                    };
                    return loginResponse;
                }
                else
                {
                    var member = MemberRepository.Instance.GetAll().Where(x => x.Email.Equals(loginRequest.Email) && 
                                                                               x.Password.Equals(loginRequest.Password)).SingleOrDefault();
                    if(member == null)
                    {
                        throw new CrudException(HttpStatusCode.NotFound, "Wrong Email or Password!!!", loginRequest.Email);
                    }

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var secretKey = _config.GetSection("ApiSetting");
                    _secretKey = secretKey["Secret"];

                    var key = Encoding.ASCII.GetBytes(_secretKey);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Email, loginRequest.Email),
                            new Claim(ClaimTypes.Role, "Customer")
                        }),
                        
                        Expires = DateTime.UtcNow.AddDays(7),

                        SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);

                    LoginResponseModel loginResponse = new LoginResponseModel
                    {
                        Token = tokenHandler.WriteToken(token),
                    };

                    return loginResponse;
                }
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

        public async Task<MemberReponseModel> Register(CreateMemberRequestModel createMemberRequest)
        {
            try
            {
                var checkMember = MemberRepository.Instance.GetAll().Where(x => x.Email.Equals(createMemberRequest.Email))
                                                             .FirstOrDefault();
                if (checkMember != null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Member is already exits!!!", createMemberRequest.Email);
                }
                var member = _mapper.Map<CreateMemberRequestModel, Member>(createMemberRequest);
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
    }
}
