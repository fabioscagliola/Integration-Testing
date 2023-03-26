using FluentValidation;

namespace com.fabioscagliola.IntegrationTesting.WebApi;

public class PersonValidation : AbstractValidator<Person>
{
    public PersonValidation()
    {
        RuleFor(person => person.FName)
            .NotEmpty()
            .WithMessage(Properties.Resources.PersonValidationFNameIsEmpty);

        RuleFor(person => person.LName)
            .NotEmpty()
            .WithMessage(Properties.Resources.PersonValidationLNameIsEmpty);
    }
}
