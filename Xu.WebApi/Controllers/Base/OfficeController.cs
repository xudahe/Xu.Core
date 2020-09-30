using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xu.WebApi.Controllers
{
    [Route("api/[Controller]/[action]")]
    [ApiController]
    public class OfficeController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public OfficeController(IWebHostEnvironment env)
        {
            _env = env;
        }
    }
}