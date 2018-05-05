using Microsoft.Data.Sqlite;
using Pimbrouwersdotcom.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Pimbrouwersdotcom.Web
{
  public class SqliteConnectionFactory : IDbConnectionFactory
  {
    private readonly string connectionString;

    public SqliteConnectionFactory(string connectionString)
    {
      if (string.IsNullOrWhiteSpace(connectionString))
      {
        throw new ArgumentNullException("ConnectionString cannot be null");
      }

      this.connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateOpenConnection()
    {
      var conn = new SqliteConnection(connectionString);

      try
      {
        if (conn.State != ConnectionState.Open)
          await conn.OpenAsync();
      }
      catch (Exception exception)
      {
        throw new Exception("An error occured while connecting to the database. See innerException for details.", exception);
      }

      return conn;
    }
  }
}