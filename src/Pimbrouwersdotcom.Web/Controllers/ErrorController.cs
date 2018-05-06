using Microsoft.AspNetCore.Mvc;
using Pimbrouwersdotcom.Web.Models;
using System.Net;

namespace Pimbrouwersdotcom.Web.Controllers
{
  public class ErrorController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Code(int id)
    {
      return View(new ErrorStatusCodeModel()
      {
        StatusCode = id,
        PageTitle = $"{id} Error"
      });
    }
  }
}