using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZimaHrm.Infrastructure.Extensions;

namespace ZimaHrm.Web.Controllers
{
    [Route("")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DbMigrationController : Controller
    {
        private readonly IServiceProvider _svcProvider;

        public DbMigrationController(IServiceProvider svcProvider)
        {
            _svcProvider = svcProvider;
        }

        [HttpGet("/db-migration")]
        public Task<bool> Index()
        {
            return Task.Run(() => _svcProvider.MigrateDbContext() != null);
        }
    }
}
