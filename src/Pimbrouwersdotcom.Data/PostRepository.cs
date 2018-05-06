using Sequel;
using Pimbrouwersdotcom.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;

namespace Pimbrouwersdotcom.Data
{
  public class PostRepository : Repository<Post>
  {
    public PostRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<int> Create(Post post, IDbTransaction transaction = null, int? commandTimeout = null)
    {
      return await CreateEntity(post, transaction, commandTimeout);
    }

    public async Task<Post> Read(int id, int? commandTimeout = null)
    {
      return await ReadEntity(id, commandTimeout);
    }

    public async Task<bool> Update(Post post, IDbTransaction transaction = null, int? commandTimeout = null)
    {
      return await UpdateEntity(post, transaction, commandTimeout);
    }

    public async Task<bool> Delete(Post post, IDbTransaction transaction = null, int? commandTimeout = null)
    {
      return await DeleteEntity(post, transaction, commandTimeout);
    }

    public async Task<bool> AddTag(int postId, int tagId, IDbTransaction transaction = null, int? commandTimeout = null)
    {
      var sql = new SqlBuilder()
        .Insert("PostTag")
        .Columns("PostId, TagId")
        .Values("@postId", "@tagId")
        .ToSql();

      return (await ExecuteScalar<int>(sql, new { postId, tagId }, transaction, commandTimeout)) == 1;
    }

    public async Task<bool> DeleteTag(int postId, int tagId, IDbTransaction transaction = null, int? commandTimeout = null)
    {
      var sql = new SqlBuilder()
        .Delete()
        .From("PostTag")
        .Where("PostId = @postId", "TagId = @tagId")
        .ToSql();

      return (await ExecuteScalar<int>(sql, new { postId, tagId }, transaction, commandTimeout)) == 1;
    }

    public async Task<IEnumerable<Post>> Page(DateTime? dt = null, OrderBy order = OrderBy.Desc, int take = 3, int? commandTimeout = null)
    {
      var sqlBuilder = new SqlBuilder()
        .Select("*")
        .From("Post")
        .Limit(take);

      if (dt.HasValue)
      {
        sqlBuilder.Where("Dt < @dt");
      }

      if (order == OrderBy.Desc)
      {
        sqlBuilder.OrderByDesc("Dt");
      }
      else
      {
        sqlBuilder.OrderBy("Dt");
      }

      return await Query(sqlBuilder.ToSql(), new { dt }, commandTimeout: commandTimeout);
    }
  }
}