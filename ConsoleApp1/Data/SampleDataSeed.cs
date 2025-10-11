namespace ConsoleApp1.Data;

using ConsoleApp1.DependencyInjection.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[Service]
public class SampleDataSeed
{
    private readonly SampleDbContext _context;
    private readonly ILogger<SampleDataSeed> _logger;

    public SampleDataSeed(SampleDbContext context, ILogger<SampleDataSeed> logger)
    {
        _context = context;
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();
        _logger = logger;
        logger.LogInformation("SampleDataSeed initialized.");
    }
    [Trace]
    public virtual void Seed()
    {
        _logger.LogInformation("Seeding sample data...");
        // Check if the database is already seeded
        if (_context.SampleData.Any())
        {
            return; // Database has been seeded
        }

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            // Seed the database with sample data
            var sampleEntities = new List<SampleRecord>
                {
                    new SampleRecord { Name = "Sample 1", CreatedAt = DateTime.UtcNow },
                    new SampleRecord { Name = "Sample 2", CreatedAt = DateTime.UtcNow },
                    new SampleRecord { Name = "Sample 3", CreatedAt = DateTime.UtcNow }
                };
            _context.SampleData.AddRange(sampleEntities);
            _context.SaveChanges();
            transaction.Commit();
            _logger.LogInformation("Sample data seeded successfully.");
            
        }
        catch (System.Exception)
        {
            transaction.Rollback();
            _logger.LogError("An error occurred while seeding sample data.");
            throw;
        }
    }
}