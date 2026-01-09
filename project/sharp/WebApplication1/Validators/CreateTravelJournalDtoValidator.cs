using FluentValidation;
using WebApplication1.Models.DTO;

namespace WebApplication1.Validators;

public class CreateTravelJournalDtoValidator : AbstractValidator<CreateTravelJournalDto>
{
    public CreateTravelJournalDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(255).WithMessage("Title must not exceed 255 characters");
    }
}

