using FluentValidation;
using GokstadFriidrettsforeningAPI.Models;
namespace GokstadFriidrettsforeningAPI.Validation;

public class MemberValidator : AbstractValidator<Member>
{
    public MemberValidator()
    {
        RuleFor(m => m.FirstName)
            .NotEmpty()
            .Length(3, 30);
        
        RuleFor(m => m.LastName)
            .NotEmpty()
            .Length(3, 50);
        
        RuleFor(m => m.Email)
            .NotEmpty()
            .Length(5, 100);

        RuleFor(m => m.Gender)
            .NotEmpty()
            .Must(g => g is 'M' or 'F' or 'U');

        RuleFor(m => m.DateOfBirth)
            .NotEmpty();

        RuleFor(m => m.HashedPassword)
            .NotEmpty();
    }
}

public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(a => a.Street)
            .NotEmpty()
            .MaximumLength(100).WithMessage("Gatenavn kan ikke være lenger enn 100 tegn");
        
        RuleFor(a => a.PostalCode)
            .NotEmpty()
            .InclusiveBetween(1000, 9999).WithMessage("Må være mellom 1000 og 9999");

        RuleFor(a => a.City)
            .NotEmpty()
            .MaximumLength(50).WithMessage("By kan maks ha 50 tegn");
    }
}
