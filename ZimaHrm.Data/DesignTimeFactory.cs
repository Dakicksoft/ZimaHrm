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
            builder.UseSqlServer("Server=.\\SQLExpress,1433;Initial Catalog=hrmdb;Persist Security Info=False;User ID=zimahrm;Password={YOUR_PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;").EnableDetailedErrors().EnableSensitiveDataLogging();

            return new AppDbContext(builder.Options);
        }
    }
}
