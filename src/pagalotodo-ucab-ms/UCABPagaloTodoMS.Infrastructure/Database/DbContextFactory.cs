using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UCABPagaloTodoMS.Infrastructure.Database;

public class DbContextFactory : IDbContextFactory<UCABPagaloTodoDbContext>
{
    private readonly string _dbConnectionString;

    public DbContextFactory(string dbConnectionString)
    {
        _dbConnectionString = dbConnectionString;
    }
    public UCABPagaloTodoDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<UCABPagaloTodoDbContext>();
        optionsBuilder.UseSqlServer(_dbConnectionString);
        return new UCABPagaloTodoDbContext(optionsBuilder.Options);
    }
}