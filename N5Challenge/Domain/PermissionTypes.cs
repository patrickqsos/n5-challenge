using System.ComponentModel.DataAnnotations;

namespace N5Challenge.Domain;

public class PermissionTypes
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Description { get; set; } = null!;
}