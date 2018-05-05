using System.Data;
using System.Threading.Tasks;

namespace Pimbrouwersdotcom.Data
{
  public interface IDbConnectionFactory
  {
    Task<IDbConnection> CreateOpenConnection();
  }
}