using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Requests;

namespace WebApiTest
{
    public class UpdatePlayerTest : BaseTestWrapper
    {
        public override async Task Setup()
        {
            await base.Setup();
        }

        [Test]
        public async Task Should_Update_Existing_Player_Successfully()
        {
            try
            {
                Player request = new()
                {
                    Name = "John McEnroe",
                    Position = "midfielder",
                    PlayerSkills = new()
                    {
                        new() { Skill = "attack", Value = 40 },
                        new() { Skill = "stamina", Value = 30 }
                    }
                };

                var response = await client.PutAsJsonAsync("/api/player/1", request);
                var body = await response.Content.ReadAsStringAsync();

                Assert.That(response.IsSuccessStatusCode, Is.True, $"Expected success but got {response.StatusCode} - {body}");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to update player: {ex.Message}");
            }
        }

        [Test]
        public async Task Should_Return_NotFound_For_NonExistent_Player()
        {
            try
            {
                Player request = new()
                {
                    Name = "Clark Kent",
                    Position = "midfielder",
                    PlayerSkills = new()
                    {
                        new() { Skill = "strength", Value = 50 }
                    }
                };

                var response = await client.PutAsJsonAsync("/api/player/9999", request);
                var body = await response.Content.ReadAsStringAsync();

                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound), "Returned 404 Not Found as expected.");
                Assert.That(body, Does.Contain("Player with ID 9999 not found"), "Response body should contain the expected error message.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to test method that expects no player with the passed in player Id: {ex.Message}");
            }
        }

        [Test]
        public async Task Should_Return_BadRequest_For_Empty_Name()
        {
            Player request = new()
            {
                Name = "",
                Position = "midfielder",
                PlayerSkills = new()
                {
                    new() { Skill = "attack", Value = 40 }
                }
            };

            var response = await client.PutAsJsonAsync("/api/player/1", request);
            var json = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
            Assert.That(json, Does.Contain("Player name cannot be empty"), "Validation message for empty name is present as expected.");
        }

        [Test]
        public async Task Should_Return_BadRequest_For_Invalid_Position()
        {
            Player request = new()
            {
                Name = "Mike Doe",
                Position = "midfielder1", // Invalid position
                PlayerSkills = new()
                {
                    new() { Skill = "attack", Value = 40 }
                }
            };

            var response = await client.PutAsJsonAsync("/api/player/1", request);
            var json = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest), "Expected 400 BadRequest for invalid position.");
            Assert.That(json, Does.Contain("Invalid value for position: midfielder1"), "Validation message for invalid position is present as expected.");
        }

        [Test]
        public async Task Should_Return_BadRequest_For_Invalid_Skill()
        {
            Player request = new()
            {
                Name = "Mike Shinoda",
                Position = "defender",
                PlayerSkills = new()
                {
                    new() { Skill = "jumping", Value = 20 } // Invalid skill
                }
            };

            var response = await client.PutAsJsonAsync("/api/player/1", request);
            var json = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest), "Expected 400 BadRequest for invalid skill.");
            Assert.That(json, Does.Contain("Invalid value for skill: jumping"), "Validation message for invalid skill is present as expected.");
        }
    }
}
