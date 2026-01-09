using FluentValidation;
using WebApplication1.Models.DTO;

namespace WebApplication1.Validators;

public class CreateTripDtoValidator : AbstractValidator<CreateTripDto>
{
    public CreateTripDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(255).WithMessage("Title must not exceed 255 characters");

        RuleFor(x => x.TravelJournalId)
            .GreaterThan(0).WithMessage("TravelJournalId must be greater than 0");

        RuleFor(x => x.LocationId)
            .GreaterThan(0).WithMessage("LocationId must be greater than 0");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("StartDate is required");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("EndDate must be greater than or equal to StartDate");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .When(x => x.Rating.HasValue)
            .WithMessage("Rating must be between 1 and 5");
    }
}

