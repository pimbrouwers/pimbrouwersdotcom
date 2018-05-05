using Sequel.SqlBuilder;
using System;

namespace Pimbrouwersdotcom.Data
{
  public class PostRepository
  {
    public PostRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {

    }   
  }
}