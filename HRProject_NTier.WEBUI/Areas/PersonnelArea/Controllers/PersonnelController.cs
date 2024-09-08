using HRProject_NTier.CORE.Entities;
using HRProject_NTier.CORE.ViewModels;
using HRProject_NTier.DATAACCESS.Context;
using HRProject_NTier.DATAACCESS.Repositories.Abstract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HRProject_NTier.WEBUI.Areas.PersonnelArea.Controllers
{
    [Area("PersonnelArea")]
    public class PersonnelController : Controller
    {
        private readonly IPersonnelRepository _personnelrepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ISpendingRepository _spendingRepository;
        private readonly IManagerRepository _managerRepository;
        private ProjectContext _context;

        public PersonnelController(IPersonnelRepository personnelRepository, IWebHostEnvironment environment, IPermissionRepository permissionRepository, IPaymentRepository paymentRepository, ISpendingRepository spendingRepository ,ProjectContext projectContext, IManagerRepository managerRepository)
        {
            _personnelrepository = personnelRepository;
            _permissionRepository = permissionRepository;
            _environment = environment;
            _paymentRepository = paymentRepository;
            _spendingRepository = spendingRepository;
            _context = projectContext;
            _managerRepository = managerRepository;
        }

        public IActionResult Index()
        {
            return View();
        }
        //Payment
        public IActionResult CashPay(int id)
        {
            TempData["perID"] = id;
            return RedirectToAction(nameof(AddPayment));
        }
        public IActionResult AddPayment()
        {
           

            return View();
        }
        [HttpPost]
        public IActionResult AddPayment(Payment payment)
        {
           
            if(payment.RequestDate < DateTime.Now)
            {
                ViewData["HataMesajıTarih"] = "Lüften Geçmiş Bir Tarih Girmeyiniz!";
                return View();
            }
            else
            {
                _paymentRepository.AddPayment(payment);
            }
            ViewData["MesajKaydet"] = "Avans başarı ile kayıt edilmiştir.";
            return RedirectToAction("ListPayment");
        }
        //AvansListeleme
        public IActionResult ListPayment()
        {
            int personelid = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
           ViewData["personelID"] = Convert.ToInt32(HttpContext.Session.GetInt32("id"));

            var result = (from p in _context.Payments
                          join per in _context.Personnels
                          on p.PersonnelID equals per.ID
                          where p.PersonnelID == personelid
                          select new Payment
                          {                      
                              Amount = p.Amount,
                              Description = p.Description,
                              CreatedDate = p.CreatedDate,
                              IsActived = p.IsActived,
                              IsDeleted = p.IsDeleted,
                              IsApproved = p.IsApproved,
                              RequestDate=p.RequestDate,
                              PersonnelID = personelid,
                           
                              
                          }).ToList();
     
            ViewData["onaylanan"] = result.Count();
       
            return View(result);
        }
        public IActionResult ResimDosyaEkle()
        {
            return View();
        }

        //resimDosya
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResimDosyaEkle([Bind("ID,IsActived,IsDeleted,CreatedDate,ModifiedDate,Name,Description,PersonnelID,InvoiceImageFile")] Spending spending)
        {
           
            spending.PersonnelID = 1;

            if (ModelState.IsValid)
            {
                ViewData["mesaj"] = "ModelState.Isvalid İçinde";


                var filePath = Path.Combine(_environment.WebRootPath, "Harcamalar");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                using (var dosyaAkisi = new FileStream(Path.Combine(filePath, spending.InvoiceImageFile.FileName), FileMode.Create))
                {
                    await spending.InvoiceImageFile.CopyToAsync(dosyaAkisi);
                }
                spending.InvoiceImage = spending.InvoiceImageFile.FileName;
            

                _context.Add(spending);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ResimDosyaListe));
            }
            return View(spending);
        }
        public IActionResult ResimDosyaListe()
        {
            var result = _context.Spendings;
            return View(result);
        }

        public IActionResult hesabiPasifeAl(int id)
        {
            Personnel personel = _personnelrepository.GetByID(id);
            if (personel.IsActived != false)
            {
                personel.IsActived = false;
                _personnelrepository.UpdatePersonnel(personel);
                HttpContext.Session.Remove("username");
            }
           
            else
            {
                return RedirectToAction("Logout", "Login");
            }
            return RedirectToAction("Logout", "Login");
        }
        public IActionResult PersonelUyelikAyarlari()
        {
            return View();
        }
    
        [Route("/İzin Talebinde Bulun")]
        [HttpGet]
        public IActionResult AddPermissionsPersonel()
        {
            ViewData["İzin Tipi"] = _permissionRepository.Permissions.Select(x => x.permissionType).ToList();
            return View();
        }


        [HttpGet]
        public IActionResult UpdatePersonnel(int id)
        {
            Personnel personnel1 = _personnelrepository.GetByID(id);
            personnel1.ID = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
            return View(personnel1);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePersonnel(Personnel personnel, int id)
        {
            //Personnel personnel1 = _personnelrepository.GetByID(id);
            //personnel1.ID = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
            personnel.IsActived = true;
            personnel.ManagerID = 1;
            _personnelrepository.UpdatePersonnel(personnel);
            //return RedirectToAction("ListPersonnel");
            return View(personnel);
            //return RedirectToAction("PersonnelShow", "Personnel", new { area = "PersonnelArea" });
        }


        public IActionResult PersonnelDetail()
        {
            return View(_personnelrepository.GetByID(Convert.ToInt32(HttpContext.Session.GetInt32("id"))));
        }
        [HttpPost]
        public IActionResult PersonnelDetail(Personnel personnel)
        {
            personnel.ID = Convert.ToInt32(HttpContext.Session.GetInt32("id"));

            List<Personnel> personelDetay = _personnelrepository.Personnels.Select(x => new Personnel
            {
                Name = x.Name,
                Surname = x.Surname,
                MailAddress = x.MailAddress,
                Address = x.Address,
                Password=x.Password,
                Telephone = x.Telephone,
                BirthDate = x.BirthDate,
                Gender = x.Gender,
                City = x.City,
                Department = x.Department,
                HiredDate = x.HiredDate,
                Salary=x.Salary
               

            }).ToList();
            return View(personelDetay);
        }

        public IActionResult ListPermission(Permission permission,int id)
        {
            id = Convert.ToInt32(HttpContext.Session.GetInt32("id"));

            var permissions = _permissionRepository.Permissions.Where(p => p.PersonnelID == id).Select(x => new Permission
            {
                Name = x.Name,
                Description = x.Description,
                StartedDate = x.StartedDate,
                EndDate = x.EndDate,
                CreatedDate = x.CreatedDate,
                IsActived = x.IsActived,
                IsApproved = x.IsApproved,
            }).ToList();

            ViewData["Kullanılansure"] = _permissionRepository.Permissions.Where(p => p.Name == "Süt İzni").Select(p => p.EndDate.Day - p.StartedDate.Day).ToList();
            return View(permissions);
          
        }

        [Route("/İzin-Talebinde-Bulun")]
        [HttpGet]
        public IActionResult AddPermissionAsPersonel()
        {
            ViewData["PermissionTypes"] = _permissionRepository.Permissions.ToList();
            return View();
        }

        [Route("/İzin-Talebinde-Bulun")]
        [HttpPost]
        public IActionResult AddPermissionAsPersonel(Permission permission)
        {
            int personelid = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
            permission.PersonnelID = personelid;

            _personnelrepository.AddPermission(permission);
            return RedirectToAction("ListPermission", "Personnel", new { area = "PersonnelArea" });
        }

        [Route("/Harcama-Talebinde-Bulun")]
        [HttpGet]
        public IActionResult AddSpendingAsPersonel()
        {
            ViewData["SpendingTypes"] = _spendingRepository.Spendings.ToList();
           
            return View();
        }

        [Route("/Harcama-Talebinde-Bulun")]
        [HttpPost]
        public IActionResult AddSpendingAsPersonel(Spending spending)
        {
            int personelid = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
            spending.PersonnelID = personelid;

            _personnelrepository.AddSpending(spending);
            return RedirectToAction("ListSpending", "Personnel");
        }
        [HttpGet]
        public IActionResult ListSpending()
        {
            //ViewData["personelSpending"] = _spendingRepository.Spendings.ToList();

            return View(_spendingRepository.Spendings.ToList());
        }
    }
}
