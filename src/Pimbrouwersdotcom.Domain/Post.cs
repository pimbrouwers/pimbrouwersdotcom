using FluentValidation;
using System;

namespace Pimbrouwersdotcom.Domain
{
  public class Post
  {
    public int Id { get; set;}
    public string Title { get; set; }
    public string Tldr { get; set; }
    public string Body { get; set; }
    public DateTime Created { get; set; }
  
    public IEnumerable<Tag> Tags { get; set; }
  }
}
