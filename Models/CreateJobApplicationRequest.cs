using System.ComponentModel.DataAnnotations;

namespace JobTracker.Models;

public class CreateJobApplicationRequest
{
    [Required]
    [StringLength(150)]
    public string Company { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Role { get; set; } = string.Empty;

    [Required]
    public DateOnly? AppliedDate { get; set; }
}