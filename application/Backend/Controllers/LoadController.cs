using Backend.Load;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoadController : ControllerBase
    {
        /// <summary>
        /// Generates CPU load
        /// </summary>
        /// <param name="percentage"></param>
        /// <param name="durationInSeconds"></param>
        /// <returns></returns>
        [HttpPost("cpu")]
        public ActionResult GenerateCPULoad(int percentage, int durationInSeconds)
        {
            if (percentage < 0 || percentage > 100 || durationInSeconds <= 0)
            {
                return BadRequest();
            }

            new CPULoadGenerator().GenerateLoad(percentage, durationInSeconds);
            return Ok();
        }

        /// <summary>
        /// Allocates the given amount of memory. Garbage collection is only performed when needed (extreme low memory).
        /// </summary>
        /// <param name="megabytes"></param>
        /// <param name="durationInSeconds"></param>
        /// <returns></returns>
        [HttpPost("memory")]
        public ActionResult GenerateMemoryLoad(int megabytes, int durationInSeconds)
        {
            if(megabytes <= 0 || durationInSeconds <= 0)
            {
                return BadRequest();
            }

            new MemoryLoadGenerator().GenerateLoad(megabytes, durationInSeconds);
            return Ok();
        }
    }
}
