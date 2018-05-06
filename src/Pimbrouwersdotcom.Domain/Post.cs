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
    public DateTime Dt { get; set; } = DateTime.Now;

    public IEnumerable<Tag> Tags { get; set; }
  }

  public class PostValidator : AbstractValidator<Post>
  {
    public PostValidator()
    {
      RuleFor(p => p.Title)
        .NotEmpty()
        .MaximumLength(100);

      RuleFor(p => p.Tldr)
        .NotEmpty()
        .MaximumLength(255);

      RuleFor(p => p.Body)
        .NotEmpty()
        .MaximumLength(4000);

      RuleFor(p => p.Dt)
        .NotEmpty();
    }
  }
}