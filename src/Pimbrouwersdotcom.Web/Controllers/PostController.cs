using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pimbrouwersdotcom.Data;
using Pimbrouwersdotcom.Web.Models;
using System.Threading.Tasks;

namespace Pimbrouwersdotcom.Web.Controllers
{
  public class PostController : Controller
  {
    private readonly DbContext db;
    private readonly ILogger logger;

    public PostController(
      DbContext db,
      ILogger<PostController> logger)
    {
      this.db = db;
      this.logger = logger;
    }

    public async Task<IActionResult> Index(PostIndexModel model)
    {
      var posts = await db.Post.Page(model.dt, model.order);

      model.Posts = posts;
      model.PageTitle = "Blog";

      return View(model);
    }
  }
}