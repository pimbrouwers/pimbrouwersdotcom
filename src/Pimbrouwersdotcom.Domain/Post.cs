using FluentValidation;
using System;
using System.Collections.Generic;

namespace Pimbrouwersdotcom.Domain
{
  public class Post
  {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Tldr { get; set; }
    public string Body { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;

    public IEnumerable<Tag> Tags { get; set; }
  }

  public class PostValidator : AbstractValidator<Post>
  {
    public PostValidator()
    {
      RuleFor(p => p.Title)
        .MaximumLength(100);

      RuleFor(p => p.Tldr)
        .MaximumLength(255);

      RuleFor(p => p.Body)
        .MaximumLength(4000);
    }
  }
}