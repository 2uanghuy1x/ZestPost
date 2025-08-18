
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ZestPost.Base;

namespace ZestPost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        // POST: api/Comment/CommentByProfile
        [HttpPost("CommentByProfile")]
        public async Task<IActionResult> CommentByProfile([FromBody] object payload)
        {
            // Implementation for commenting by profile
            return Ok(new ViModelSync { status = true, message = "CommentByProfile successful" });
        }

        // POST: api/Comment/CommentByPage
        [HttpPost("CommentByPage")]
        public async Task<IActionResult> CommentByPage([FromBody] object payload)
        {
            // Implementation for commenting by page
            return Ok(new ViModelSync { status = true, message = "CommentByPage successful" });
        }

        // POST: api/Comment/CommentByGroup
        [HttpPost("CommentByGroup")]
        public async Task<IActionResult> CommentByGroup([FromBody] object payload)
        {
            // Implementation for commenting by group
            return Ok(new ViModelSync { status = true, message = "CommentByGroup successful" });
        }
    }
}
