using HRProject_NTier.CORE.Entities;
using HRProject_NTier.DATAACCESS.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HRProject_NTier.WEBUI.Areas.PersonnelArea.Controllers
{
    [Area("PersonnelArea")]
    public class LoginController : Controller
    {
        private IPersonnelRepository _personnelRepository;
        public LoginController(IPersonnelRepository personnelRepository)
        {
            _personnelRepository = personnelRepository;
        }

        public IActionResult LoginForm(int id)
        {
            Personnel personnel = _personnelRepository.GetByID(id);
            return View(personnel);
        }


        [Route("PersonnelArea")]
        [HttpPost]
        public IActionResult PersonnelLogin(string mail, string password)
        {
            Personnel giris = _personnelRepository.Personnels.FirstOrDefault(x => x.MailAddress == mail && x.Password == password && x.IsActived == true);
            if (giris != null && mail.Equals(giris.MailAddress) && password.Equals(giris.Password))
            {

                HttpContext.Session.SetInt32("id", giris.ID);//PersonnelID
                return RedirectToAction("Index", "Home", new { area = "PersonnelArea" });

            }
            else
            {
                ViewBag.invalidManagerAccount = "Kullanıcı adı / şifre hatalı veya hesabınız aktif değil!";

                return View("LoginForm");
            }
        }
        [Route("PersonnelArea/logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("mail");
            //return RedirectToAction("LoginForm");
            return RedirectToAction("LoginButton", "Home", new { area = "" });
        }
    }
}
