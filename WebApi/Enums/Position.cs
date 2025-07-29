using System.Text.Json.Serialization;

namespace WebApi.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Position
    {
        Defender = 1,
        Midfielder = 2,
        Forward = 3
    }
}
