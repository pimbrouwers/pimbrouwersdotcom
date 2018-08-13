using Pimbrouwersdotcom.Data;
using Pimbrouwersdotcom.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pimbrouwersdotcom.Web.Models
{
  public class PostIndexModel : PageModel
  {
    public DateTime? dt { get; set; }

    public string order { get; set; }

    public IEnumerable<Post> Posts { get; set; }

    public DateTime? LastDt =>
      Posts?.LastOrDefault().Dt;
  }
}