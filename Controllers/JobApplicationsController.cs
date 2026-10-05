using JobTracker.Data;
using JobTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Controllers;

[ApiController]
[Route("api/job-applications")]
public class JobApplicationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public JobApplicationsController(AppDbContext db)
    {
        _db = db;
    }

   [HttpGet]
public async Task<ActionResult<List<JobApplication>>> GetAll(
    [FromQuery] string? status,
    [FromQuery] string? company)
{
    var query = _db.JobApplications.AsNoTracking();

    if (!string.IsNullOrWhiteSpace(status))
    {
        var statusFilter = status.Trim();
        query = query.Where(x => x.Status == statusFilter);
    }

    if (!string.IsNullOrWhiteSpace(company))
    {
        var companyFilter = company.Trim();
        query = query.Where(x => x.Company.Contains(companyFilter));
    }

    var applications = await query
        .OrderByDescending(x => x.AppliedDate)
        .ThenByDescending(x => x.Id)
        .ToListAsync();

    return Ok(applications);
}

    [HttpGet("{id:int}")]
    public async Task<ActionResult<JobApplication>> GetById(int id)
    {
        var application = await _db.JobApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (application is null)
        {
            return NotFound();
        }

        return Ok(application);
    }

    [HttpPost]
    public async Task<ActionResult<JobApplication>> Create(
        CreateJobApplicationRequest request)
    {

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (request.AppliedDate!.Value > today)
        {
            ModelState.AddModelError(
                nameof(request.AppliedDate),
                "Applied date cannot be in the future.");

            return ValidationProblem(ModelState);
        }

        var application = new JobApplication
        {
            Company = request.Company.Trim(),
            Role = request.Role.Trim(),
            Status = "Applied",
            AppliedDate = request.AppliedDate!.Value
        };

        _db.JobApplications.Add(application);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = application.Id },
            application);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateApplicationStatusRequest request)
    {
        var application = await _db.JobApplications.FindAsync(id);

        if (application is null)
        {
            return NotFound();
        }

        application.Status = request.Status;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var application = await _db.JobApplications.FindAsync(id);

        if (application is null)
        {
            return NotFound();
        }

        _db.JobApplications.Remove(application);
        await _db.SaveChangesAsync();

        return NoContent();
    }

[HttpPut("{id:int}/details")]
public async Task<IActionResult> UpdateDetails(
    int id,
    UpdateApplicationDetailsRequest request)
{
    var application = await _db.JobApplications.FindAsync(id);

    if (application is null)
    {
        return NotFound();
    }

    application.Notes = request.Notes?.Trim();
    application.FollowUpDate = request.FollowUpDate;

    await _db.SaveChangesAsync();

    return NoContent();
}

[HttpGet("follow-ups")]
public async Task<ActionResult<List<JobApplication>>> GetFollowUps(
    [FromQuery] DateOnly? dueBy)
{
    var cutoff = dueBy ?? DateOnly.FromDateTime(DateTime.UtcNow);

    var applications = await _db.JobApplications
        .AsNoTracking()
        .Where(x =>
            x.FollowUpDate != null &&
            x.FollowUpDate <= cutoff &&
            (x.Status == "Applied" || x.Status == "Interview"))
        .OrderBy(x => x.FollowUpDate)
        .ThenBy(x => x.Id)
        .ToListAsync();

    return Ok(applications);
}

}