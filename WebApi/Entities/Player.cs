// /////////////////////////////////////////////////////////////////////////////
// PLEASE DO NOT RENAME OR REMOVE ANY OF THE CODE BELOW. 
// YOU CAN ADD YOUR CODE TO THIS FILE TO EXTEND THE FEATURES TO USE THEM IN YOUR WORK.
// YOU SHOULD NOT CHANGE THE DATABASE STRUCTURE, ADDING NEW FIELDS, RENAMING OR REMOVING THE CURRENT FIELDS MAY RESULT IN A FAILED TEST
// /////////////////////////////////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities;

public class Player
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public List<PlayerSkill> PlayerSkills { get; set; }
}