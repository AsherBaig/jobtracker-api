using System.ComponentModel.DataAnnotations;

namespace JobTracker.Models;

public class UpdateApplicationStatusRequest
{
    [Required]
    [RegularExpression(
        "^(Applied|Interview|Offer|Rejected|Withdrawn)$",
        ErrorMessage =
            "Status must be Applied, Interview, Offer, Rejected or Withdrawn.")]
    public string Status { get; set; } = string.Empty;
}