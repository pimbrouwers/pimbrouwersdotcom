using System;

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

    protected async Task<IEnumerable<TEntity>> Query(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
    {
      if (transaction != null) return await transaction.Connection.QueryAsync<TEntity>(sql, param, transaction, commandType: commandType);

      using (var conn = await connectionFactory.CreateOpenConnection())
      {
        return await conn.QueryAsync<TEntity>(sql, param, transaction, commandType: commandType);
      }
    }
  }
}
