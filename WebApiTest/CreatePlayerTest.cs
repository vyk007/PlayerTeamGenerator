// /////////////////////////////////////////////////////////////////////////////
// TESTING AREA
// THIS IS AN AREA WHERE YOU CAN TEST YOUR WORK AND WRITE YOUR TESTS
// /////////////////////////////////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Requests;
using WebApiTest.Base;

namespace WebApiTest
{

    public class CreatePlayerTest : BaseTestWrapper
    {
        [Test]
        public async Task TestSample()
        {
            Player player = new()
            {
                Name = "player name",
                Position = "defender",
                PlayerSkills = new()
                {
                    new() { Skill = "attack", Value = 60 },
                    new() { Skill = "speed", Value = 80 },
                }
            };

            var response = await client.PostAsJsonAsync("/api/player", player);
            try
            {
                var responseObject = await response.Content.ReadAsStringAsync();
                Assert.That(responseObject, Is.Not.Null);
            }
            catch
            {
                Assert.Fail("Invalid response object");
            }
        }

        [Test]
        public async Task CreatePlayer_ValidRequest_ReturnsOk()
        {
            Player newPlayer = new()
            {
                Name = "Test Player",
                Position = "midfielder",
                PlayerSkills = new()
            {
                new() { Skill = "attack", Value = 70 },
                new() { Skill = "stamina", Value = 85 },
            }
            };

            var response = await client.PostAsJsonAsync("/api/player", newPlayer);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected status 200 OK");
            var createdPlayer = await response.Content.ReadFromJsonAsync<Player>();
            Assert.That(createdPlayer, Is.Not.Null);
            Assert.That(createdPlayer.Name, Is.EqualTo("Test Player"));
        }

        [Test]
        public async Task CreatePlayer_EmptyName_ReturnsBadRequest()
        {
            Player request = new()
            {
                Name = "",
                Position = "forward",
                PlayerSkills = new()
            {
                new() { Skill = "speed", Value = 60 }
            }
            };

            var response = await client.PostAsJsonAsync("/api/player", request);
            var json = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(json, Does.Contain("Player name cannot be empty"), "Error message for empty name is present as expected.");
        }

        [Test]
        public async Task CreatePlayer_InvalidPosition_ReturnsBadRequest()
        {
            Player request = new()
            {
                Name = "Jane",
                Position = "goalie", // Invalid
                PlayerSkills = new()
            {
                new() { Skill = "speed", Value = 60 }
            }
            };

            var response = await client.PostAsJsonAsync("/api/player", request);
            var json = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(json, Does.Contain("Invalid value for position: goalie"), "Error message for invalid position is present as expected.");
        }

        [Test]
        public async Task CreatePlayer_MissingSkills_ReturnsBadRequest()
        {
            Player request = new()
            {
                Name = "Jane",
                Position = "midfielder",
                PlayerSkills = new() { } // No skills
            };

            var response = await client.PostAsJsonAsync("/api/player", request);
            var json = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(json, Does.Contain("Player must have at least one skill"), "Error message for missing skills is present as expected.");
        }

        [Test]
        public async Task CreatePlayer_InvalidSkillName_ReturnsBadRequest()
        {
            Player request = new()
            {
                Name = "Mark",
                Position = "forward",
                PlayerSkills = new()
            {
                new() { Skill = "jumping", Value = 50 } // Invalid skill
            }
            };

            var response = await client.PostAsJsonAsync("/api/player", request);
            var json = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(json, Does.Contain("Invalid value for skill: jumping"), "Error message for invalid skill is present as expected.");
        }

        [Test]
        public async Task CreatePlayer_DuplicateSkills_ReturnsBadRequest()
        {
            Player request = new()
            {
                Name = "Paul",
                Position = "defender",
                PlayerSkills = new()
            {
                new() { Skill = "attack", Value = 40 },
                new() { Skill = "attack", Value = 30 } // Duplicate skill
            }
            };

            var response = await client.PostAsJsonAsync("/api/player", request);
            var json = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(json, Does.Contain("Duplicate skills are not allowed: attack"), "Error message for duplicate skills is present as expected.");
        }

        [Test]
        public async Task CreatePlayer_DuplicatePlayer_ReturnsBadRequest()
        {
            Player duplicate = new()
            {
                Name = "player name",
                Position = "defender",
                PlayerSkills = new()
            {
                new() { Skill = "speed", Value = 70 }
            }
            };

            var response = await client.PostAsJsonAsync("/api/player", duplicate);
            var content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(content.ToLower(), Does.Contain("already exists"));
        }

        [Test]
        public async Task CreatePlayer_InvalidSkillValue_ReturnsBadRequest()
        {
            Player invalid = new()
            {
                Name = "Invalid Skill Player",
                Position = "forward",
                PlayerSkills = new()
            {
                new() { Skill = "attack", Value = 150 } // Out of range
            }
            };

            var response = await client.PostAsJsonAsync("/api/player", invalid);
            var content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(content.ToLower(), Does.Contain("must be between 0 and 100"), "Error message for out-of-range skill value is present.");
        }

    }
}
