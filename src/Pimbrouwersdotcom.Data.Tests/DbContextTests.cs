using Microsoft.Data.Sqlite;
using Pimbrouwersdotcom.Domain;
using Pimbrouwersdotcom.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Pimbrouwersdotcom.Data.Tests
{
  public class DbContextTests
  {
    private IDbConnectionFactory connectionFactory;
    private DbContext db;

    public DbContextTests()
    {
      connectionFactory = new SqliteConnectionFactory("data source=./../../../../../pimbrouwersdotcom_test.sqlite3");
      db = new DbContext(connectionFactory);

      string schema = System.IO.File.ReadAllText("./../../../../../schema.sql");

      using (var conn = db.ConnectionFactory.CreateOpenConnection().Result)
      {
        var cmd = new SqliteCommand(schema, conn as SqliteConnection);
        cmd.ExecuteNonQuery();
      }
    }

    [Fact]
    public async Task CreatePost()
    {
      int id = await db.Post.Create(new Post()
      {
        Title = "Title",
        Tldr = "test test test",
        Body = "sljkdsaj dslkjsalkjdsa lkkjdlksajd lkdlkjdslkja"
      });

      var post = await db.Post.Read(id);

      Assert.Equal(id, post.Id);
    }

    [Fact]
    public async Task CreatePostAndUpdate()
    {
      int id = await db.Post.Create(new Post()
      {
        Title = "Title2",
        Tldr = "test test test",
        Body = "sljkdsaj dslkjsalkjdsa lkkjdlksajd lkdlkjdslkja"
      });

      var post = await db.Post.Read(id);

      post.Title = "Title3";

      bool updated = await db.Post.Update(post);

      post = await db.Post.Read(id);

      Assert.True(updated);
      Assert.Equal(id, post.Id);
    }

    [Fact]
    public async Task CreatePostAndDelete()
    {
      int id = await db.Post.Create(new Post()
      {
        Title = "Title2",
        Tldr = "test test test",
        Body = "sljkdsaj dslkjsalkjdsa lkkjdlksajd lkdlkjdslkja"
      });

      bool deleted = await db.Post.Delete(new Post() { Id = id });
      var post = await db.Post.Read(id);

      Assert.True(deleted);
      Assert.Null(post);
    }

    [Fact]
    public async Task CreatePostWithTags()
    {
      var post = new Post()
      {
        Title = "Title",
        Tldr = "test test test",
        Body = "sljkdsaj dslkjsalkjdsa lkkjdlksajd lkdlkjdslkja",
        Tags = new HashSet<Tag>()
        {
          new Tag()
          {
            Label = "Test1"
          },
          new Tag()
          {
            Label = "Test2"
          },
          new Tag()
          {
            Label = "Test3"
          }
        }
      };

      int postId = await db.Post.Create(post);

      foreach (var tag in post.Tags)
      {
        int id = await db.Tag.Create(tag);

        await db.Post.AddTag(postId, id);
      }

      var postAfterCreate = await db.Post.Read(postId);
      var tags = await db.Tag.FirstByPostId(postId);

      Assert.Equal(postId, postAfterCreate.Id);
      Assert.Equal(3, tags.Count());
    }

    [Fact]
    public async Task CreateTag()
    {
      var id = await db.Tag.Create(new Tag()
      {
        Label = "CreateTag"
      });

      var tag = await db.Tag.FindByLabel("CreateTa");

      Assert.Equal(id, tag.Id);
    }

    [Fact]
    public async Task PageDescPost()
    {
      int id = await db.Post.Create(new Post()
      {
        Title = "Title",
        Tldr = "test test test",
        Body = "sljkdsaj dslkjsalkjdsa lkkjdlksajd lkdlkjdslkja",
        Dt = new DateTime(2018, 1, 1)
      });

      int id2 = await db.Post.Create(new Post()
      {
        Title = "Title2",
        Tldr = "test test test",
        Body = "sljkdsaj dslkjsalkjdsa lkkjdlksajd lkdlkjdslkja",
        Dt = new DateTime(2018, 2, 1)
      });

      int id3 = await db.Post.Create(new Post()
      {
        Title = "Title3",
        Tldr = "test test test",
        Body = "sljkdsaj dslkjsalkjdsa lkkjdlksajd lkdlkjdslkja",
        Dt = new DateTime(2018, 3, 1)
      });

      var posts = await db.Post.Page(new DateTime(2018, 3, 1));

      Assert.Equal(2, posts.Count());
    }

    [Fact]
    public async Task UpdatePost()
    {
      var post = new Post()
      {
        Title = "Title",
        Tldr = "test test test",
        Body = "sljkdsaj dslkjsalkjdsa lkkjdlksajd lkdlkjdslkja",
        Tags = new HashSet<Tag>()
        {
          new Tag()
          {
            Label = "Test1"
          },
          new Tag()
          {
            Label = "Test2"
          }
        }
      };

      int postId = await db.Post.Create(post);

      foreach (var tag in post.Tags)
      {
        int id = await db.Tag.Create(tag);

        await db.Post.AddTag(postId, id);
      }

      var postAfterCreate = await db.Post.Read(postId);
      var tags = await db.Tag.FirstByPostId(postId);

      Assert.Equal(postId, postAfterCreate.Id);
      Assert.Equal(2, tags.Count());

      postAfterCreate.Title = "Title2";

      await db.Post.Update(postAfterCreate);

      var postAfterUpdate = await db.Post.Read(postId);

      Assert.Equal("Title2", postAfterUpdate.Title);

      await db.Post.DeleteTag(postId, tags.LastOrDefault().Id);

      var tagsAfterDelete = await db.Tag.FirstByPostId(postId);

      Assert.Single(tagsAfterDelete);
    }
  }
}