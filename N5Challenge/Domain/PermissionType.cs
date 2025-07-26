using System.ComponentModel.DataAnnotations;

namespace N5Challenge.Domain;

public class PermissionType
{
    /// <summary>
    /// Represents the unique identifier for a permission type.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Represents the description of the permission type.
    /// </summary>
    [Required, MaxLength(100)]
    public string Description { get; set; } = null!;
}