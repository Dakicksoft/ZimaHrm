using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ZimaHrm.Data
{
    public sealed class DesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
         
            var builder = new DbContextOptionsBuilder<AppDbContext>();

            //Change here
            builder.UseSqlServer("Server=localhost,1433;Initial Catalog=hrmdb;Persist Security Info=False;User ID=sa;Password=myPass123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;")
                   .EnableDetailedErrors()
                   .EnableSensitiveDataLogging();

            return new AppDbContext(builder.Options);
        }
    }
}
