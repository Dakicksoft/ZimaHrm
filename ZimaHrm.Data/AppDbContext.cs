using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using ZimaHrm.Data.Entity;

namespace ZimaHrm.Data
{
    public class AppDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<Award> Awards { get; set; }
        public DbSet<Notice> Notices { get; set; }
        public DbSet<Attendence> Attendences { get; set; }
        public DbSet<LeaveGroup> LeaveGroup { get; set; }
        public DbSet<LeaveEmployee> LeaveEmployee { get; set; }
        public DbSet<LeaveType> LeaveType { get; set; }
        public DbSet<LeaveApplication> LeaveApplication { get; set; }
        public DbSet<AllowanceType> AllowanceType { get; set; }
        public DbSet<Allowance> Allowance { get; set; }
        public DbSet<AllowanceEmployee> AllowanceEmployee { get; set; }
        public DbSet<PaySlip> PaySlip { get; set; }
        public DbSet<EmployeePaySlip> EmployeePaySlip { get; set; }
        public DbSet<PaySlipAllowance> PaySlipAllowance { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasOne(x => x.Department)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Company>()
                        .HasData(SeedData.BuildApplicationCompanies());

            RegisterIdentity(modelBuilder);

        }

        private static void RegisterIdentity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                        .ToTable("Role", schema: "Identity")
                        .HasData(SeedData.BuildApplicationRoles());

            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("User", schema: "Identity");
                b.HasIndex(s => new { s.CompanyId });
                b.Property(s => s.Status).HasConversion(v => v.ToString(), v => (UserStatus)Enum.Parse(typeof(UserStatus), v));
                b.HasData(SeedData.BuildApplicationUsers());
                b.Property(s => s.CompanyId).HasDefaultValue<Guid>(Guid.Empty);
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.ToTable("UserRole", schema: "Identity");

                b.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasData(SeedData.BuildApplicationUserRoles());
            });

            modelBuilder.Entity<UserClaim>().ToTable("UserClaim", schema: "Identity");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogin", schema: "Identity");
            modelBuilder.Entity<RoleClaim>().ToTable("RoleClaim", schema: "Identity");
            modelBuilder.Entity<UserToken>().ToTable("UserToken", schema: "Identity");
            modelBuilder.Entity<UserAudit>().ToTable("UserAudit", schema: "Identity");
        }

    }
}
