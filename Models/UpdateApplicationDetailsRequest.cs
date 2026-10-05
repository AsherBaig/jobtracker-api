using System.ComponentModel.DataAnnotations;

namespace JobTracker.Models;

public class UpdateApplicationDetailsRequest
{
    [StringLength(2000)]
    public string? Notes { get; set; }

    public DateOnly? FollowUpDate { get; set; }
}