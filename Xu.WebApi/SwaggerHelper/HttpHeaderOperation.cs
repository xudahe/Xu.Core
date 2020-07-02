using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace Xu.WebApi.SwaggerHelper
{
    public class HttpHeaderOperation : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            throw new NotImplementedException();
        }
    }
}