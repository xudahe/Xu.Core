using System.Collections.Generic;
using System.Security.Claims;
using Xu.Model.ResultModel;

namespace Xu.Common.HttpContextUser
{
    public interface IAspNetUser
    {
        string Name { get; }

        int ID { get; }

        /// <summary>
        /// 租户id
        /// </summary>
        long TenantId { get; }

        bool IsAuthenticated();

        IEnumerable<Claim> GetClaimsIdentity();

        List<string> GetClaimValueByType(string ClaimType);

        string GetToken();

        List<string> GetUserInfoFromToken(string ClaimType);

        MessageModel<string> MessageModel { get; set; }
    }
}