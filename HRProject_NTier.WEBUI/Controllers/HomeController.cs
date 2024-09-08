using HRProject_NTier.CORE.Entities;
using HRProject_NTier.DATAACCESS.Repositories.Abstract;
using HRProject_NTier.WEBUI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HRProject_NTier.WEBUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPackageRepository _packageRepository;
        private readonly IManagerCommentRepository _managerCommentRepository;
        private readonly IManagerRepository _managerRepository;
        public readonly IVisitorCommentsRepository _visitorCommentsRepository;
        public readonly IAdminRepository _adminRepository;
        
        public HomeController(ILogger<HomeController> logger, IPackageRepository packageRepository, IManagerCommentRepository managerCommentRepository, IManagerRepository managerRepository, IVisitorCommentsRepository visitorCommentsRepository,IAdminRepository adminRepository)
        {
            _logger = logger;
            _packageRepository = packageRepository;
            _managerCommentRepository = managerCommentRepository;
            _managerRepository = managerRepository;
            _visitorCommentsRepository = visitorCommentsRepository;
            _adminRepository = adminRepository;
        }

        public IActionResult Index()
        {

             return View(Tuple.Create<List<Package>,List<ManagerComment> ,List<Manager>>(_packageRepository.Packages.ToList(), _managerCommentRepository.Comments.ToList(), _managerRepository.Managers.ToList()));

           
            
        }
        [HttpPost]
        public IActionResult Index([Bind(Prefix ="item1")] Package item1, [Bind(Prefix = "item2")] ManagerComment item2, [Bind(Prefix ="item3")] Manager item3)
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Abouth()
        {
            return View();
        }


        public IActionResult LoginButton()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Contact(VisitorComments visitorComments)
        {
            //visitorComments.AdminID = Convert.ToInt32(HttpContext.Session.GetInt32("id"));

            visitorComments.AdminID =_adminRepository.Admins.FirstOrDefault().ID;
            _visitorCommentsRepository.InsertVisitorComments(visitorComments);
           


            return RedirectToAction("Contact");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
