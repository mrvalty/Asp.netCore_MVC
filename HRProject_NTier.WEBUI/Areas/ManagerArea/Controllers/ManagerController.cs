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

namespace HRProject_NTier.WEBUI.Areas.ManagerArea.Controllers
{
    [Area("ManagerArea")]
    public class ManagerController : Controller
    {
        private IManagerRepository _managerRepository;
        private IPersonnelRepository _personnelRepository;
        private IWebHostEnvironment _managerEnvironment;
        private IPermissionRepository _permissionRepository;
        private IPaymentRepository _paymentRepository;
        private IPackageRepository _packageRepository;
        //private IWebHostEnvironment webHostEnviroment;
        private IWebHostEnvironment _personnelEnvironment;
        private IManagerCommentRepository _managerCommentRepository;
        private ProjectContext _context;
        private ISpendingRepository _spendingRepository;
        private IPersonnelRaporRepository _personnelRaporRepository;
        private ICompanyRepository _companyRepository;


        public ManagerController(IManagerRepository managerRepository, IWebHostEnvironment managerEnvironment, IPersonnelRepository personnelRepository, IPermissionRepository permissionRepository, IPaymentRepository paymentRepository, IPackageRepository packageRepository, IWebHostEnvironment personelEnvironment, IManagerCommentRepository managerCommentRepository, ProjectContext projectContext, ISpendingRepository spendingRepository, IPersonnelRaporRepository personnelRaporRepository,ICompanyRepository companyRepository)
        {
            _managerRepository = managerRepository;
            _managerEnvironment = managerEnvironment;
            _personnelRepository = personnelRepository;
            _permissionRepository = permissionRepository;
            _paymentRepository = paymentRepository;
            _packageRepository = packageRepository;
            _personnelEnvironment = personelEnvironment;
            _managerCommentRepository = managerCommentRepository;
            _context = projectContext;
            _spendingRepository = spendingRepository;
            _personnelRaporRepository = personnelRaporRepository;
            _companyRepository = companyRepository;

        }
        public IActionResult Index()
        {
            return View(Tuple.Create<List<Manager>, List<Personnel>, List<Permission>, List<Package>>(_managerRepository.Managers.ToList(), _personnelRepository.Personnels.ToList(), _permissionRepository.Permissions.ToList(), _packageRepository.Packages.ToList()));
        }

        [HttpGet]
        public IActionResult UpdateManager() => View(_managerRepository.GetById(Convert.ToInt32(HttpContext.Session.GetInt32("id"))));


        [HttpPost]
        public IActionResult UpdateManager(Manager manager)
        {

            manager.ID = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
            manager.IsActived = true;

            manager.ModifiedDate = DateTime.Now;

            if (manager.Password == null || manager.Name == null || manager.MailAddress == null)
            {

                return View(manager);
            }
            else
            {

                if (manager.ProfileImageFile != null)//Resim boş gelmesi kontrolü
                {
                    //resimyükleme
                    string managerImage = Path.Combine(_managerEnvironment.WebRootPath, "images");
                    if (manager.ProfileImageFile.Length > 0)
                    {
                        using (FileStream file = new FileStream(Path.Combine(managerImage, manager.ProfileImageFile.FileName), FileMode.Create))
                        {
                            manager.ProfileImageFile.CopyTo(file);
                        }
                    }
                    manager.ProfileImage = manager.ProfileImageFile.FileName;

                    _managerRepository.UpdateManager(manager);
                    return RedirectToAction("ManagerPanel", "Manager");
                }
                else
                {
                    _managerRepository.UpdateManager(manager);
                    return RedirectToAction("ManagerPanel", "Manager");
                }
            }

        }
        //Payment
        [Route("/Harcama-Ekle")]
        public IActionResult AddPayment()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddPayment(InsertPaymentVM paymentVM)
        {
            paymentVM.PersonelID = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
            _paymentRepository.AddPayment(paymentVM);
            ViewData["MesajKaydet"] = "Avans başarı ile kayıt edilmiştir.";
            return RedirectToAction("ListPayment");
        }

