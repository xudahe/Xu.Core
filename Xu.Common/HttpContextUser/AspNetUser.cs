using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xu.Common;
using Xu.Common.HttpContextUser;
using Xu.Model.ResultModel;

namespace Xu.Common.HttpContextUser
{
    public class AspNetUser : IAspNetUser
    {
        private readonly IHttpContextAccessor _accessor;

        public AspNetUser(IHttpContextAccessor accessor, ILogger<AspNetUser> logger)
        {
            _accessor = accessor;
        }

        public string Name => GetName();

        private string GetName()
        {
            if (IsAuthenticated() && _accessor.HttpContext.User.Identity.Name.IsNotEmptyOrNull())
            {
                return _accessor.HttpContext.User.Identity.Name;
            }
            else
            {
                if (!string.IsNullOrEmpty(GetToken()))
                {
                    var getNameType = Permissions.IsUseIds4 ? "name" : "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
                    return GetUserInfoFromToken(getNameType).FirstOrDefault().ObjToString();
                }
            }

            return "";
        }

        public int ID => GetClaimValueByType("jti").FirstOrDefault().ToInt32Req();

        public long TenantId => GetClaimValueByType("TenantId").FirstOrDefault().ToInt64Req();

        public bool IsAuthenticated()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }


        public string GetToken()
        {
            return _accessor.HttpContext?.Request?.Headers["Authorization"].ObjToString().Replace("Bearer ", "");
        }

        public List<string> GetUserInfoFromToken(string ClaimType)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var token = "";

            token = GetToken();
            // token校验
            if (token.IsNotEmptyOrNull() && jwtHandler.CanReadToken(token))
            {
                JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(token);

                return (from item in jwtToken.Claims
                        where item.Type == ClaimType
                        select item.Value).ToList();
            }

            return new List<string>() { };
        }

        public MessageModel<string> MessageModel { get; set; }

        public IEnumerable<Claim> GetClaimsIdentity()
        {
            var claims = _accessor.HttpContext.User.Claims.ToList();
            var headers = _accessor.HttpContext.Request.Headers;
            foreach (var header in headers)
            {
                claims.Add(new Claim(header.Key, header.Value));
            }

            return claims;
        }

        public List<string> GetClaimValueByType(string ClaimType)
        {
            return (from item in GetClaimsIdentity()
                    where item.Type == ClaimType
                    select item.Value).ToList();
        }
    }
}