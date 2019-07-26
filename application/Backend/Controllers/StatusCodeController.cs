using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusCodeController : ControllerBase
    {
        /// <summary>
        /// Genreates an empty response with the given status code.
        /// Could be used to monitor the response codes with Prometheus.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("{code}")]
        public ActionResult GenerateHttpStatusCodeResponse(int code)
        {
            if (code <= 0)
            {
                return BadRequest();
            }

            return StatusCode(code);
        }
    }
}