        //AvansGüncelleme
        public IActionResult UpdatePayment(Payment payment, int id)
        {
            var result = _paymentRepository.GetById(id);
            if (result.IsActived == false)
            {
                result.IsActived = true;
                result.IsApproved = false;
                _paymentRepository.UpdatePayment(result);
            }
            return RedirectToAction(nameof(ListPayment));
        }

        public IActionResult ReddedilenAvans()
        {
            var result = (from p in _context.Payments
                          join per in _context.Personnels
                          on p.PersonnelID equals per.ID
                          where p.IsApproved == true
                          select new PaymentManagerPersonelVM
                          {
                              PersonelName = per.Name,
                              PersonelSurname = per.Surname,
                              PaymentAmount = p.Amount,
                              Description = p.Description,
                              CreatedDate = p.CreatedDate,
                              IsActived = p.IsActived,
                              IsDeleted = p.IsDeleted,
                              IsApproved = p.IsApproved,
                              PersonelID = per.ID,
                              ID = p.ID,
                              RequestDate = p.RequestDate,
                              //Message=p.Message
                          }).ToList();
            ViewData["reddedilen"] = _paymentRepository.Payments.Where(p => p.IsApproved == true).Count();
            ViewData["onaylanan"] = _paymentRepository.Payments.Where(p => p.IsActived == true).Count();
            ViewData["onaylananList"] = _paymentRepository.Payments.Count();
            ViewData["onaylananBekleyen"] = _paymentRepository.Payments.Where(p => p.IsActived == false && p.IsApproved == false).Count();
            return View(result);
        }

        public IActionResult OnaylananAvans()
        {
            var result = (from p in _context.Payments
                          join per in _context.Personnels
                          on p.PersonnelID equals per.ID
                          where p.IsActived == true
                          select new PaymentManagerPersonelVM
                          {
                              PersonelName = per.Name,
                              PersonelSurname = per.Surname,
                              PaymentAmount = p.Amount,
                              Description = p.Description,
                              CreatedDate = p.CreatedDate,
                              IsActived = p.IsActived,
                              IsDeleted = p.IsDeleted,
                              IsApproved = p.IsApproved,
                              PersonelID = per.ID,
                              ID = p.ID,
                              RequestDate = p.RequestDate,
                              //Message=p.Message
                          }).ToList();
            ViewData["onaylanan"] = _paymentRepository.Payments.Where(p => p.IsActived == true).Count();
            ViewData["reddedilen"] = _paymentRepository.Payments.Where(p => p.IsApproved == true).Count();
            ViewData["onaylananList"] = _paymentRepository.Payments.Count();
            ViewData["onaylananBekleyen"] = _paymentRepository.Payments.Where(p => p.IsActived == false && p.IsApproved == false).Count();
            return View(result);
        }


        //AvansSilme
        [Route("/Avans-Silme")]
        public IActionResult DeletePayment(Payment payment, int id)
        {
            var result = _paymentRepository.GetById(id);
            if (result.IsApproved == false)
            {
                result.IsActived = false;
                result.IsApproved = true;
                _paymentRepository.DeletePayment(result);
            }
            return RedirectToAction(nameof(ListPayment));
        }

