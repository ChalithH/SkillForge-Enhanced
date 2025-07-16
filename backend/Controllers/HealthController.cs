using Microsoft.AspNetCore.Mvc;

namespace SkillForge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public ActionResult<object> Get()
        {
            return Ok(new 
            { 
                status = "healthy",
                service = "SkillForge API",
                timestamp = DateTime.UtcNow
            });
        }
    }
}