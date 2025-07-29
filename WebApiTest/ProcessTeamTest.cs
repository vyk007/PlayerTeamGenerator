// /////////////////////////////////////////////////////////////////////////////
// TESTING AREA
// THIS IS AN AREA WHERE YOU CAN TEST YOUR WORK AND WRITE YOUR TESTS
// /////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebApiTest.Base;

namespace WebApiTest
{
    public class ProcessTeamTest : BaseTestWrapper
    {

        [Test]
        public async Task TestSample()
        {
            List<TeamProcessViewModel> requestData = new List<TeamProcessViewModel>()
            {
                new TeamProcessViewModel()
                {
                    Position = "defender",
                    MainSkill = "speed",
                    NumberOfPlayers = "1"
                }
            };

            var response = await client.PostAsJsonAsync("/api/team/process", requestData);
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
        public async Task ValidRequest_ShouldReturnSelectedPlayers()
        {
            var requestData = new List<TeamProcessViewModel>
            {
                new TeamProcessViewModel
                {
                    Position = "midfielder",
                    MainSkill = "speed",
                    NumberOfPlayers = "2"
                }
            };

            var response = await client.PostAsJsonAsync("/api/team/process", requestData);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var responseObject = await response.Content.ReadAsStringAsync();
            Assert.That(responseObject, Is.Not.Null.And.Contains("playerSkills"));
        }

        [Test]
        public async Task DuplicatePositionAndSkill_ShouldReturnBadRequest()
        {
            var requestData = new List<TeamProcessViewModel>
            {
                new TeamProcessViewModel
                {
                    Position = "defender",
                    MainSkill = "speed",
                    NumberOfPlayers = "1"
                },
                new TeamProcessViewModel
                {
                    Position = "defender",
                    MainSkill = "speed",
                    NumberOfPlayers = "1"
                }
            };

            var response = await client.PostAsJsonAsync("/api/team/process", requestData);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var responseObject = await response.Content.ReadAsStringAsync();
            Assert.That(responseObject, Does.Contain("Duplicate position and skill combinations"));
        }

        [Test]
        public async Task InvalidNumberOfPlayers_ShouldReturnBadRequest()
        {
            var requestData = new List<TeamProcessViewModel>
            {
                new TeamProcessViewModel
                {
                    Position = "defender",
                    MainSkill = "defense",
                    NumberOfPlayers = "two" // Invalid number
                }
            };

            var response = await client.PostAsJsonAsync("/api/team/process", requestData);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var responseObject = await response.Content.ReadAsStringAsync();
            Assert.That(responseObject, Does.Contain("Number of players must be a valid positive number greater than 0."));
        }

        [Test]
        public async Task NotEnoughPlayers_ShouldReturnBadRequest()
        {
            var requestData = new List<TeamProcessViewModel>
            {
                new TeamProcessViewModel
                {
                    Position = "forward",
                    MainSkill = "attack",
                    NumberOfPlayers = "10" 
                }
            };

            var response = await client.PostAsJsonAsync("/api/team/process", requestData);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var responseObject = await response.Content.ReadAsStringAsync();
            Assert.That(responseObject, Does.Contain("Insufficient number of players"));
        }




        [Test]
        public async Task EmptyPosition_ShouldReturnValidationError()
        {
            var requestData = new List<TeamProcessViewModel>
            {
                new TeamProcessViewModel
                {
                    Position = "",
                    MainSkill = "speed",
                    NumberOfPlayers = "1"
                }
            };

            var response = await client.PostAsJsonAsync("/api/team/process", requestData);
            var responseText = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseText, Does.Contain("Position is required"));
            Assert.That(responseText, Does.Contain("Invalid value for position"));
        }

        [Test]
        public async Task EmptyMainSkill_ShouldReturnValidationError()
        {
            var requestData = new List<TeamProcessViewModel>
            {
                new TeamProcessViewModel
                {
                    Position = "defender",
                    MainSkill = "",
                    NumberOfPlayers = "1"
                }
            };

            var response = await client.PostAsJsonAsync("/api/team/process", requestData);
            var responseText = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseText, Does.Contain("Main skill is required"));
            Assert.That(responseText, Does.Contain("Invalid value for skill"));
        }

        [Test]
        public async Task EmptyNumberOfPlayers_ShouldReturnValidationError()
        {
            var requestData = new List<TeamProcessViewModel>
            {
                new TeamProcessViewModel
                {
                    Position = "defender",
                    MainSkill = "speed",
                    NumberOfPlayers = ""
                }
            };

            var response = await client.PostAsJsonAsync("/api/team/process", requestData);
            var responseText = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseText, Does.Contain("Number of players is required"));
            Assert.That(responseText, Does.Contain("Number of players must be a valid positive number greater than 0"));
        }

        [Test]
        public async Task ZeroNumberOfPlayers_ShouldReturnValidationError()
        {
            var requestData = new List<TeamProcessViewModel>
            {
                new TeamProcessViewModel
                {
                    Position = "defender",
                    MainSkill = "speed",
                    NumberOfPlayers = "0"
                }
            };

            var response = await client.PostAsJsonAsync("/api/team/process", requestData);
            var responseText = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseText, Does.Contain("Number of players must be a valid positive number greater than 0"));
        }
       
    }

}

