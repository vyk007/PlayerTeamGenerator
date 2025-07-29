using FluentValidation;
using WebApi.Enums;
using WebApi.Requests;

namespace WebApi.Validators
{
    public class TeamRequirementRequestValidator : AbstractValidator<TeamRequirementRequest>
    {
        public TeamRequirementRequestValidator()
        {
            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("Position is required.")
                .Must(p => Enum.GetNames<Position>().Any(n => n.Equals(p, StringComparison.OrdinalIgnoreCase)))
                .WithMessage(x => $"Invalid value for position: {x.Position}");

            RuleFor(x => x.MainSkill)
                .NotEmpty().WithMessage("Main skill is required.")
                .Must(s => Enum.GetNames<Skill>().Any(n => n.Equals(s, StringComparison.OrdinalIgnoreCase)))
                .WithMessage(x => $"Invalid value for skill: {x.MainSkill}");

            RuleFor(x => x.NumberOfPlayers)
                .NotEmpty().WithMessage("Number of players is required.")
                .Must(val => int.TryParse(val, out int result) && result > 0)
                .WithMessage("Number of players must be a valid positive number greater than 0.");
        }
    }
}
