using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZimaHrm.Core.DataModel;
using ZimaHrm.Core.Infrastructure.Mapping;
using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository companyRepository;

        public CompanyController(ICompanyRepository companyRepository)
        {
            this.companyRepository = companyRepository;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var settings = companyRepository.All().FirstOrDefault();
            if (settings != null)
                return View(settings.Map<CompanyModel>());
            return View(new CompanyModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CompanyModel model, IFormFile logoPostedFileBase)
        {
            if (!ModelState.IsValid) return View(model);
            if (logoPostedFileBase != null && logoPostedFileBase.Length > 0)
            {
                var path = Path.Combine(
                 Directory.GetCurrentDirectory(), "wwwroot/Images",
                 logoPostedFileBase.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    logoPostedFileBase.CopyTo(stream);
                }
                model.Logo = $"/Images/{logoPostedFileBase.FileName}";
            }
            else
            {
                var settings = companyRepository.Find(model.Id);
                if (settings != null)
                    model.Logo = settings.Logo;
            }
            companyRepository.Update(model.Map<Company>(), model.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}
