using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UCABPagaloTodoMS.Infrastructure.Database;

public class DbContextFactory : IDesignTimeDbContextFactory<UCABPagaloTodoDbContext>
{
    private readonly string _dbConnectionString;

    public DbContextFactory(string dbConnectionString)
    {
        _dbConnectionString = dbConnectionString;
    }

    public UCABPagaloTodoDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UCABPagaloTodoDbContext>();
        optionsBuilder.UseSqlServer(_dbConnectionString);
        return new UCABPagaloTodoDbContext(optionsBuilder.Options);
    }
}