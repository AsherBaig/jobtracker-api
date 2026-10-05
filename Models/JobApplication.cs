namespace JobTracker.Models;

public class JobApplication
{
    public int Id { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = "Applied";
    public DateOnly AppliedDate { get; set; }
    public string? Notes { get; set; }
    public DateOnly? FollowUpDate { get; set; }
}