using CRMUKMTPApi.Models;
using MetaQuotes.MT5CommonAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MT5LIB.Models;

namespace CRMUKMTPApi.Data;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
        RelationalDatabaseCreator? databaseCreator = Database.GetService<IRelationalDatabaseCreator>() as RelationalDatabaseCreator;
        if (databaseCreator != null)
        {
            if (!databaseCreator.Exists())
                databaseCreator.Create();
            if (!databaseCreator.HasTables())
                databaseCreator.CreateTables();
        }
    }
    public DbSet<ManagerOrder> Orders { get; set; }
    public DbSet<ManagerDeal> Deals { get; set; }
    public DbSet<ManagerDailyReport> Daily { get; set; }
    public DbSet<ManagerUser> Users { get; set; }
    public DbSet<ManagerPosition> Positions { get; set; }
    public DbSet<ManagerSummaryReport> SummaryReports { get; set; }
   
}
