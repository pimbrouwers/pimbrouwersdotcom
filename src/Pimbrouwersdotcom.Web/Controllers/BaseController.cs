using Microsoft.Extensions.Logging;
using Pimbrouwersdotcom.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pimbrouwersdotcom.Web.Controllers
{
  public abstract class BaseController
  {
    private readonly DbContext db;
    private readonly ILogger logger;

    public BaseController(
      DbContext db,
      ILogger<PostController> logger)
    {
      this.db = db;
      this.logger = logger;
    }
  }
}