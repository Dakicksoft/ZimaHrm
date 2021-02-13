using ZimaHrm.Data.Repository;
using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Repository.PaySlip;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rotativa.AspNetCore;
using Microsoft.Extensions.Hosting;
using ZimaHrm.Data.Repository.Interfaces;
using ZimaHrm.Data;
using ZimaHrm.Data.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ZimaHrm.Web.Services;

namespace ZimaHrm.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                      .AddCookie(options =>
                      {
                          options.LoginPath = "/Home/login/";
                          options.AccessDeniedPath = "/Home/Index";
                      });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var mvcBuilder = services.AddControllersWithViews().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

#if DEBUG
            mvcBuilder.AddRazorRuntimeCompilation();
#endif

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default")));
            services.AddScoped<DbContext, AppDbContext>();

            services.AddIdentity<User, Role>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.AddHttpContextAccessor();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();

            services.AddTransient<IDashboardRepository, DashboardRepository>();
            services.AddTransient<IEmployeeDashboard, EmployeeDashboard>();

            services.AddTransient<IDepartmentRepository, DepartmentRepository>();
            services.AddTransient<IDesignationRepository, DesignationRepository>();
            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            services.AddTransient<IHolidayRepository, HolidayRepository>();
            services.AddTransient<IAwardRepository, AwardRepository>();
            services.AddTransient<INoticeRepository, NoticeRepository>();
            services.AddTransient<IAttendenceRepository, AttendenceRepository>();
            services.AddTransient<ILeaveGroupRepository, LeaveGroupRepository>();
            services.AddTransient<ILeaveEmployeeRepository, LeaveEmployeeRepository>();
            services.AddTransient<ILeaveTypeRepository, LeaveTypeRepository>();
            services.AddTransient<ILeaveApplicationRepository, LeaveApplicationRepository>();

            services.AddTransient<IAllowanceTypeRepository, AllowanceTypeRepository>();
            services.AddTransient<IAllowanceRepository, AllowanceRepository>();
            services.AddTransient<IAllowanceEmployee, AllowanceEmployeeRepository>();

            services.AddTransient<IPaySlipRepository, PaySlipRepository>();
            services.AddTransient<IEmployeePaySlipRepository, EmployeePaySlipRepository>();
            services.AddTransient<IPaySlipAllowanceRepository, PaySlipAllowanceRepository>();

            ConfigureMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseRouting();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // who are you?  
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}",
                   defaults: new { controller = "Home", action = "Index" });
            });

            RotativaConfiguration.Setup(env.WebRootPath, "Rotativa");

        }

        private void ConfigureMapper()
        {
            AgileObjects.AgileMapper.Mapper.WhenMapping
              .From<Core.DataModel.DesignationModel>()
              .To<Designation>()
              .Map(dto => dto.DepartmentModel, p => p.Department);

            AgileObjects.AgileMapper.Mapper.WhenMapping
                       .From<Designation>()
                       .To<Core.DataModel.DesignationModel>()
                       .Map(dto => dto.Department, p => p.DepartmentModel);


            AgileObjects.AgileMapper.Mapper.WhenMapping
                        .From<Core.DataModel.EmployeeModel>()
                        .To<Employee>()
                        .Map(dto => dto.DepartmentModel, p => p.Department);

            AgileObjects.AgileMapper.Mapper.WhenMapping
                       .From<Employee>()
                       .To<Core.DataModel.EmployeeModel>()
                       .Map(dto => dto.Department, p => p.DepartmentModel)
                       .And
                       .Map(dto => dto.Designation, p => p.DesignationModel); ;
        }
    }
}