        //onaybekleyen
        public IActionResult onayBekleyen()
        {
            var result = (from p in _context.Payments
                          join per in _context.Personnels
                          on p.PersonnelID equals per.ID
                          where p.IsActived == false && p.IsApproved == false
                          select new PaymentManagerPersonelVM
                          {
                              PersonelName = per.Name,
                              PersonelSurname = per.Surname,
                              PaymentAmount = p.Amount,
                              Description = p.Description,
                              CreatedDate = p.CreatedDate,
                              IsActived = p.IsActived,
                              IsDeleted = p.IsDeleted,
                              IsApproved = p.IsApproved,
                              PersonelID = per.ID,
                              ID = p.ID,
                              RequestDate = p.RequestDate,
                              //Message=p.Message
                          }).ToList();
            ViewData["onaylananBekleyen"] = _paymentRepository.Payments.Where(p => p.IsActived == false && p.IsApproved == false).Count();
            ViewData["onaylanan"] = _paymentRepository.Payments.Where(p => p.IsActived == true).Count();
            ViewData["reddedilen"] = _paymentRepository.Payments.Where(p => p.IsApproved == true).Count();
            ViewData["onaylananList"] = _paymentRepository.Payments.Count();
            return View(result);
        }
        //AvansListeleme
        public IActionResult ListPayment()
        {
            var result = (from p in _context.Payments
                          join per in _context.Personnels
                          on p.PersonnelID equals per.ID
                          select new PaymentManagerPersonelVM
                          {
                              PersonelName = per.Name,
                              PersonelSurname = per.Surname,
                              PaymentAmount = p.Amount,
                              Description = p.Description,
                              CreatedDate = p.CreatedDate,
                              IsActived = p.IsActived,
                              IsDeleted = p.IsDeleted,
                              IsApproved = p.IsApproved,
                              PersonelID = per.ID,
                              ID = p.ID,
                              RequestDate = p.RequestDate,
                              //Message=p.Message
                          }).ToList();
            ViewData["onaylananList"] = _paymentRepository.Payments.Count();
            ViewData["onaylanan"] = _paymentRepository.Payments.Where(p => p.IsActived == true).Count();
            ViewData["reddedilen"] = _paymentRepository.Payments.Where(p => p.IsApproved == true).Count();
            ViewData["onaylananBekleyen"] = _paymentRepository.Payments.Where(p => p.IsActived == false && p.IsApproved == false).Count();


            return View(result);

        }


        //Bildirimleri göster
        [HttpGet]
        public IActionResult ShowNotice()
        {
            /*----------------------------------------Avanslar------------------------------------------*/
            //ViewBag.onaylananList = _paymentRepository.Payments.Count(); //


            //ViewBag.onaylanan = _paymentRepository.Payments.Where(p => p.IsActived == true).Count();
            //ViewBag.reddedilen = _paymentRepository.Payments.Where(p => p.IsApproved == true).Count();


            //ViewData["onaylananBekleyen"] = _paymentRepository.Payments.Where(p => p.IsActived == false && p.IsApproved == false).Count();

            ViewBag.onaylananBekleyen = _paymentRepository.Payments.Where(p => p.IsActived == false && p.IsApproved == false).Count();


            /*--------------------------------------- İzin Talepleri-----------------------------------------------*/
            ViewData["IzinTalebi"] = _permissionRepository.Permissions.Where(x => x.StartedDate.Month == DateTime.Now.Month && x.StartedDate.Day - DateTime.Now.Day <= 3).Include(x => x.Personnel).ToList();

            return View();

            //return View(Tuple.Create<int,IEnumerable<Personnel>,IEnumerable<Permission>>(_paymentRepository.Payments.Where(p => p.IsActived == true).Count(), _personnelRepository.Personnels.Where(x => x.BirthDate.Month == DateTime.Now.Month && x.BirthDate.Day - DateTime.Now.Day <= 3 && x.BirthDate.Day - DateTime.Now.Day > 0).ToList(), _permissionRepository.Permissions.Where(x => x.StartedDate.Month == DateTime.Now.Month && x.StartedDate.Day - DateTime.Now.Day <= 3).Include(x => x.Personnel).ToList()));
        }
        [HttpPost]
        public IActionResult ShowNotice(/*[Bind(Prefix ="item1")] int item1,[Bind(Prefix ="item2")] IEnumerable<Personnel> item2,[Bind(Prefix ="item3")] IEnumerable<Permission> item3*/ int a)
        {
            return View();
        }

        public IActionResult ListPackage()
        {

            List<ListPackageVM> listPackageVMs = _packageRepository
                .Packages
                .Select(x => new ListPackageVM
                {
                    Name = x.Name,
                    PersonnelNumber = x.PersonnelNumber,
                    StartedDate = x.StartedDate,
                    EndDate = x.EndDate,
                    Price = x.Price

                }).ToList();
            return View(listPackageVMs);
        }

