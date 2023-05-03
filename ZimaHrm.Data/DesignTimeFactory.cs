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
            builder.UseNpgsql("Server=195.142.108.54;Database=zimahrm_db;User Id=paas_admin_user;Password=$<CN1dN_2X&k68oj")
                   .EnableDetailedErrors()
                   .EnableSensitiveDataLogging();

            return new AppDbContext(builder.Options);
        }
    }
}
