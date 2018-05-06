using Pimbrouwersdotcom.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pimbrouwersdotcom.Web.Models
{
  public enum BlogIndexOrder
  {
    Desc,
    Asc
  }

  public class BlogIndexModel : PageModel
  {
    public DateTime? dt { get; set; }

    public BlogIndexOrder o { get; set; }

    public IEnumerable<Post> Posts
    { get; set; }

    public DateTime? LastDt =>
      Posts?.LastOrDefault().Dt;
  }
}