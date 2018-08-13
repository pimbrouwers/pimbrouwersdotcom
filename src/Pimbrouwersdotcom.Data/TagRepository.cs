using Dapper.UnitOfWork;
using LunchPail;
using Pimbrouwersdotcom.Domain;
using Sequel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Pimbrouwersdotcom.Data
{
  public class TagRepository : AbstractRepository<Tag>
  {
    public TagRepository(
      IDbContext dbContext,
      ISqlMapper<Tag> sqlMapper)
      : base(dbContext, sqlMapper)
    {
    }

    public async Task<int> Create(Tag tag, IDbTransaction transaction = null)
    {
      var existing = await FirstBy("Label", tag.Label);

      return existing?.Id ?? await CreateEntity(tag);
    }

    public async Task<Tag> FindByLabel(string label)
    {
      return await FirstLike("Label", $"{label}%");
    }

    public async Task<IEnumerable<Tag>> FindByPostId(int postId)
    {
      var sql = new SqlBuilder()
        .Select(sqlMapper.Fields)
        .From(sqlMapper.Table)
        .Join($"PostTag on PostTag.TagId = {sqlMapper.Table}.Id")
        .Where("PostTag.PostId = @postId")
        .ToSql();

      return await Query(sql, new { postId });
    }
  }
}