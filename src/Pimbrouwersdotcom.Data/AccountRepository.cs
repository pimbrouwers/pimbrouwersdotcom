using Dapper.UnitOfWork;
using LunchPail;
using Pimbrouwersdotcom.Domain;
using Sequel;
using SimpleCrypto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pimbrouwersdotcom.Data
{
  public class AccountRepository : AbstractRepository<Account>
  {
    public AccountRepository(
      IDbContext dbContext,
      ISqlMapper<Account> sqlMapper)
      : base(dbContext, sqlMapper)
    {
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

    private async Task<Account> FindByUsername(string username)
    {
      var sql = new SqlBuilder()
        .Select(sqlMapper.Fields)
        .From(sqlMapper.Table)
        .Where("Username like @username")
        .Limit(1)
        .ToSql();

      return await QueryFirstOrDefault(sql, new { username });
    }
  }
}