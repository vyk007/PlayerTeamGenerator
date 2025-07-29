// /////////////////////////////////////////////////////////////////////////////
// TESTING AREA
// THIS IS AN AREA WHERE YOU CAN TEST YOUR WORK AND WRITE YOUR TESTS
// /////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Net.Http.Json;
using System.Net;
using System.Threading.Tasks;
using WebApi.Entities;

namespace WebApiTest
{
    public class ListPlayerTest : BaseTestWrapper
    {
        public override async Task Setup()
        {
            await base.Setup();
        }

        [Test]
        public async Task TestSample()
        {

            var response = await client.GetAsync("/api/player");
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
        public async Task GetPlayers_ReturnsAllPlayersSuccessfully()
        {
            try
            {
                var response = await client.GetAsync("/api/player");

                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected 200 OK response");

                var players = await response.Content.ReadFromJsonAsync<List<Player>>();

                Assert.That(players, Is.Not.Null, "Player list should not be null");
                Assert.That(players.Count, Is.GreaterThan(0), "At least one player should exist in the response");

                foreach (var player in players)
                {
                    Assert.That(player.Name, Is.Not.Null.And.Not.Empty, "Player Name should not be null or empty");
                    Assert.That(player.Position, Is.Not.Null.And.Not.Empty, "Player Position should not be null or empty");
                    Assert.That(player.PlayerSkills, Is.Not.Null, "PlayerSkills should not be null");

                    foreach (var skill in player.PlayerSkills)
                    {
                        Assert.That(skill.Skill, Is.Not.Null.And.Not.Empty, "Skill name should not be null or empty");
                        Assert.That(skill.Value, Is.InRange(0, 100), "Skill value should be between 0 and 100");
                    }
                }
            }
            catch
            {
                Assert.Fail("Invalid response object");
            }
        }

        [Test]
        public async Task GetPlayers_NoPlayersExist_ReturnsEmptyList()
        {
            try
            {
                // Remove all seeded players
                dataContext.Players.RemoveRange(dataContext.Players);
                await dataContext.SaveChangesAsync();

                var response = await client.GetAsync("/api/player");

                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected 200 OK when no players exist");

                var players = await response.Content.ReadFromJsonAsync<List<Player>>();

                Assert.That(players, Is.Not.Null, "Player list should not be null");
                Assert.That(players.Count, Is.EqualTo(0), "Expected zero players in the response");
            }
            catch
            {
                Assert.Fail("Exception occurred while testing with empty database");
            }
        }


    }
}
