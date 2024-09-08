using HRProject_NTier.CORE.Entities;
using HRProject_NTier.CORE.ViewModels;
using HRProject_NTier.DATAACCESS.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRProject_NTier.WEBUI.Areas.ManagerArea.Controllers
{

    [Area("ManagerArea")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPackageRepository _packageRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IPersonnelRepository _personnelRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IManagerRepository _managerRepository;
        private readonly IPaymentRepository _paymentRepository;

        public HomeController(ILogger<HomeController> logger, IPackageRepository packageRepository, IPersonnelRepository personnelRepository, IPermissionRepository permissionRepository, ICompanyRepository companyRepository, IManagerRepository managerRepository, IPaymentRepository paymentRepository)
        {
            _logger = logger;
            _packageRepository = packageRepository;
            _permissionRepository = permissionRepository;
            _personnelRepository = personnelRepository;
            _companyRepository = companyRepository;
            _managerRepository = managerRepository;
            _paymentRepository = paymentRepository;
        }

        //yorum
        //PaketList
        
        public IActionResult Index(int id)
        {
            id = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
            ViewData["paketler"] = _managerRepository.Managers.Where(x => x.ID == id).Include(x =>x.Company.Package);

            return View(Tuple.Create< IEnumerable<Permission>, IEnumerable<Personnel>>
                (
                           _permissionRepository.Permissions.Where(x => x.StartedDate.Month == DateTime.Now.Month && x.StartedDate.Day - DateTime.Now.Day <= 3 && x.StartedDate.Day - DateTime.Now.Day > 0).Include(x => x.Personnel).ToList()
                           , _personnelRepository.Personnels.Where(x => x.BirthDate.Month == DateTime.Now.Month && x.BirthDate.Day - DateTime.Now.Day <= 3 && x.BirthDate.Day - DateTime.Now.Day >0).ToList()
                 ));
        }
        [HttpPost]
        public IActionResult Index([Bind(Prefix = "item1")] IEnumerable<Permission> item1, [Bind(Prefix = "item2")] IEnumerable<Personnel> item2)
        {
            return View();
        }

        public IActionResult VisualizePayment(int id)
        {

            id = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
            ViewData["grafikManager"] = _personnelRepository.Personnels.Where(x => x.ManagerID == id).Include(x => x.Payments);


            List<Payment> payments = _paymentRepository.Payments.Select(x => new Payment
            {
                Description = x.Description,
                Amount =x.Amount

            }).ToList();
            return new JsonResult(payments);

        }
    }
}
