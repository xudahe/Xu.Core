﻿using System.Collections.Generic;
using System.Security.Claims;

namespace Xu.Common
{
    public interface IAspNetUser
    {
        string Name { get; }
        int ID { get; }

        bool IsAuthenticated();

        IEnumerable<Claim> GetClaimsIdentity();

        List<string> GetClaimValueByType(string ClaimType);

        string GetToken();

        List<string> GetUserInfoFromToken(string ClaimType);
    }
}