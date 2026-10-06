using System.Net;
using System.Net.Http.Json;
using JobTracker.Data;
using JobTracker.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace JobTracker.Tests;

public class JobApplicationsTests
{
    [Fact]
    public async Task CreateApplication_CanBeFetchedById()
    {
        // Separate temporary database for this test.
        var databasePath = Path.Combine(
            Path.GetTempPath(),
            $"jobtracker-test-{Guid.NewGuid()}.db");

        try
        {
            using var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseContentRoot(
                        FindApiProjectDirectory());

                    builder.UseSetting(
                        "ConnectionStrings:DefaultConnection",
                        $"Data Source={databasePath};Pooling=False");
                });

            using var client = factory.CreateClient();

            // Create the test database using our migrations.
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                await db.Database.MigrateAsync();
            }

            // Act: create an application.
            var response = await client.PostAsJsonAsync(
                "/api/job-applications",
                new
                {
                    company = "Test Company",
                    role = "Backend Developer",
                    appliedDate = "2026-01-01"
                });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content
                .ReadFromJsonAsync<JobApplication>();

            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.NotNull(response.Headers.Location);

            // Fetch through the Location returned by POST.
            var getResponse = await client.GetAsync(
                response.Headers.Location);

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var saved = await getResponse.Content
                .ReadFromJsonAsync<JobApplication>();

            Assert.NotNull(saved);
            Assert.Equal(created.Id, saved.Id);
            Assert.Equal("Test Company", saved.Company);
            Assert.Equal("Backend Developer", saved.Role);
            Assert.Equal("Applied", saved.Status);
        }
        finally
        {
            File.Delete(databasePath);
            File.Delete(databasePath + "-wal");
            File.Delete(databasePath + "-shm");
        }
    }

    private static string FindApiProjectDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(
                directory.FullName, "JobTracker.csproj")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not find JobTracker.csproj.");
    }
}