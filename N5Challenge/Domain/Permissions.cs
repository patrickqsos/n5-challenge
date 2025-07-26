using System.ComponentModel.DataAnnotations;

namespace N5Challenge.Domain;

public class Permissions
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string EmployeeName { get; set; } = null!;

    /// <summary>
    /// Foreign‑key towards <see cref="PermissionTypes"/>.
    /// </summary>
    public int PermissionTypeId { get; set; }
    public PermissionTypes PermissionTypes { get; set; } = null!;

    /// <summary>
    /// When the permission takes / took place (UTC).
    /// Defaults to <see cref="DateTime.UtcNow"/> at insert time.
    /// </summary>
    public DateTime PermissionDate { get; set; } = DateTime.UtcNow;
}