using HRProject_NTier.CORE.Entities;
using HRProject_NTier.DATAACCESS.Repositories.Abstract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HRProject_NTier.WEBUI.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IManagerRepository _managerRepository;
        private readonly IPackageRepository _packageRepository;
        private IWebHostEnvironment _companyEnvironment;

        public CompanyController(ICompanyRepository companyRepository, IManagerRepository managerRepository, IPackageRepository packageRepository, IWebHostEnvironment companyEnvironment)
        {
            _companyRepository = companyRepository;
            _managerRepository = managerRepository;
            _packageRepository = packageRepository;
            _companyEnvironment = companyEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddCompany() => View();
        [HttpPost]
        public IActionResult AddCompany(Company company, string mail, string packageName)
        {
            company.IsActived = true;
            company.IsDeleted = false;
            company.CreatedDate = DateTime.Now;
            company.ModifiedDate = DateTime.Now;
            if (company.Name == null || mail == null || packageName == null)
            {
                ViewBag.nullException = "Mail adresini ve paket ismini lütfen giriniz!";
                return View();
            }
            else
            {              
                {
                    try
                    {
                        company.PackageID = _packageRepository.Packages.FirstOrDefault(x => x.Name == packageName).ID;
                        company.ManagerID = _managerRepository.Managers.FirstOrDefault(x => x.MailAddress == mail).ID;
                        company.AdminID = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
                        _companyRepository.InsertCompany(company);
                        return RedirectToAction("ListCompany", "Admin");
                    }
                    catch (DbUpdateException)
                {

                    ViewBag.mailRemark = "Aynı mail adresini kullanamazsınız!";
                    return View();
                }
                    catch(NullReferenceException)
                    {
                        ViewBag.mailRemark1 = "Sistem de böyle kayıtlı bir mail adresi bulunmamaktadır.";
                        return View();
                    }
                              
                }
            }
        }

    } 
}