        public IActionResult hesabiPasifeAl(int id)
        {
            Manager manager = _managerRepository.GetById(Convert.ToInt32(HttpContext.Session.GetInt32("id")));
            if (manager.IsActived != false)
            {
                manager.IsActived = false;
                _managerRepository.UpdateManager(manager);

                List<Personnel> plist = _personnelRepository.Personnels
                    .Where(p => p.ManagerID == Convert.ToInt32(HttpContext.Session.GetInt32("id"))).ToList();

                foreach (var item in plist)
                {
                    item.IsActived = false;

                }
                _context.Personnels.UpdateRange(plist);
                _context.SaveChanges();

                //ViewData["mesaj"] = "Yönetici tarafından, Yönetici ve yöneticiye ait tüm personel hesapları pasif hale geçmiştir.";
                HttpContext.Session.Remove("username");
            }
            else
            {
                return RedirectToAction("Logout", "Login");
            }

            return RedirectToAction("Logout", "Login");
        }
        public IActionResult uyelikAyarlari()
        {
            return View();
        }

        [Route("/Personel-Listesi")]
        public IActionResult ListPersonnel(Manager manager, int id)
        {
            id = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
            ViewData["personeller"] = _personnelRepository.Personnels.Where(x => x.ManagerID == id).Include(x => x.Manager).ToList();

            return View();
        }


        //PersonelEkle
        
        [HttpGet]
        public IActionResult AddPersonnel()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddPersonnel(InsertPersonnelVM personnelVM)
        {
            personnelVM.ManagerID = Convert.ToInt32(HttpContext.Session.GetInt32("id"));     
            _personnelRepository.InsertPersonelPhoto(personnelVM);
            return RedirectToAction("ListPersonnel");
        }

        //AktiveİzinListeleme
        [Route("/Onay-Bekleyen-İzin-Listesi")]
        public IActionResult ListActivePermissions()
        {
            //ViewData[("PersonelPermissionsDetails")] = _permissionRepository.Permissions.Where(x => x.IsActived == true).Include(x => x.Personnel).ToList();
            ViewData[("PersonelPermissionsDetails")] = _permissionRepository.Permissions.Where(x => x.IsActived == true && x.IsApproved == false).Include(x => x.Personnel).ToList();

            ViewBag.BekleyenSayisi = _permissionRepository.Permissions.Where(x => x.IsActived == true && x.IsApproved == false).Include(x => x.Personnel).Count();
            ViewBag.ReddedilenSayisi = _permissionRepository.Permissions.Where(x => x.IsActived == false && x.IsApproved == false).Include(x => x.Personnel).Count();
            ViewBag.OnaylananSayisi = _permissionRepository.Permissions.Where(x => x.IsActived == false && x.IsApproved == true).Include(x => x.Personnel).Count();

            return View("ListActivePermissions");
        }

        //İzinTalebiReddedilmişleriListeleme
        [Route("/Reddedilen-İzin-Listesi")]
        [HttpGet]
        public IActionResult ListDissaprovedPermissions()
        {
            ViewData[("PersonelPermissionsDisapprovals")] = _permissionRepository.Permissions.Where(x => x.IsActived == false && x.IsApproved == false).Include(x => x.Personnel).ToList();

            ViewBag.BekleyenSayisi = _permissionRepository.Permissions.Where(x => x.IsActived == true && x.IsApproved == false).Include(x => x.Personnel).Count();
            ViewBag.ReddedilenSayisi = _permissionRepository.Permissions.Where(x => x.IsActived == false && x.IsApproved == false).Include(x => x.Personnel).Count();
            ViewBag.OnaylananSayisi = _permissionRepository.Permissions.Where(x => x.IsActived == false && x.IsApproved == true).Include(x => x.Personnel).Count();

            return View("ListDissaprovedPermissions");

        }

        //Onaylanan izinlerin listesi
        [Route("/Onaylanan-İzin-Listesi")]

