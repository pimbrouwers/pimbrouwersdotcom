using Dapper;
using Sequel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Pimbrouwersdotcom.Data
{
  public abstract class Repository<TEntity> where TEntity : class
  {
    private readonly IDbConnectionFactory connectionFactory;
    protected readonly Table<TEntity> table;

    protected Repository(
      IDbConnectionFactory connectionFactory)
    {
      this.connectionFactory = connectionFactory;
      this.table = new Table<TEntity>();
    }

    protected async Task<int> Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text, IDbConnection conn = null)
    {
      if (conn != null) return await conn.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
      else if (transaction != null) return await transaction.Connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);

      using (conn = await connectionFactory.CreateOpenConnection())
      {
        return await conn.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
      }
    }

    protected async Task<T> ExecuteScalar<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text, IDbConnection conn = null)
    {
      if (conn != null) return await conn.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType);
      else if (transaction != null) return await conn.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType);

      using (conn = await connectionFactory.CreateOpenConnection())
      {
        return await conn.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType);
      }
    }

    protected async Task<T> QuerySingle<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
    {
      if (transaction != null) return await transaction.Connection.QuerySingleAsync<T>(sql, param, transaction, commandType: commandType);

      using (var conn = await connectionFactory.CreateOpenConnection())
      {
        return await conn.QuerySingleAsync<T>(sql, param, transaction, commandType: commandType);
      }
    }

    protected async Task<T> QueryFirstOrDefault<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
    {
      if (transaction != null) return await transaction.Connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandType: commandType);

      using (var conn = await connectionFactory.CreateOpenConnection())
      {
        return await conn.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandType: commandType);
      }
    }

    protected async Task<IEnumerable<TEntity>> Query(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
    {
      if (transaction != null) return await transaction.Connection.QueryAsync<TEntity>(sql, param, transaction, commandType: commandType);

      using (var conn = await connectionFactory.CreateOpenConnection())
      {
        return await conn.QueryAsync<TEntity>(sql, param, transaction, commandType: commandType);
      }
    }

    protected async Task<int> CreateEntity(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null)
    {
      return await QuerySingle<int>($"{table.CreateSql.ToSql()}; select last_insert_rowid();", entity, transaction, commandTimeout);
    }

    protected async Task<TEntity> ReadEntity(object key, int? commandTimeout = null)
    {
      return await QueryFirstOrDefault<TEntity>(table.GetSql.Where($"{table.Key} = @key").Limit(1).ToSql(), new { key }, commandTimeout: commandTimeout);
    }

    protected async Task<bool> UpdateEntity(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null)
    {
      return (await Execute(table.UpdateSql.ToSql(), entity, transaction, commandTimeout)) == 1;
    }

    protected async Task<bool> DeleteEntity(TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null)
    {
      return (await Execute(table.DeleteSql.ToSql(), entity, transaction, commandTimeout)) == 1;
    }
  }

  public class Table<TEntity>
  {
    private Type entityType;

    private string name;
    private string key = "Id";
    private string[] fields;
    private string[] nonKeyFields;

    public Table()
    {
      entityType = typeof(TEntity);
    }

    public virtual string Name
    {
      get
      {
        if (string.IsNullOrWhiteSpace(name))
        {
          name = entityType.Name;
        }

        return name;
      }
      set
      {
        name = value;
      }
    }

    public virtual string Key
    {
      get
      {
        return key;
      }
      set
      {
        key = value;
      }
    }

    public string[] Fields
    {
      get
      {
        if (fields == null)
        {
          fields = entityType.GetProperties().Where(p =>
            p.CanWrite
            && p.PropertyType.IsPublic
            && string.Equals(p.PropertyType.Namespace, "system", StringComparison.OrdinalIgnoreCase)
            && !typeof(ICollection<>).IsAssignableFrom(p.PropertyType)
          ).Select(p => p.Name).ToArray();
        }

        return fields;
      }
    }

    public string[] NonKeyFields
    {
      get
      {
        if (nonKeyFields == null)
        {
          nonKeyFields = Fields.Where(f => !string.Equals(f, Key, StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        return nonKeyFields;
      }
    }

    #region CRUD Sql Builders

    public SqlBuilder GetSql
    {
      get
      {
        return new SqlBuilder()
        .Select(Fields)
        .From(Name);
      }
    }

    public SqlBuilder CreateSql
    {
      get
      {
        return new SqlBuilder()
        .Insert(Name)
        .Columns(NonKeyFields)
        .Values(string.Join(",", NonKeyFields.Select(f => $"@{f}")));
      }
    }

    public SqlBuilder UpdateSql
    {
      get
      {
        return new SqlBuilder()
        .Update(Name)
        .Set(NonKeyFields.Select(f => $"{f} = @{f}").ToArray())
        .Where($"{Key} = @{Key}");
      }
    }

    public SqlBuilder DeleteSql
    {
      get
      {
        return new SqlBuilder()
        .Delete()
        .From(Name)
        .Where($"{Key} = @{Key}");
      }
    }

    #endregion CRUD Sql Builders
  }
}