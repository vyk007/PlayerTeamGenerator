using System.Text.Json.Serialization;

namespace WebApi.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Skill
    {
        Defense = 1,
        Attack = 2,
        Speed = 3,
        Strength = 4,
        Stamina = 5
    }
}
