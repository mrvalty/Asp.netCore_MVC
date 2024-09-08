using HRProject_NTier.CORE.Entities;
using HRProject_NTier.DATAACCESS.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRProject_NTier.WEBUI.Areas.ManagerArea.Controllers
{
    //[Route("account")]
    [Area("ManagerArea")]
    
    public class LoginController : Controller
    {
        private IManagerRepository _managerRepository;
        public LoginController(IManagerRepository managerRepository)
        {
            _managerRepository = managerRepository;
        }
        //[Authorize]
        //[Route("ManagerArea")]
        public IActionResult LoginForm()
        {
            return View();
        }
        [Route("ManagerArea")]
        [HttpPost]
        public IActionResult ManagerLogin(string mail, string password)
        {
            Manager giris = _managerRepository.Managers.FirstOrDefault(x => x.MailAddress==mail && x.Password == password);
            if (giris != null && mail.Equals(giris.MailAddress) && password.Equals(giris.Password) && giris.IsActived == true)
            {
                
                HttpContext.Session.SetInt32("id", giris.ID);//ManagerID
                return RedirectToAction("Index", "Home", new { area = "ManagerArea" });
            }
            else
            {
                ViewBag.invalidManagerAccount = "Mail Adresi / şifre hatalı veya yönetici aktif değil";
                return View("LoginForm");
            }
        }
        [Route("ManagerArea/logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("mail");
            return RedirectToAction("LoginForm");
        }
    }
}