        public IActionResult ListApprovePermissions()
        {
            ViewData[("PersonelPermissionsApprovals")] = _permissionRepository.Permissions.Where(x => x.IsActived == false && x.IsApproved == true).Include(x => x.Personnel).ToList();

            ViewBag.BekleyenSayisi = _permissionRepository.Permissions.Where(x => x.IsActived == true && x.IsApproved == false).Include(x => x.Personnel).Count();
            ViewBag.ReddedilenSayisi = _permissionRepository.Permissions.Where(x => x.IsActived == false && x.IsApproved == false).Include(x => x.Personnel).Count();
            ViewBag.OnaylananSayisi = _permissionRepository.Permissions.Where(x => x.IsActived == false && x.IsApproved == true).Include(x => x.Personnel).Count();
            ViewBag.yaklasanDogumGunu = _personnelRepository.Personnels.Where(x => x.BirthDate.Month == DateTime.Now.Month && (x.BirthDate.Day - DateTime.Now.Day <= 3 && x.BirthDate.Day - DateTime.Now.Day > 0)).ToList();


            return View("ListApprovePermissions");

        }

        //KişiBazlıPersonelİzinGörüntüleme
        [Route("/Personel-Bekleyen-İzin-Listesi")]
        [HttpGet]
        public IActionResult ListPersonnelPermissions(int id)
        {
            ViewData[("PersonelPermissionsDetails")] = _permissionRepository.Permissions.Where(x => x.PersonnelID == id && x.IsActived == true && x.IsApproved == false).Include(x => x.Personnel).ToList();
            return View();
        }

        //İzinOnaylama

        [HttpGet]
        public IActionResult ApprovedPermission(int id)
        {
            var permission = _permissionRepository.Permissions.Where(x => x.ID == id).FirstOrDefault();
            _permissionRepository.ApprovePermission(permission);
            return RedirectToAction("ListActivePermissions");
        }
        //İzinReddetme
        [HttpGet]
        public IActionResult DisApprovePermission(int id)
        {
            var permission = _permissionRepository.Permissions.FirstOrDefault(x => x.ID == id);
            _permissionRepository.DisApprovePermission(permission);

            return RedirectToAction("ListActivePermissions");
        }

        [HttpGet]
        public IActionResult SendDisMessage(int id, string description)
        {
            Permission permission = _permissionRepository.GetByID(id);
            permission.Description = description;
            _permissionRepository.UpdatePermission(permission);
            return RedirectToAction("ListActivePermissions");
        }

        // YaklaşanDoğumGünleri ve İzin Taleplerini Görüntüleme
        [HttpGet]
        public IActionResult ShowUpComingEvent(Personnel personnel, Permission permission)
        {

            var permissions = _permissionRepository.Permissions.Where(x => x.StartedDate.Month == DateTime.Now.Month && x.StartedDate.Day - DateTime.Now.Day <= 3 && DateTime.Now.Day - x.StartedDate.Day > 0).Include(x => x.Personnel).ToList();

            return View();
        }

        [HttpPost]
        public IActionResult ShowUpComingEvent([Bind(Prefix = "item1")] IEnumerable<Personnel> item1, [Bind(Prefix = "item2")] IEnumerable<Permission> item2)
        {
            return View();
        }

        //AktiveHarcamaListeleme
        [Route("/Aktif-Harcama-Listesi")]
        public IActionResult ListActiveSpending()
        {
            ViewData[("AktiveSpendings")] = _spendingRepository.Spendings.Where(x => x.IsActived == true && x.IsApproved == false).Include(x => x.Personnel).ToList();
            ViewBag.BekleyenSayisi = _spendingRepository.Spendings.Where(x => x.IsActived == true && x.IsApproved == false).Include(x => x.Personnel).Count();
            ViewBag.ReddedilenSayisi = _spendingRepository.Spendings.Where(x => x.IsActived == false && x.IsApproved == false).Include(x => x.Personnel).Count();
            ViewBag.OnaylananSayisi = _spendingRepository.Spendings.Where(x => x.IsActived == false && x.IsApproved == true).Include(x => x.Personnel).Count();

            return View();
        }

