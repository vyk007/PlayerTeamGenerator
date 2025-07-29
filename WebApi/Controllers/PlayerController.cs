// /////////////////////////////////////////////////////////////////////////////
// YOU CAN FREELY MODIFY THE CODE BELOW IN ORDER TO COMPLETE THE TASK
// /////////////////////////////////////////////////////////////////////////////

namespace WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Helpers;
using WebApi.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Enums;
using FluentValidation;
using WebApi.Requests;
using WebApi.Filters;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
  private readonly DataContext Context;
    private readonly IValidator<CreatePlayerRequest> Validator;

    public PlayerController(DataContext context, IValidator<CreatePlayerRequest> validator)
    {
        Context = context;
        Validator = validator;
    }

    /// <summary>
    /// Retrieves all players.
    /// </summary>
    /// <returns>A list of all players.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Player>>> GetAll()
    {
        try
        {
            var players = await Context.Players
                .Include(p => p.PlayerSkills)
                .ToListAsync();

            return Ok(players);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error retrieving players: {ex.Message}" });
        }
    }

    /// <summary>
    /// Creates a new player with the specified details.
    /// </summary>
    /// <param name="request">The player creation request.</param>
    /// <returns>The created player or a validation error.</returns>
    [HttpPost]
    public async Task<ActionResult<Player>> PostPlayer([FromBody] CreatePlayerRequest request)
    {
        //await Task.Run(() => Context.Players.FirstOrDefault(x => x.Id == 2));
        //throw new NotImplementedException();
        try
        {
            // Validate request
            var validationResult = await Validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var error = validationResult.Errors.First();
                return BadRequest(new { Message = error.ErrorMessage });
            }

            // Check if player with same name and position already exists.
            var nameLower = request.Name.Trim().ToLowerInvariant();
            var positionLower = request.Position.Trim().ToLowerInvariant();

            var exists = await Context.Players
                .AnyAsync(p => p.Name.ToLower() == nameLower && p.Position.ToLower() == positionLower);

            if (exists)
            {
                return BadRequest(new { Message = $"A player named '{request.Name}' with position '{request.Position}' already exists." });
            }

            // Convert request to domain model
            var player = new Player
            {
                Name = request.Name,
                Position = Enum.Parse<Position>(request.Position, ignoreCase: true).ToString(),
                PlayerSkills = request.PlayerSkills.Select(skill => new PlayerSkill
                {
                    Skill = Enum.Parse<Skill>(skill.Skill, ignoreCase: true).ToString(),
                    Value = skill.Value
                }).ToList()
            };

            Context.Players.Add(player);
            await Context.SaveChangesAsync();

            return Ok(player);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = $"Something went wrong while creating the player: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Updates an existing player with the specified ID.
    /// </summary>
    /// <param name="playerId">The ID of the player to update.</param>
    /// <param name="request">The updated player details.</param>
    /// <returns>The updated player or a validation error.</returns>
    [HttpPut("{playerId}")]
    public async Task<IActionResult> PutPlayer(int playerId, [FromBody] CreatePlayerRequest request)
    {
        try
        {
            var validationResult = await Validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var error = validationResult.Errors.First();
                return BadRequest(new { Message = error.ErrorMessage });
            }

            var player = await Context.Players
                .Include(p => p.PlayerSkills)
                .FirstOrDefaultAsync(p => p.Id == playerId);

            if (player == null)
                return NotFound(new { Message = $"Player with ID {playerId} not found." });

            // Check for duplicate (excluding the current record)
            var nameLower = request.Name.Trim().ToLowerInvariant();
            var positionLower = request.Position.Trim().ToLowerInvariant();
            var duplicateExists = await Context.Players
                .AnyAsync(p => p.Id != playerId &&
                               p.Name.ToLower() == nameLower &&
                               p.Position.ToLower() == positionLower);

            if (duplicateExists)
            {
                return BadRequest(new { Message = $"Another player with name '{request.Name}' and position '{request.Position}' already exists." });
            }

            // Update fields
            player.Name = request.Name;
            player.Position = Enum.Parse<Position>(request.Position, true).ToString();

            // Remove old skills
            Context.PlayerSkills.RemoveRange(player.PlayerSkills);

            // Add new skills
            player.PlayerSkills = request.PlayerSkills.Select(skill => new PlayerSkill
            {
                Skill = Enum.Parse<Skill>(skill.Skill, true).ToString(),
                Value = skill.Value
            }).ToList();

            await Context.SaveChangesAsync();

            return Ok(player);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error updating player: {ex.Message}" });
        }
    }

    /// <summary>
    /// Deletes the player with the specified ID.
    /// </summary>
    /// <param name="playerId">The ID of the player to delete.</param>
    /// <returns>The deleted player or a NotFound result if the player does not exist.</returns>
    [HttpDelete("{playerId}")]
    [BearerTokenRequired]
    public async Task<ActionResult<Player>> DeletePlayer(int playerId)
    {
        try
        {
            var player = await Context.Players
                .Include(p => p.PlayerSkills)
                .FirstOrDefaultAsync(p => p.Id == playerId);

            if (player == null)
                return NotFound(new { Message = $"Player with ID {playerId} not found." });

            Context.PlayerSkills.RemoveRange(player.PlayerSkills);
            Context.Players.Remove(player);
            await Context.SaveChangesAsync();

            return Ok(new { Message = $"Player '{player.Name}' with ID: {player.Id} deleted successfully.", Player = player });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error deleting player: {ex.Message}" });
        }
    }
}