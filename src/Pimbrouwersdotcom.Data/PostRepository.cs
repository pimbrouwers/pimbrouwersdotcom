using Dapper.UnitOfWork;
using LunchPail;
using Sequel;
using Pimbrouwersdotcom.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;

namespace Pimbrouwersdotcom.Data
{
  public class PostRepository : AbstractRepository<Post>
  {
    public PostRepository(
      IDbContext dbContext,
      ISqlMapper<Post> sqlMapper)
      : base(dbContext, sqlMapper)
    {
    }

    public async Task<int> Create(Post post, IDbTransaction transaction = null)
    {
      return await CreateEntity(post);
    }

    public async Task<Post> Read(int id)
    {
      return await ReadEntity(id);
    }

    public async Task<bool> Update(Post post, IDbTransaction transaction = null)
    {
      return await UpdateEntity(post);
    }

    public async Task<bool> Delete(Post post, IDbTransaction transaction = null)
    {
      return await DeleteEntity(post);
    }

    public async Task<bool> AddTag(int postId, int tagId, IDbTransaction transaction = null)
    {
      var sql = new SqlBuilder()
        .Insert("PostTag")
        .Columns("PostId, TagId")
        .Values("@postId", "@tagId")
        .ToSql();

      return (await ExecuteScalar<int>(sql, new { postId, tagId })) == 1;
    }

    public async Task<bool> DeleteTag(int postId, int tagId, IDbTransaction transaction = null)
    {
      var sql = new SqlBuilder()
        .Delete()
        .From("PostTag")
        .Where("PostId = @postId", "TagId = @tagId")
        .ToSql();

      return (await ExecuteScalar<int>(sql, new { postId, tagId })) == 1;
    }

    public async Task<IEnumerable<Post>> Page(DateTime? dt = null, string order = "desc", int take = 16)
    {
      var sqlBuilder = new SqlBuilder()
        .Select("*")
        .From("Post")
        .Limit(take);

      if (dt.HasValue)
      {
        sqlBuilder.Where("Dt < @dt");
      }

      if (string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase))
      {
        sqlBuilder.OrderByDesc("Dt");
      }
      else
      {
        sqlBuilder.OrderBy("Dt");
      }

      return await Query(sqlBuilder.ToSql(), new { dt });
    }
  }
}