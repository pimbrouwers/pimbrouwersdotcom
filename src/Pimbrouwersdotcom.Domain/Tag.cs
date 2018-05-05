using FluentValidation; 

namespace Pimbrouwersdotcom.Domain
{
  public class Tag
  {
    public int Id { get; set; }
    public string Label { get; set; }
  }

  public class TagValidator : AbstractValidator<Tag>
  {
    public TagValidator()
    {
      RuleFor(t => t.Label)
        .MaximumLength(255);
    }
  }
}