using System.ComponentModel.DataAnnotations;

namespace N5Challenge.Domain;

public class Permission
{
    /// <summary>
    /// Represents the unique identifier for a permission.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Represents the forename of the employee associated with the permission.
    /// </summary>
    [Required, MaxLength(150)]
    public string EmployeeForename { get; set; } = null!;

    /// <summary>
    /// Represents the surname of the employee associated with the permission.
    /// </summary>
    [Required, MaxLength(150)]
    public string EmployeeSurname { get; set; } = null!;

    /// <summary>
    /// Foreign‑key towards <see cref="PermissionType"/>.
    /// </summary>
    public int PermissionTypeId { get; set; }
    public PermissionType PermissionType { get; set; } = null!;

    /// <summary>
    /// When the permission takes / took place (UTC).
    /// Defaults to <see cref="DateTime.UtcNow"/> at insert time.
    /// </summary>
    public DateTime PermissionDate { get; set; } = DateTime.UtcNow;
}