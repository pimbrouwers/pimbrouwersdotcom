using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pimbrouwersdotcom.Data;
using Pimbrouwersdotcom.Web.Models;
using Pimbrouwersdotcom.Web.Services;
using System.Threading.Tasks;

namespace Pimbrouwersdotcom.Web.Controllers
{
  public class PostController : Controller
  {
    private readonly PostService postService;
    private readonly ILogger logger;

    public PostController(
      PostService postService,
      ILogger<PostController> logger)
    {
      this.postService = postService;
      this.logger = logger;
    }

    public async Task<IActionResult> Index(PostIndexModel model)
    {
      var posts = await postService.Page(model.dt, model.order);

      model.Posts = posts;
      model.PageTitle = "Blog";

      return View(model);
    }
  }
}