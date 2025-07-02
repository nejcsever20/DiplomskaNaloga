using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UserProfile
{
    [Key] public int Id { get; set; } //primary key for the table

    [Required] public string? UserId { get; set; }

    [Required] public string? AvatarPath { get; set; }
}