using HRProject_NTier.CORE.Entities;
using HRProject_NTier.CORE.ViewModels;
using HRProject_NTier.DATAACCESS.Context;
using HRProject_NTier.DATAACCESS.Encryption;
using HRProject_NTier.DATAACCESS.Repositories.Abstract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace HRProject_NTier.WEBUI.Models
{
    public class PersonnelRepository : IPersonnelRepository
    {
        private ProjectContext _context;
        private IWebHostEnvironment webHostEnviroment;
        private IManagerRepository _managerRepository;
        private IPermissionRepository _permissionRepository;
        private ISpendingRepository _spendingRepository;


        public PersonnelRepository(ProjectContext context, IWebHostEnvironment hostEnviroment,IManagerRepository managerRepository, IPermissionRepository permissionRepository,ISpendingRepository spendingRepository)
        {
            _context = context;
             webHostEnviroment = hostEnviroment;
            _managerRepository = managerRepository;
            _permissionRepository = permissionRepository;
            _spendingRepository = spendingRepository;
        }

        public IQueryable<Personnel> Personnels => _context.Personnels;

        public bool DeletePersonnel(int id)
        {
            _context.Personnels.Remove(GetByID(id));
            return _context.SaveChanges() > 0;
        }

        public Personnel GetByID(int id)
        {
            return _context.Personnels.Find(id);
        }

        public bool InsertPersonnel(Personnel personnel)
        {
            _context.Personnels.Add(personnel);
            return _context.SaveChanges() > 0;
        }
        public bool UpdatePersonnel(Personnel personnel)
        {
            _context.Personnels.Update(personnel);
            return _context.SaveChanges() > 0;
        }


        Random rnd = new Random();
        //HttpContext HttpContext;
        public bool InsertPersonelPhoto(InsertPersonnelVM personnelVM)
        {
         

            //HttpContext.SessionSetInt32("id", );
            string uniqueFileName = UploadFile(personnelVM);

            var ps = rnd.Next(1, 9).ToString() + Convert.ToChar(rnd.Next(97, 123)) + Convert.ToChar(rnd.Next(65, 91)) + Convert.ToChar(rnd.Next(97, 123)) + Convert.ToChar(rnd.Next(65, 91)) + Convert.ToChar(rnd.Next(97, 123)) + Convert.ToChar(rnd.Next(65, 91)) + rnd.Next(1, 99).ToString();
            //string password = EncryptionHelper.ToMD5(ps);

            Personnel personnel = new Personnel()
            {
                Name = personnelVM.Name,
                Surname = personnelVM.Surname,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                IsActived = true,
                IsDeleted = false,
                MailAddress = personnelVM.MailAddress,
                PersonnelMailBody = personnelVM.PersonnelMailBody,
                Telephone = personnelVM.Telephone,
                BirthDate = personnelVM.BirthDate,
                Password = ps,
                CompanyName = personnelVM.CompanyName,
                Department = personnelVM.Department,
                ManagerID = personnelVM.ManagerID,
                ProfileImage = uniqueFileName

            };
            SendActivationMail(personnel);
            _context.Personnels.Add(personnel);
            return _context.SaveChanges() > 0;
        }

      

        //private void SendActivationMailNormal(Personnel personnel)
        //{


        //    using (MailMessage mm = new MailMessage("mvcblogproje1@gmail.com", personnel.MailAddress))

        //    {
        //        mm.Subject = "Personel Kullanıcı Adı ve şifresi";
        //        string body = "Merhaba ";
        //        body += "<br /><br />Şirketimizde işe başladığınıza çok seviniyoruz.Ailemize hoşgeldiniz.";
        //        body += $"<br /><a>Kullanıcı Adınız:{personnel.Name}</a>";
        //        body += $"<br /><a>Şifreniz:{personnel.Password}</a>";

        //        body += "<br /><br />Teşekkürler";
        //        body += "<br /><br />İyi Çalışmalar";
        //        mm.Body = body;
        //        mm.IsBodyHtml = true;
        //        SmtpClient smtp = new SmtpClient();
        //        smtp.Host = "smtp.gmail.com";
        //        smtp.EnableSsl = true;

        //        NetworkCredential NetworkCred = new NetworkCredential("mvcblogproje1@gmail.com", "mv,c@blog.proje1");
        //        smtp.UseDefaultCredentials = true;
        //        smtp.Credentials = NetworkCred;
        //        smtp.Port = 587;
        //        smtp.Send(mm);
        //    }
        //}


        private void SendActivationMail(Personnel personnel)
        {


            using (MailMessage mm = new MailMessage("mvc.proje@hotmail.com", personnel.MailAddress))

            {
                mm.Subject = "Personel Kullanıcı Adı ve şifresi";
                string body = "";

                body += personnel.PersonnelMailBody;
                //body += $"<br /><a>Kullanıcı Adınız:{personnel.Name}+{rnd.Next(1,100)}+{Convert.ToChar(rnd.Next(97, 123))}</a>";//Burayı ayarlayınca Login controller da da ayarlamaları yap
                //body += $"<br /><a>Kullanıcı Adınız:{personnel.Name}</a>";
                body += $"<br /><a>Şifreniz:{personnel.Password}</a>";

                body += "<br /><br />Teşekkürler";
                body += "<br />İyi Çalışmalar";

                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.outlook.com";
                smtp.EnableSsl = true;

                NetworkCredential NetworkCred = new NetworkCredential("mvc.proje@hotmail.com", "Ankara123.");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }


        private string UploadFile(InsertPersonnelVM ınsertPersonnelVM)
        {
            string uniqueFileName = null;
            if (ınsertPersonnelVM.ProfileImageFile!=null)
            {

           

            if (ınsertPersonnelVM.ProfileImageFile.ContentType.Contains("image"))
            {
                string uploadsFolder = Path.Combine(webHostEnviroment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + ınsertPersonnelVM.ProfileImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ınsertPersonnelVM.ProfileImageFile.CopyTo(fileStream);
                    return filePath.Substring(filePath.IndexOf("\\images\\"));
                }
            }
            else
            {
                return $"Doğru belge giriniz.";
            }

        }
            else
            {
                return null;
            }
        }
    

        public bool AddPermission(Permission permission)
        {
           

                permission.Name = "İzin Talebi";
                permission.IsActived = true;
                permission.IsApproved = false;
                permission.IsDeleted = false;
                permission.ManagerID = 1;

               _context.Permissions.Add(permission);
                return _context.SaveChanges() > 0;
        }

        public bool AddSpending(Spending spending)
        {


            spending.Name = "Harcama Talebi";
            spending.IsActived = true;
            spending.IsApproved = false;
            spending.IsDeleted = false;
            spending.CreatedDate = DateTime.Now;
   
    

            _context.Spendings.Add(spending);
            return _context.SaveChanges() > 0;
        }

    }
}