        [Route("/Onaylanan-Harcama-Listesi")]
        public IActionResult ListAproovedSpending()
        {
            ViewData[("PersonelSpendingApprovals")] = _spendingRepository.Spendings.Where(x => x.IsActived == false && x.IsApproved == true).Include(x => x.Personnel).ToList();
            ViewBag.BekleyenSayisi = _spendingRepository.Spendings.Where(x => x.IsActived == true && x.IsApproved == false).Include(x => x.Personnel).Count();
            ViewBag.ReddedilenSayisi = _spendingRepository.Spendings.Where(x => x.IsActived == false && x.IsApproved == false).Include(x => x.Personnel).Count();
            ViewBag.OnaylananSayisi = _spendingRepository.Spendings.Where(x => x.IsActived == false && x.IsApproved == true).Include(x => x.Personnel).Count();

            return View("ListAproovedSpending");
        }
        [Route("/Reddedilen-Harcama-Listesi")]
        public IActionResult ListDissaprovedSpending()
        {
            ViewData[("PersonelSpendingsDisapprovals")] = _spendingRepository.Spendings.Where(x => x.IsActived == false && x.IsApproved == false).Include(x => x.Personnel).ToList();
            ViewBag.BekleyenSayisi = _spendingRepository.Spendings.Where(x => x.IsActived == true && x.IsApproved == false).Include(x => x.Personnel).Count();
            ViewBag.ReddedilenSayisi = _spendingRepository.Spendings.Where(x => x.IsActived == false && x.IsApproved == false).Include(x => x.Personnel).Count();
            ViewBag.OnaylananSayisi = _spendingRepository.Spendings.Where(x => x.IsActived == false && x.IsApproved == true).Include(x => x.Personnel).Count();

            return View("ListDissaprovedSpending");
        }

        //KişiBazlıHarcamaGörüntüleme
        [Route("/Personelin-Harcama-Listesi")]
        [HttpGet]
        public IActionResult ListPersonelSpending(int id)
        {
            ViewData[("PersonelSpendingDetails")] = _spendingRepository.Spendings.Where(x => x.PersonnelID == id && x.IsActived == true && x.IsApproved == false).Include(x => x.Personnel).ToList();
            return View();
        }

        [HttpGet]
        public IActionResult DisApproveSpending(int id)
        {
            var spending = _spendingRepository.Spendings.Where(x => x.ID == id).FirstOrDefault();
            _spendingRepository.DisApproveSpending(spending);

            return RedirectToAction("ListActiveSpending");
        }

        [HttpGet]
        public IActionResult SendSpendingDisMessage(int id, string description)
        {
            Spending spending = _spendingRepository.GetByID(id);
            spending.Description = description;
            _spendingRepository.UpdateSpending(spending);
            return RedirectToAction("ListActiveSpending");
        }

        [HttpGet]
        public IActionResult ApproveSpending(int id)
        {
            var spending = _spendingRepository.Spendings.Where(x => x.ID == id).FirstOrDefault();
            _spendingRepository.ApproveSpending(spending);

            return RedirectToAction("ListActiveSpending");
        }

