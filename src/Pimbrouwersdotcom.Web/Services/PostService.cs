using LunchPail;
using Pimbrouwersdotcom.Data;
using Pimbrouwersdotcom.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pimbrouwersdotcom.Web.Services
{
  public class PostService
  {
    private readonly IDbContext db;
    private readonly PostRepository postRepository;

    public PostService(
      IDbContext db,
      PostRepository postRepository)
    {
      this.db = db;
      this.postRepository = postRepository;
    }

    public async Task<IEnumerable<Post>> Page(DateTime? dt = null, string order = "desc", int take = 16)
    {
      var posts = await postRepository.Page(dt, order, take);
      db.Commit();

      return posts;
    }
  }
}