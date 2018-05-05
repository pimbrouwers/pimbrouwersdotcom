namespace Pimbrouwersdotcom.Data
{
  public interface IDbConnectionFactory
  {
    Task<IDbConnection> CreateOpenConnection();
  }
}