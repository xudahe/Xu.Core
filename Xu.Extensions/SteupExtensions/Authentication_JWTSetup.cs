﻿using Castle.Core.Internal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xu.Common;

namespace Xu.Extensions
{
    /// <summary>
    /// JWT权限 认证服务
    /// </summary>
    public static class Authentication_JWTSetup
    {
        public static void AddAuthentication_JWTSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //读取配置文件
            var symmetricKeyAsBase64 = AppSecretConfig.Audience_Secret_String;
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var Issuer = Appsettings.App(new string[] { "Audience", "Issuer" });
            var Audience = Appsettings.App(new string[] { "Audience", "Audience" });

            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // 令牌验证参数
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,//是否验证SecurityKey
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,//是否验证Issuer
                ValidIssuer = Issuer,//发行人
                ValidateAudience = true,//是否验证Audience
                ValidAudience = Audience,//订阅人
                ValidateLifetime = true,//是否验证失效时间
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireExpirationTime = true,
            };

            // 开启Bearer认证
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = nameof(ApiResponseHandler);
                o.DefaultForbidScheme = nameof(ApiResponseHandler);
            })
             // 添加JwtBearer服务
             .AddJwtBearer(o =>
             {
                 o.TokenValidationParameters = tokenValidationParameters;
                 o.Events = new JwtBearerEvents
                 {
                     OnChallenge = context =>
                     {
                         context.Response.Headers.Add("Token-Error", context.ErrorDescription);
                         return Task.CompletedTask;
                     },
                     OnAuthenticationFailed = context =>
                     {
                         var jwtHandler = new JwtSecurityTokenHandler();
                         var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                         if (!token.IsNullOrEmpty() && jwtHandler.CanReadToken(token))
                         {
                             var jwtToken = jwtHandler.ReadJwtToken(token);

                             if (jwtToken.Issuer != Issuer)
                             {
                                 context.Response.Headers.Add("Token-Error-Iss", "issuer is wrong!");
                             }

                             if (jwtToken.Audiences.FirstOrDefault() != Audience)
                             {
                                 context.Response.Headers.Add("Token-Error-Aud", "Audience is wrong!");
                             }
                         }

                         // 如果过期，则把<是否过期>添加到，返回头信息中
                         if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                         {
                             context.Response.Headers.Add("Token-Expired", "true");
                         }
                         return Task.CompletedTask;
                     }
                 };
             })
             .AddScheme<AuthenticationSchemeOptions, ApiResponseHandler>(nameof(ApiResponseHandler), o => { });
        }
    }
}