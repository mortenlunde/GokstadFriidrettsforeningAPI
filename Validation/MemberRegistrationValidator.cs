using FluentValidation;
using GokstadFriidrettsforeningAPI.ModelResponses;
namespace GokstadFriidrettsforeningAPI.Validation;

public class MemberRegistrationValidator : AbstractValidator<MemberRegistration>
{
    public MemberRegistrationValidator()
    {
        RuleFor(m => m.FirstName)
            .NotEmpty()
            .Length(3, 30).WithMessage("Fornavn må være mellom 3 og 30 tegn");
        
        RuleFor(m => m.LastName)
            .NotEmpty()
            .Length(3, 50).WithMessage("Etternavn må være mellom 3 og 50 tegn");

        RuleFor(m => m.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Skriv en gyldig epost");

        RuleFor(m => m.Gender)
            .NotEmpty()
            .Must(g => g is 'M' or 'F' or 'U').WithMessage("Vennligst oppi kjønn");

        RuleFor(m => m.DateOfBirth)
            .NotEmpty().WithMessage("Velg en gyldig fødselsdag");

        RuleFor(m => m.Password)
            .NotEmpty().WithMessage("Passord kan ikke være blankt")
            .Length(6, 20).WithMessage("Passord må være mellom 6 og 20 tegn")
            .Matches("[A-Z]+").WithMessage("Passord må inneholde minst én stor bokstav")
            .Matches("[a-z]+").WithMessage("Passord må inneholde minst én liten bokstav")
            .Matches("[0-9]+").WithMessage("Passord må inneholde minst ett tall")
            .Matches("[^a-zA-Z0-9]+").WithMessage("Passord må inneholde minst ett spesialtegn")
            .Must(p => !p.Any(x => "æøåÆØÅ".Contains(x))).WithMessage("Du har tastet inn ugyldige tegn");
    }
}