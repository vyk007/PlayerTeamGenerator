using FluentValidation;
using WebApi.Enums;
using WebApi.Requests;

namespace WebApi.Validators
{
    public class PlayerSkillDtoValidator : AbstractValidator<PlayerSkillDto>
    {
        public PlayerSkillDtoValidator()
        {
            RuleFor(x => x.Skill)
            .Must(skill => Enum.GetNames<Skill>().Any(s => s.Equals(skill, StringComparison.OrdinalIgnoreCase)))
            .WithMessage(x => $"Invalid value for skill: {x.Skill}");

            RuleFor(x => x.Value)
                .InclusiveBetween(0, 100)
                .WithMessage(x =>
                    $"Invalid skill value for skill '{x.Skill}' : {x.Value}. Must be between 0 and 100.");

        }
    }
}
