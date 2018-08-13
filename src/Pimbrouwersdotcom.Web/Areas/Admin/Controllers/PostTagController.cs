using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pimbrouwersdotcom.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Pimbrouwersdotcom.Web.Areas.Admin.Controllers
{
  [Area("Admin")]
  [Authorize]
  public class PostTagController : Controller
  {
    private readonly PostRepository postRepository;
    private readonly ILogger logger;

    public PostTagController(
      PostRepository postRepository,
      ILogger<PostTagController> logger)
    {
      this.postRepository = postRepository;
      this.logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int postId, int tagId)
    {
      try
      {
        await postRepository.DeleteTag(postId, tagId);
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Unable to delete post-tag.");
      }

      return RedirectToAction("Edit", "Post", new { id = postId });
    }
  }
}