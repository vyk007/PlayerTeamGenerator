// /////////////////////////////////////////////////////////////////////////////
// TESTING AREA
// THIS IS AN AREA WHERE YOU CAN TEST YOUR WORK AND WRITE YOUR TESTS
// /////////////////////////////////////////////////////////////////////////////

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;
using WebApiTest.Base;

namespace WebApiTest
{

    public abstract class BaseTestWrapper
    {
        protected HttpClient client;
        protected DataContext dataContext;
        [SetUp]
        public virtual async Task Setup()
        {
            // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
            // at the end of the test (see Dispose below).
            var _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            // These options will be used by the context instances in this test suite, including the connection opened above.
            var contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite(_connection)
                .Options;

            dataContext = new DataContext(contextOptions);
            var app = new TestApplication(_connection);

            client = app.CreateClient();

            await dataContext.Database.EnsureDeletedAsync();
            await dataContext.Database.EnsureCreatedAsync();

            // Seed players
            await SeedTestPlayers();
        }

        [TearDown]
        public virtual void TearDown()
        {
            dataContext.Database.EnsureDeleted();
            client.Dispose();
        }

        protected async Task CreatePlayer(PlayerViewModel newPlayer = null)
        {
            newPlayer ??= playerOne;
            Player model = new()
            {
                Name = newPlayer.Name,
                Position = newPlayer.Position,
                PlayerSkills = newPlayer.PlayerSkills.Select(x => new PlayerSkill
                {
                    Skill = x.Skill,
                    Value = x.Value
                }).ToList()
            };

            dataContext.Players.Add(model);
            await dataContext.SaveChangesAsync();
        }

        private async Task SeedTestPlayers()
        {
            var players = new List<Player>
            {
                new Player
                {
                    Name = playerOne.Name,
                    Position = playerOne.Position,
                    PlayerSkills = playerOne.PlayerSkills.Select(x => new PlayerSkill
                    {
                        Skill = x.Skill,
                        Value = x.Value
                    }).ToList()
                },
                new Player
                {
                    Name = playerTwo.Name,
                    Position = playerTwo.Position,
                    PlayerSkills = playerTwo.PlayerSkills.Select(x => new PlayerSkill
                    {
                        Skill = x.Skill,
                        Value = x.Value
                    }).ToList()
                },
                new Player
                {
                    Name = playerThree.Name,
                    Position = playerThree.Position,
                    PlayerSkills = playerThree.PlayerSkills.Select(x => new PlayerSkill
                    {
                        Skill = x.Skill,
                        Value = x.Value
                    }).ToList()
                },
                new Player
                {
                    Name = playerFour.Name,
                    Position = playerFour.Position,
                    PlayerSkills = playerFour.PlayerSkills.Select(x => new PlayerSkill
                    {
                        Skill = x.Skill,
                        Value = x.Value
                    }).ToList()
                }
            };

            await dataContext.Players.AddRangeAsync(players);
            await dataContext.SaveChangesAsync();
        }

        protected PlayerViewModel playerOne = new()
        {
            Name = "player name",
            Position = "defender",
            PlayerSkills = new()
            {
                new() { Skill = "attack", Value = 60 },
                new() { Skill = "speed", Value = 80 },
            }
        };

        protected PlayerViewModel playerTwo = new()
        {
            Name = "John Doe",
            Position = "forward",
            PlayerSkills = new()
            {
                new() { Skill = "attack", Value = 90 },
                new() { Skill = "stamina", Value = 75 },
            }
        };

        protected PlayerViewModel playerThree = new()
        {
            Name = "Jane Smith",
            Position = "midfielder",
            PlayerSkills = new()
            {
                new() { Skill = "pass", Value = 85 },
                new() { Skill = "speed", Value = 70 },
            }
        };

        protected PlayerViewModel playerFour = new()
        {
            Name = "Alex Lee",
            Position = "midfielder",
            PlayerSkills = new()
            {
                new() { Skill = "defense", Value = 95 },
                new() { Skill = "speed", Value = 60 },
            }
        };
    }
}
