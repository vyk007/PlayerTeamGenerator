using FluentValidation;
using WebApi.Enums;
using WebApi.Requests;

namespace WebApi.Validators
{
    public class CreatePlayerRequestValidator : AbstractValidator<CreatePlayerRequest>
    {
        public CreatePlayerRequestValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty()
               .WithMessage("Player name cannot be empty.");

            RuleFor(x => x.Position)
                .Must(p => Enum.GetNames<Position>().Any(n => n.Equals(p, StringComparison.OrdinalIgnoreCase)))
                .WithMessage(x => $"Invalid value for position: {x.Position}");

            RuleFor(x => x.PlayerSkills)
                .NotEmpty().WithMessage("Player must have at least one skill.");

            RuleForEach(x => x.PlayerSkills).SetValidator(new PlayerSkillDtoValidator());

            RuleFor(x => x.PlayerSkills)
             .Must(skills =>
             {
                 var duplicates = skills
                     .GroupBy(s => s.Skill.ToLowerInvariant())
                     .Where(g => g.Count() > 1)
                     .Select(g => g.Key)
                     .ToList();

                 return !duplicates.Any();
             })
             .WithMessage(x =>
             {
                 var duplicates = x.PlayerSkills
                     .GroupBy(s => s.Skill.ToLowerInvariant())
                     .Where(g => g.Count() > 1)
                     .Select(g => g.Key)
                     .ToList();

                 return $"Duplicate skills are not allowed: {string.Join(", ", duplicates)}.";
             });
        }

    }
}
