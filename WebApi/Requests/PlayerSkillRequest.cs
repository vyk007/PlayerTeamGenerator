namespace WebApi.Requests
{
    public class PlayerSkillRequest
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public List<PlayerSkillDto> PlayerSkills { get; set; }
    }

    public class PlayerSkillDto
    {
        public string Skill { get; set; }
        public int Value { get; set; }
    }
}
