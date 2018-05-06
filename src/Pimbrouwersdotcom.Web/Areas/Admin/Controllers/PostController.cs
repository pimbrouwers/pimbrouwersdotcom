using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pimbrouwersdotcom.Data;
using Pimbrouwersdotcom.Domain;
using Pimbrouwersdotcom.Web.Areas.Admin.Models;
using System;
using System.Threading.Tasks;

namespace Pimbrouwersdotcom.Web.Areas.Admin.Controllers
{
  [Area("Admin")]
  [Authorize]
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

    [HttpGet]
    public async Task<IActionResult> Index(PostIndexModel model)
    {
      var posts = await db.Post.Page(model.dt, take: 10);

      if (posts == null)
      {
        return BadRequest();
      }

      model.Posts = posts;
      return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
      return View(new PostCreateModel()
      {
        Post = new Post()
        {
          Dt = DateTime.Now
        }
      });
    }

    [HttpPost]
    public async Task<IActionResult> Create(PostCreateModel model)
    {
      if (ModelState.IsValid)
      {
        try
        {
          int postId = await db.Post.Create(model.Post);

          if (!string.IsNullOrWhiteSpace(model.Tags))
          {
            var tags = model.Tags.Split(",");

            foreach (var tag in tags)
            {
              int tagId = await db.Tag.Create(new Tag() { Label = tag.Trim() });

              await db.Post.AddTag(postId, tagId);
            }
          }

          return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
          logger.LogError(ex, "Unable to create post {@model}");
          ModelState.AddModelError("", "Unable to create post.");
        }
      }

      return View();
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      var post = await db.Post.Read(id);

      if (post == null)
      {
        return NotFound();
      }

      post.Tags = await db.Tag.FirstByPostId(id);

      return View(new PostEditModel()
      {
        Post = post
      });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, PostEditModel model)
    {
      if (id != model.Post.Id)
      {
        return NotFound();
      }

      try
      {
        await db.Post.Update(model.Post);

        if (!string.IsNullOrWhiteSpace(model.Tags))
        {
          var tags = model.Tags.Split(",");

          foreach (var tag in tags)
          {
            int tagId = await db.Tag.Create(new Tag() { Label = tag.Trim() });

            await db.Post.AddTag(id, tagId);
          }
        }

        return RedirectToAction("Index");
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Unable to create post {@model}");
        ModelState.AddModelError("", "Unable to create post.");
      }

      return View(model);
    }
  }
}