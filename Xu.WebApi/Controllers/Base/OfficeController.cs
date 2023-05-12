using Microsoft.AspNetCore.Mvc;

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