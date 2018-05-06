using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pimbrouwersdotcom.Web.Areas.Admin.Models;
using Pimbrouwersdotcom.Data;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Pimbrouwersdotcom.Web.Areas.Admin.Controllers
{
  [Area("Admin")]
  public class AccountController : Controller
  {
    private readonly DbContext db;
    private readonly ILogger logger;

    public AccountController(
      DbContext db,
      ILogger<PostController> logger)
    {
      this.db = db;
      this.logger = logger;
    }

    [HttpGet]
    public IActionResult Login()
    {
      if (HttpContext.User.Identity.IsAuthenticated)
      {
        return RedirectToAction("Index", "Post");
      }

      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(AccountLoginModel model)
    {
      if (HttpContext.User.Identity.IsAuthenticated)
      {
        return RedirectToAction("Index", "Post");
      }

      try
      {
        var account = await db.Account.Login(model.Username, model.Password);

        if (account == null)
        {
          ModelState.AddModelError("", "Invalid username/password.");
        }
        else
        {
          var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString(), ClaimValueTypes.Integer32),
            new Claim(ClaimTypes.Authentication, account.Username, ClaimValueTypes.String),
          };

          var userIdentity = new ClaimsIdentity(claims, "pimbrouwersdotcom");

          await HttpContext.SignInAsync("pimbrouwersdotcom", new ClaimsPrincipal(userIdentity));

          return RedirectToAction("Index", "Post");
        }
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Unable to login {@model}");
        ModelState.AddModelError("", "Invalid username/password.");
      }

      return View(model);
    }
  }
}