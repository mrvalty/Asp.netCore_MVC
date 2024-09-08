using HRProject_NTier.CORE.Entities;
using HRProject_NTier.DATAACCESS.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRProject_NTier.WEBUI.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class LoginController : Controller
    {
        private IAdminRepository _adminRepository;
        public LoginController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }
      

        public IActionResult LoginForm()
        {
            return View();
        }
        [Route("AdminArea")]
        [HttpPost]
        public IActionResult AdminLogin(string mail, string password)
        {
            Admin giris = _adminRepository.Admins.FirstOrDefault(x => x.MailAddress == mail && x.Password == password );
            if (giris != null && mail.Equals(giris.MailAddress) && password.Equals(giris.Password))
            { 
                HttpContext.Session.SetInt32("id", giris.ID);
                return RedirectToAction("Index", "Home", new { area = "AdminArea" });
            }
            else
            {
                ViewBag.invalidManagerAccount = "Kullanıcı adı veya şifre hatalı!";
                return View("LoginForm");
            }
        }
        [Route("AdminArea/logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("mail");
            return RedirectToAction("LoginForm");
        }
    }
}
