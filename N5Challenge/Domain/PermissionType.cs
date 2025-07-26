using System.ComponentModel.DataAnnotations;

namespace N5Challenge.Domain;

public class PermissionType
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Description { get; set; } = null!;
}