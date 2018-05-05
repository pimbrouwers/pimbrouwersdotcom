namespace Pimbrouwersdotcom.Data
{
  public class DbContext
  {
    private PostRepository post;
    private TagRepository tag;

    public DbContext(IDbConnectionFactory connectionFactory)
    {
      ConnectionFactory = connectionFactory;
    }

    public IDbConnectionFactory ConnectionFactory { get; }

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