namespace WebApi.Requests
{
    public class CreatePlayerRequest
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public List<PlayerSkillDto> PlayerSkills { get; set; }
    }
}
