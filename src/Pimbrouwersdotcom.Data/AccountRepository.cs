using Pimbrouwersdotcom.Domain;
using Sequel;
using SimpleCrypto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pimbrouwersdotcom.Data
{
  public class AccountRepository : Repository<Account>
  {
    public AccountRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<Account> FindByUsername(string username, int? commandTimeout = null)
    {
      var sql = new SqlBuilder()
        .Select("*")
        .From(table.Name)
        .Where("Username like @username")
        .Limit(1)
        .ToSql();

      return await QueryFirstOrDefault<Account>(sql, new { username }, commandTimeout: commandTimeout);
    }

    public async Task<Account> Login(string username, string password)
    {
      ICryptoService cryptoService = new PBKDF2();
      string hashedPassword = string.Empty;

      var potentialAccount = await FindByUsername(username);

      if (!string.IsNullOrWhiteSpace(potentialAccount?.Salt))
      {
        hashedPassword = cryptoService.Compute(password, potentialAccount.Salt);
      }

      if (string.IsNullOrWhiteSpace(potentialAccount?.Password) ||
        string.IsNullOrWhiteSpace(hashedPassword) ||
        !cryptoService.Compare(potentialAccount.Password, hashedPassword))
      {
        potentialAccount = null;
      }
      else
      {
        potentialAccount.Password = null;
        potentialAccount.Salt = null;
      }

      return potentialAccount;
    }
  }
}