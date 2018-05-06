namespace Pimbrouwersdotcom.Data
{
  public enum OrderBy
  {
    Desc,
    Asc
  }

  public class DbContext
  {
    private AccountRepository account;
    private PostRepository post;
    private TagRepository tag;

    public DbContext(IDbConnectionFactory connectionFactory)
    {
      ConnectionFactory = connectionFactory;
    }

    public IDbConnectionFactory ConnectionFactory { get; }

    public AccountRepository Account
    {
      get
      {
        if (account == null)
        {
          account = new AccountRepository(ConnectionFactory);
        }

        return account;
      }
    }

    public PostRepository Post
    {
      get
      {
        if (post == null)
        {
          post = new PostRepository(ConnectionFactory);
        }

        return post;
      }
    }

    public TagRepository Tag
    {
      get
      {
        if (tag == null)
        {
          tag = new TagRepository(ConnectionFactory);
        }

        return tag;
      }
    }
  }
}