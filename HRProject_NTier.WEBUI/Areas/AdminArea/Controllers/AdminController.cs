using HRProject_NTier.CORE.Entities;
using HRProject_NTier.CORE.ViewModels;
using HRProject_NTier.DATAACCESS.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace HRProject_NTier.WEBUI.Areas.AdminArea.Controllers
{

    [Area("AdminArea")]
    public class AdminController : Controller
    {
        private readonly IPackageRepository _packageRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IManagerRepository _managerRepository;
        private readonly IAdminRepository _adminRepository;
        public readonly IManagerCommentRepository _managerCommentRepository;
        public AdminController(IPackageRepository packageRepository, ICompanyRepository companyRepository, IManagerRepository managerRepository, IManagerCommentRepository managerCommentRepositor,IAdminRepository adminRepository)
        {
            _packageRepository = packageRepository;
            _companyRepository = companyRepository;
            _managerRepository = managerRepository;
            _managerCommentRepository = managerCommentRepositor;
            _adminRepository = adminRepository;
        }
        public IActionResult Index()
        {
            return View(Tuple.Create<List<Package>>(_packageRepository.Packages.ToList()));

        }
        //PaketSilme
        public IActionResult DeletePackage(Package package, int id)
        {
            var result = _packageRepository.GetByID(id);
            if (result.IsActived == true)
            {
                result.IsActived = false;
                result.IsDeleted = true;
                _packageRepository.DeletePackage(result);
            }
            return RedirectToAction(nameof(ListPackage));

        }
        //PaketListeleme
        public IActionResult ListPackage()
        {
            var result = _packageRepository.Packages;

            return View(result);
        }
        //ŞirketlerinListesi
        public IActionResult ListCompany()
        {
            var result = (from c in _companyRepository.Companies
                          join p in _packageRepository.Packages
                          on c.PackageID equals p.ID
                          join m in _managerRepository.Managers
                          on c.ManagerID equals m.ID
                          select new ListCompanyAdminVM
                          {
                              CompanyName = c.Name,
                              IsActived = c.IsActived,
                              IsDeleted = c.IsDeleted,
                              PersonnelNumber = p.PersonnelNumber,
                              StartedDate = p.StartedDate,
                              EndDate = p.EndDate,
                              PackageName = p.Name,
                              Name = m.Name,
                              Surname = m.Surname,
                              MailAddress = m.MailAddress


                          }).ToList();

            return View(result);
        }
        //Manager Yorumları

        public IActionResult ListManagerComment()
        {
            var result = (from m in _managerRepository.Managers
                          join mc in _managerCommentRepository.Comments
                          on m.ID equals mc.ManagerID

                          select new ManagerCommentVM
                          {
                              ID = mc.ID,
                              Name = m.Name,
                              Surname = m.Surname,
                              Comment = mc.Comment,
                              IsDeleted=mc.IsDeleted,
                              IsActived=mc.IsActived,
                              IsApproved=mc.IsApproved
                          }
                        ).ToList();
            ViewData["TumYorum"] = _managerCommentRepository.Comments.Count();
            ViewData["onaylanan"] = _managerCommentRepository.Comments.Where(p => p.IsApproved == true).Count();
            ViewData["onayBekleyenYorum"] = _managerCommentRepository.Comments.Where(p => p.IsApproved == false && p.IsActived == true).Count();
            ViewData["reddedilenYorum"] = _managerCommentRepository.Comments.Where(p => p.IsApproved == false && p.IsActived == false).Count();

            return View(result);
        }
        //ManagerCommentKabulEt
        public IActionResult DeleteAdmin(ManagerComment managerComment, int id)
        {
            var result = _managerCommentRepository.GetById(id);
            if (result.IsApproved == false)
            {
                result.IsActived = false;
                result.IsApproved = false;
                _managerCommentRepository.DeleteComment(result);
            }
            return RedirectToAction(nameof(ListManagerComment));
        }

        //Aktif
        public IActionResult UpdateAdmin(ManagerComment managerComment, int id)
        {
            var result = _managerCommentRepository.GetById(id);
            if (result.IsActived == true)
            {
                result.IsActived = false;
                result.IsApproved = true;
                _managerCommentRepository.UpdateComments(result);
            }
            return RedirectToAction(nameof(ListManagerComment));
        }
        //Pasif
      
        //OnaylananYorumlar
        public IActionResult onaylananYorum()
        {
            var result = (from m in _managerRepository.Managers
                          join mc in _managerCommentRepository.Comments
                          on m.ID equals mc.ManagerID
                          where mc.IsApproved == true && mc.IsActived == false
                          select new ManagerCommentVM
                          {
                              ID = mc.ID,
                              Name = m.Name,
                              Surname = m.Surname,
                              Comment = mc.Comment,
                              IsDeleted = mc.IsDeleted,
                              IsActived = mc.IsActived,
                              IsApproved = mc.IsApproved
                          }
                    ).ToList();
            ViewData["TumYorum"] = _managerCommentRepository.Comments.Count();
            ViewData["onaylanan"] = _managerCommentRepository.Comments.Where(p => p.IsApproved == true).Count();
            ViewData["onayBekleyenYorum"] = _managerCommentRepository.Comments.Where(p => p.IsApproved == false && p.IsActived == true).Count();
            ViewData["reddedilenYorum"] = _managerCommentRepository.Comments.Where(p => p.IsApproved == false && p.IsActived == false).Count();

            return View(result);
        }
        public IActionResult reddedilenYorum()
        {
            var result = (from m in _managerRepository.Managers
                          join mc in _managerCommentRepository.Comments
                          on m.ID equals mc.ManagerID
                          where mc.IsApproved == false && mc.IsActived == false
                          select new ManagerCommentVM
                          {
                              ID = mc.ID,
                              Name = m.Name,
                              Surname = m.Surname,
                              Comment = mc.Comment,
                              IsDeleted = mc.IsDeleted,
                              IsActived = mc.IsActived,
                              IsApproved = mc.IsApproved
                          }
                    ).ToList();
            ViewData["TumYorum"] = _managerCommentRepository.Comments.Count();
            ViewData["onaylanan"] = _managerCommentRepository.Comments.Where(p => p.IsApproved == true).Count();
            ViewData["onayBekleyenYorum"] = _managerCommentRepository.Comments.Where(p => p.IsApproved == false && p.IsActived == true).Count();
            ViewData["reddedilenYorum"] = _managerCommentRepository.Comments.Where(p => p.IsApproved == false && p.IsActived == false).Count();

            return View(result);
        }
        public IActionResult onayBekleyenYorum()
        {
            var result = (from m in _managerRepository.Managers
                          join mc in _managerCommentRepository.Comments
                          on m.ID equals mc.ManagerID
                          where mc.IsApproved == false && mc.IsActived == true
                          select new ManagerCommentVM
                          {
                              ID = mc.ID,
                              Name = m.Name,
                              Surname = m.Surname,
                              Comment = mc.Comment,
                              IsDeleted = mc.IsDeleted,
                              IsActived = mc.IsActived,
                              IsApproved = mc.IsApproved
                          }
                    ).ToList();
            ViewData["TumYorum"] =_managerCommentRepository.Comments.Count();
            ViewData["onaylanan"] = _managerCommentRepository.Comments.Where(p => p.IsApproved == true).Count();
            ViewData["onayBekleyenYorum"] = _managerCommentRepository.Comments.Where(p => p.IsApproved == false && p.IsActived==true).Count();
            ViewData["reddedilenYorum"] = _managerCommentRepository.Comments.Where(p => p.IsApproved == false  && p.IsActived==false).Count();
        
            return View(result);
        }


        Random rnd = new Random();


        [HttpGet]
        public IActionResult AddManager() => View();
        [HttpPost]
        public IActionResult AddManager(Manager manager)
        {
            manager.IsActived = true;
            manager.IsDeleted = false;
            manager.CreatedDate = DateTime.Now;
            manager.ModifiedDate = DateTime.Now;
            manager.Password = rnd.Next(1, 9).ToString() + Convert.ToChar(rnd.Next(97, 123)) + Convert.ToChar(rnd.Next(65, 91)) + Convert.ToChar(rnd.Next(97, 123)) + Convert.ToChar(rnd.Next(65, 91)) + Convert.ToChar(rnd.Next(97, 123)) + Convert.ToChar(rnd.Next(65, 91)) + rnd.Next(1, 99).ToString();
            if (manager.Surname == null || manager.Name == null || manager.CompanyName == null || manager.MailAddress == null)
            {
                return View(manager);
            }
            else
            {

                _managerRepository.AddManager(manager);
                SendActivationMail(manager);
                return RedirectToAction(nameof(ListManager));
            }

        }

        private void SendActivationMail(Manager manager)
        {


            using (MailMessage mm = new MailMessage("mvc.proje@hotmail.com", manager.MailAddress))

            {
                mm.Subject = "Yönetici Kullanıcı Adı ve şifresi";
                string body = "";

                body += manager.ManagerMailBody;
                //body += $"<br /><a>Kullanıcı Adınız:{personnel.Name}+{rnd.Next(1,100)}+{Convert.ToChar(rnd.Next(97, 123))}</a>";//Burayı ayarlayınca Login controller da da ayarlamaları yap
                body += $"<br /><a>Kullanıcı Adınız:{manager.MailAddress}</a>";
                body += $"<br /><a>Şifreniz:{manager.Password}</a>";

                body += "<br /><br />Teşekkürler";
                body += "<br />İyi Çalışmalar";

                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.EnableSsl = true;
                smtp.Host = "smtp.gmail.com";
                

                NetworkCredential NetworkCred = new NetworkCredential("mvc.proje@hotmail.com", "Ankara123.");
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }

        public IActionResult ListManager()
        {
            List<Manager> listManager = _managerRepository.Managers.Select(x => new Manager
            {
                ID=x.ID,
                Name = x.Name,
                Surname = x.Surname,
                MailAddress = x.MailAddress,
                Telephone = x.Telephone,
                Address = x.Address,
                City = x.City,
                CompanyName = x.CompanyName,
                Department = x.Department,
                IsActived = x.IsActived,
                IsDeleted = x.IsDeleted
            }
            ).ToList();
           
            return View(listManager);
        }
        public IActionResult DeleteManager(Manager manager, int id)
        {

            var result = _managerRepository.GetById(id);
            TempData["gelenManagerID"] = id;
            if (result.IsActived == true)
            {
                result.IsActived = false;
                result.IsDeleted = true;
                _managerRepository.DeleteManager(result);
            }
            return RedirectToAction("ListManager", "Admin");
        }
        //[Route("/Paket-Ekle")]
        [HttpGet]
        public IActionResult AddPackage()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult AddPackage(AddPackage package)
        {
            _adminRepository.InsertPack(package);
            return RedirectToAction("ListPackage");
        }

        public IActionResult DeleteManager1(Manager manager, int id)
        {

            var result = _managerRepository.GetById(id);
            TempData["gelenManagerID"] = id;
            if (result.IsActived == false)
            {
                result.IsActived = true;
                result.IsDeleted = false;
                _managerRepository.DeleteManager(result);
            }
            return RedirectToAction("ListManager", "Admin");
        }
    }
}
    

