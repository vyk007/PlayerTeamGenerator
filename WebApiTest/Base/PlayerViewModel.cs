// /////////////////////////////////////////////////////////////////////////////
// TESTING AREA
// THIS IS AN AREA WHERE YOU CAN TEST YOUR WORK AND WRITE YOUR TESTS
// /////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApiTest.Base
{
    public class PlayerViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Position { get; set; } = string.Empty;
        public List<PlayerSkillViewModel> PlayerSkills { get; set; }
    }

    public class PlayerSkillViewModel
    {
        public string Skill { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
