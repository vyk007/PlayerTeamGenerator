namespace WebApi.Requests
{
    public class PlayerSkillRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public List<PlayerSkillDto> PlayerSkills { get; set; }
    }

    public class PlayerSkillDto
    {
        public string Skill { get; set; } = string.Empty;
        public int Value { get; set; } 
    }
}
