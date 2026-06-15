using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_GRUPODOS.Models;

namespace API_GRUPODOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        [HttpPost("RegistrarAPI")]
        public IActionResult RegistrarAPI(RegistroUsuarioRequestModel model)
        {
            return Ok();
        }
    }
}


