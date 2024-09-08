using HRProject_NTier.CORE.Entities;
using HRProject_NTier.CORE.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRProject_NTier.DATAACCESS.Repositories.Abstract
{
    public interface IAdminRepository
    {
        IQueryable<Admin> Admins { get; }

        bool InsertAdmin(Admin admin);

        Admin GetByID(int id);

        bool UpdateAdmin(Admin admin);

        bool DeleteAdmin(int id);

        bool InsertPack(AddPackage packageVM);

    }
}
