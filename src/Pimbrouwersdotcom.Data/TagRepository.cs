using Pimbrouwersdotcom.Domain;
using Sequel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Pimbrouwersdotcom.Data
{
  public class TagRepository : Repository<Tag>
  {
    public TagRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<int> Create(Tag tag, IDbTransaction transaction = null, int? commandTimeout = null)
    {
      var existing = await FindByLabel(tag.Label);

      return existing?.Id ?? await CreateEntity(tag, transaction, commandTimeout);
    }

    public async Task<Tag> FindByLabel(string label, int? commandTimeout = null)
    {
      var sql = new SqlBuilder()
        .Select("*")
        .From(table.Name)
        .Where("Label like @label")
        .Limit(1)
        .ToSql();

      return await QueryFirstOrDefault<Tag>(sql, new { label = $"{label}%" }, commandTimeout: commandTimeout);
    }

    public async Task<IEnumerable<Tag>> FirstByPostId(int postId, int? commandTimeout = null)
    {
      var sql = new SqlBuilder()
        .Select($"{table.Name}.*")
        .From(table.Name)
        .Join($"PostTag on PostTag.TagId = {table.Name}.Id")
        .Where("PostTag.PostId = @postId")
        .ToSql();

      return await Query(sql, new { postId }, commandTimeout: commandTimeout);
    }
  }
}