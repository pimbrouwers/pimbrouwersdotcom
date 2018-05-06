using Pimbrouwersdotcom.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pimbrouwersdotcom.Web.Areas.Admin.Models
{
  public class PostCreateModel
  {
    public Post Post { get; set; }
    public string Tags { get; set; }
  }
}