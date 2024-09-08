using HRProject_NTier.CORE.Entities;
using HRProject_NTier.DATAACCESS.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRProject_NTier.WEBUI.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class HomeController : Controller
    {
        public readonly IVisitorCommentsRepository _visitorCommentsRepository;
        public readonly IPackageRepository _packageRepository;
        public readonly ICompanyRepository _companyRepository;
        public readonly IAdminRepository _adminRepository;

        public HomeController(IVisitorCommentsRepository visitorCommentsRepository, IPackageRepository packageRepository, ICompanyRepository companyRepository, IAdminRepository adminRepository)
        {
            _visitorCommentsRepository = visitorCommentsRepository;
            _packageRepository = packageRepository;
            _companyRepository = companyRepository;
            _adminRepository = adminRepository;
        }
        public IActionResult Index()
        {
           
            ViewData[("packageTimethreeMont")] = _companyRepository.Companies.Where(x => x.Package.EndDate.Year == DateTime.Now.Year && x.Package.EndDate.Month - DateTime.Now.Month == 1).Include(x=>x.Package).ToList();

            ViewData[("packageTimesixMont")] = _companyRepository.Companies.Where(x=> x.Package.EndDate.Year == DateTime.Now.Year && x.Package.EndDate.Month - DateTime.Now.Month == 2).Include(x=>x.Package).ToList();

            ViewData[("packageTimeoneYear")] = _companyRepository.Companies.Where(x => x.Package.EndDate.Year == DateTime.Now.Year && x.Package.EndDate.Month - DateTime.Now.Month == 3).Include(x=>x.Package).ToList();
            return View();
        }
       
        public IActionResult ListVisitorComments()
        {
                var visitormessage = _visitorCommentsRepository.VisitorComments.ToList();
            
                return View(visitormessage);
        }
        [HttpGet]
        public IActionResult DeleteVisitorMessage(int id)
        {
            _visitorCommentsRepository.DeleteVisitorComments(id);
            return RedirectToAction("ListVisitorComments");

        }
       public IActionResult AdminInfo(Admin admin,int id)
        {
            var result = _adminRepository.GetByID(id);
            
            return View(result);
        }


    }
}
