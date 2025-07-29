// /////////////////////////////////////////////////////////////////////////////
// YOU CAN FREELY MODIFY THE CODE BELOW IN ORDER TO COMPLETE THE TASK
// /////////////////////////////////////////////////////////////////////////////

using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Requests;
using WebApi.Validators;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly DataContext Context;
        private readonly IValidator<TeamRequirementRequest> Validator;
             
        public TeamController(DataContext context, IValidator<TeamRequirementRequest> validator)
        {
            Context = context;
            Validator = validator;
        }

        /// <summary>
        /// Selects the best available players based on the provided team requirements,
        /// such as position, main skill, and number of players needed.
        /// Performs validation and ensures no duplicate role combinations.
        /// </summary>
        /// <param name="requirements">A list of team role requirements including position, main skill, and number of players</param>
        /// <returns>List of selected players matching the requirements</returns>
        [HttpPost("process")]
        public async Task<ActionResult<List<object>>> Process([FromBody] List<TeamRequirementRequest> requirements)
        {
            var duplicateResult = CheckForDuplicates(requirements);
            if (duplicateResult != null) return duplicateResult;

            var validationResult = ValidateRequirements(requirements);
            if (validationResult != null) return validationResult;

            var playersResult = await GetSelectedPlayers(requirements);
            if (playersResult.Result != null) return playersResult.Result;

            var response = FormatResponse(playersResult.Value);
            return Ok(response);
        }

        /// <summary>
        /// Checks for duplicate position and skill combinations in the team requirements.
        /// </summary>
        /// <param name="requirements">List of team requirement requests.</param>
        /// <returns>BadRequest if duplicates are found, null otherwise.</returns>
        private ActionResult? CheckForDuplicates(List<TeamRequirementRequest> requirements)
        {
            var duplicates = requirements
                .GroupBy(r => $"{r.Position.ToLower()}-{r.MainSkill.ToLower()}")
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Any())
            {
                return BadRequest(new
                {
                    Message = "Duplicate position and skill combinations are not allowed.",
                    Duplicates = duplicates
                });
            }

            return null;
        }

        /// <summary>
        /// Validates each team requirement
        /// </summary>
        /// <param name="requirements">List of team requirement requests.</param>
        /// <returns>BadRequest if duplicates are found, null otherwise.</returns>
        private ActionResult? ValidateRequirements(List<TeamRequirementRequest> requirements)
        {
            var validationFailures = new List<ValidationFailure>();
            foreach (var req in requirements)
            {
                var result = Validator.Validate(req);
                if (!result.IsValid)
                    validationFailures.AddRange(result.Errors);
            }

            if (validationFailures.Any())
            {
                return BadRequest(new
                {
                    Message = "Validation failed.",
                    Errors = validationFailures.Select(e => e.ErrorMessage).ToList()
                });
            }

            return null;
        }

        /// <summary>
        /// Selects players based on the given team requirements, applying skill matching and fallback logic.
        /// </summary>
        /// <param name="requirements">List of team requirement requests.</param>
        /// <returns>List of selected players or a BadRequest if criteria cannot be met.</returns>
        private async Task<ActionResult<List<Player>>> GetSelectedPlayers(List<TeamRequirementRequest> requirements)
        {
            var selectedPlayers = new List<Player>();
            var usedPlayerIds = new HashSet<int>();

            foreach (var requirement in requirements)
            {
                if (!int.TryParse(requirement.NumberOfPlayers, out int requiredCount))
                {
                    return BadRequest(new
                    {
                        Message = $"Invalid value for number of players: {requirement.NumberOfPlayers}"
                    });
                }

                var availablePlayers = await Context.Players
                    .Include(p => p.PlayerSkills)
                    .ToListAsync();

                availablePlayers = availablePlayers
                    .Where(p => p.Position.Equals(requirement.Position, StringComparison.OrdinalIgnoreCase)
                             && !usedPlayerIds.Contains(p.Id))
                    .ToList();

                if (availablePlayers.Count < requiredCount)
                {
                    return BadRequest(new
                    {
                        Message = $"Insufficient number of players for position: {requirement.Position}"
                    });
                }

                var withSkill = availablePlayers
                    .Where(p => p.PlayerSkills.Any(s => s.Skill.Equals(requirement.MainSkill, StringComparison.OrdinalIgnoreCase)))
                    .Select(p => new
                    {
                        Player = p,
                        SkillValue = p.PlayerSkills.First(s => s.Skill.Equals(requirement.MainSkill, StringComparison.OrdinalIgnoreCase)).Value
                    })
                    .OrderByDescending(p => p.SkillValue)
                    .Take(requiredCount)
                    .ToList();

                var selectedForRole = withSkill.Select(p => p.Player).ToList();

                if (selectedForRole.Count < requiredCount)
                {
                    var remaining = requiredCount - selectedForRole.Count;

                    var withoutSkill = availablePlayers
                        .Where(p => !selectedForRole.Any(sel => sel.Id == p.Id))
                        .Select(p => new
                        {
                            Player = p,
                            MaxSkill = p.PlayerSkills.Any() ? p.PlayerSkills.Max(s => s.Value) : 0
                        })
                        .OrderByDescending(p => p.MaxSkill)
                        .Take(remaining)
                        .Select(p => p.Player)
                        .ToList();

                    selectedForRole.AddRange(withoutSkill);
                }

                foreach (var player in selectedForRole)
                    usedPlayerIds.Add(player.Id);

                selectedPlayers.AddRange(selectedForRole);
            }

            return selectedPlayers;
        }

        /// <summary>
        /// Formats the selected player data to the required response structure.
        /// </summary>
        /// <param name="players">List of selected players.</param>
        /// <returns>Formatted list of player details.</returns>
        private List<object> FormatResponse(List<Player> players)
        {
            return players.Select(p => new
            {
                name = p.Name,
                position = p.Position,
                playerSkills = p.PlayerSkills.Select(s => new
                {
                    skill = s.Skill,
                    value = s.Value
                }).ToList()
            }).Cast<object>().ToList();
        }
    }
}

