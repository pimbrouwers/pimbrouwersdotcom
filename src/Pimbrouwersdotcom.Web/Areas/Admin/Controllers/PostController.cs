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
    private readonly PostRepository postRepository;
    private readonly TagRepository tagRepository;
    private readonly ILogger logger;

    public PostController(
      PostRepository postRepository,
      TagRepository tagRepository,
      ILogger<PostController> logger)
    {
      this.postRepository = postRepository;
      this.tagRepository = tagRepository;
      this.logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(PostIndexModel model)
    {
      var posts = await postRepository.Page(model.dt, take: 10);

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
          int postId = await postRepository.Create(model.Post);

          if (!string.IsNullOrWhiteSpace(model.Tags))
          {
            var tags = model.Tags.Split(",");

            foreach (var tag in tags)
            {
              int tagId = await tagRepository.Create(new Tag() { Label = tag.Trim() });

              await postRepository.AddTag(postId, tagId);
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
      var post = await postRepository.Read(id);

      if (post == null)
      {
        return NotFound();
      }

      post.Tags = await tagRepository.FindByPostId(id);

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
        await postRepository.Update(model.Post);

        if (!string.IsNullOrWhiteSpace(model.Tags))
        {
          var tags = model.Tags.Split(",");

          foreach (var tag in tags)
          {
            int tagId = await tagRepository.Create(new Tag() { Label = tag.Trim() });

            await postRepository.AddTag(id, tagId);
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