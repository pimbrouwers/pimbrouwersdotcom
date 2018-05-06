using Pimbrouwersdotcom.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pimbrouwersdotcom.Web.Areas.Admin.Models
{
  public class PostIndexModel
  {
    public DateTime? dt { get; set; }

    public IEnumerable<Post> Posts { get; set; }

    public DateTime? LastDt =>
      Posts?.LastOrDefault().Dt;
  }
}