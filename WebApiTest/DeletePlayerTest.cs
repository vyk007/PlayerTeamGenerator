// /////////////////////////////////////////////////////////////////////////////
// TESTING AREA
// THIS IS AN AREA WHERE YOU CAN TEST YOUR WORK AND WRITE YOUR TESTS
// /////////////////////////////////////////////////////////////////////////////

using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace WebApiTest
{
    public class DeletePlayerTest : BaseTestWrapper
    {
        private const string ValidToken = "SkFabTZibXE1aE14ckpQUUxHc2dnQ2RzdlFRTTM2NFE2cGI4d3RQNjZmdEFITmdBQkE=";
        private const string InvalidToken = "InvalidToken123";

        public override async Task Setup()
        {
            await base.Setup();
        }

        [Test]
        public async Task TestSample()
        {

            var response = await client.DeleteAsync("/api/player/1");
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
        public async Task Should_Delete_Existing_Player_With_Valid_Token()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/player/1");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ValidToken);

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected 200 OK for valid deletion");
            Assert.That(content, Does.Contain("Player 'player name' with ID: 1 deleted successfully."), "Expected success message in response");
        }

        [Test]
        public async Task Should_Return_NotFound_For_NonExistent_Player()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/player/9999");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ValidToken);

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Expected 404 Not Found for non-existent player");
            Assert.That(content, Does.Contain("Player with ID 9999 not found."), "Expected message about player not being found");
        }

        [Test]
        public async Task Should_Return_Unauthorized_When_Token_Is_Missing()
        {
            var response = await client.DeleteAsync("/api/player/1");
            var content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), "Expected 401 Unauthorized without token");
            Assert.That(content, Does.Contain("Authorization header missing or invalid."), "Expected error message for missing token");
        }

        [Test]
        public async Task Should_Return_Unauthorized_When_Token_Is_Invalid()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/player/1");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", InvalidToken);

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), "Expected 401 Unauthorized for invalid token");
            Assert.That(content, Does.Contain("Invalid Bearer token."), "Expected error message for invalid token");
        }
    }
}
