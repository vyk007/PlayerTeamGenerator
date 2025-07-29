namespace WebApi.Requests
{
    public class CreatePlayerRequest
    {
        public string Name { get; set; }
        public string Position { get; set; } = string.Empty;
        public List<PlayerSkillDto> PlayerSkills { get; set; }
    }
}