        //PersonelDetayı
        [Route("/Personel-Detayı")]
        public IActionResult PersonnelDetails(int id)
        {
            int ManagerID = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
            
            ViewData["ManagerCompany"] = _managerRepository.Managers.Where(x => x.ID == ManagerID).Select(x => x.CompanyName).FirstOrDefault();

            return View(_personnelRepository.GetByID(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PersonnelDetails(Personnel personnel)
        {
            personnel.ManagerID = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
            _personnelRepository.UpdatePersonnel(personnel);
            return RedirectToAction("ListPersonnel");
        }

        public IActionResult PersonnelDetail(int id)
        {
            return View(_personnelRepository.GetByID(id));
        }
        [HttpPost]
        public IActionResult PersonnelDetail()
        {
            List<Personnel> personelDetay = _personnelRepository.Personnels.Select(x => new Personnel
            {
                Name = x.Name,
                Surname = x.Surname,
                MailAddress = x.MailAddress,
                Address = x.Address,
                Telephone = x.Telephone,
                BirthDate = x.BirthDate,
                Gender = x.Gender,
                City = x.City,
                Department = x.Department,
                HiredDate = x.HiredDate,
                FiredDate = x.FiredDate

            }).ToList();
            return View(personelDetay);
        }

        [HttpGet]
        public IActionResult PersonalInfo(int id)
        {
            var perupdate = _personnelRepository.Personnels.SingleOrDefault(x => x.ID == id);

            return View(perupdate);
        }

        [HttpPost]
        public IActionResult PersonalInfo(int id, Personnel personnel, bool Gender)
        {
            Personnel update = _personnelRepository.GetByID(id);
            update.ManagerID = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
            if (personnel.FileNamePath != null)
            {
                string files = Path.Combine(_personnelEnvironment.WebRootPath, "files");
                if (personnel.FileNamePath.Length > 0)
                {
                    using (var fileStream = new FileStream(Path.Combine(files, personnel.FileNamePath.FileName), FileMode.Create))
                    {
                        personnel.FileNamePath.CopyTo(fileStream);
                    }
                }
                update.FileName = personnel.FileNamePath.FileName;
            }
            update.BirthDate = personnel.BirthDate;
            update.HiredDate = personnel.HiredDate;
            update.IsActived = true;
            update.Gender = Gender;
            update.ModifiedDate = DateTime.Now;
            update.FiredDate = personnel.FiredDate;
            update.Address = personnel.Address;
            update.City = personnel.City;
            update.Salary = personnel.Salary;
            update.FileNamePath = personnel.FileNamePath;

            update.IsDeleted = false;

            _personnelRepository.UpdatePersonnel(update);
            return RedirectToAction("ListPersonnel");
        }

        //YorumGörüntüleme
        [Route("/Manager-Panel")]
        [HttpGet]
        public IActionResult ManagerPanel()
        {
            ViewBag.managerID = Convert.ToInt32(HttpContext.Session.GetInt32("id"));



            return View(Tuple.Create<IEnumerable<ManagerComment>, IEnumerable<Manager>>(_managerCommentRepository.Comments.ToList(), _managerRepository.Managers.ToList()));
        }
        [HttpPost]
        public IActionResult ManagerPanel([Bind(Prefix = "item1")] ManagerComment item1, [Bind(Prefix = "item2")] Manager item2)
        {
            return View();
        }

        [HttpGet]
        public IActionResult InsertManagerComment() => View();

        [HttpPost]
        public IActionResult InsertManagerComment(ManagerComment managerComment)
        {

            if (ModelState.IsValid)
            {
                managerComment.IsActived = true;
                managerComment.IsDeleted = false;
                managerComment.IsApproved = false;

                managerComment.ManagerID = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
                _managerCommentRepository.AddComment(managerComment);
              
                return RedirectToAction("ListManagerComment");

            }
            else
            {
                return View(managerComment);
            }

        }
        //MnaagerCommentList
        public IActionResult ListManagerComment()
        {
            return View(_managerCommentRepository.Comments.ToList());
        }
        //ManagerYorumGüncelle
        public IActionResult UpdateManagerComment() => View(_managerCommentRepository.GetById(Convert.ToInt32(HttpContext.Session.GetInt32("id"))));
        [HttpPost]
        public IActionResult UpdateManagerComment(ManagerComment managerComment)
        {
            //Burayı düzenle

            managerComment.ManagerID = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
            _managerCommentRepository.UpdateComment(managerComment);
            return RedirectToAction("ManagerPanel");
        }
      
        public IActionResult DeleteManagerComment(int id)
        {
            _managerCommentRepository.DeleteComment(id);
            return RedirectToAction("ListManagerComment", "Manager");
        }


        public IActionResult ShowCalendar()
        {
            return View();
        }

        public IActionResult GetCalendarEvents()
        {
            var events = _context.Calenders.ToList();
            return new JsonResult(events);
        }

        [HttpGet]
        public IActionResult personnelRapor()
        {
            return View();
        }

        [HttpPost]
        public IActionResult personnelRapor(PersonnelRapor personnelRapor)
        {
            personnelRapor.ManagerID = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
            _personnelRaporRepository.InsertPersonnelRapor(personnelRapor);
            return View(personnelRapor);
        }


    }
}
