using HRProject_NTier.CORE.Entities;
using HRProject_NTier.CORE.ViewModels;
using HRProject_NTier.DATAACCESS.Context;
using HRProject_NTier.DATAACCESS.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRProject_NTier.DATAACCESS.Repositories.Concrete
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ProjectContext _context;
        public AdminRepository(ProjectContext context)
        {
            _context = context;
        }
        public IQueryable<Admin> Admins => _context.Admins;

        public bool DeleteAdmin(int id)
        {
            _context.Admins.Remove(GetByID(id));
            return _context.SaveChanges() > 0;
        }

        public Admin GetByID(int id)
        {
            return _context.Admins.Find(id);
        }

        public bool InsertAdmin(Admin admin)
        {
            _context.Admins.Add(admin);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateAdmin(Admin admin)
        {
            _context.Admins.Update(admin);
            return _context.SaveChanges() > 0;
        }

        public bool InsertPack(AddPackage packageVM)
        {
            Package package = new Package()
            {
                Name = packageVM.Name,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                IsActived = true,
                IsDeleted = false,
                PersonnelNumber = packageVM.PersonnelNumber,
                StartedDate = packageVM.StartedDate,
                EndDate = packageVM.EndDate,
                PackageTime = (PackageTime)packageVM.Time,
                Price = packageVM.Price,
            };

            _context.Packages.Add(package);
            return _context.SaveChanges() > 0;
        }


    }
}
