using HRProject_NTier.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRProject_NTier.CORE.ViewModels
{
    public class ListCompanyAdminVM : BaseEntity
    {
        public string CompanyName { get; set; }
        public int PersonnelNumber { get; set; }
        public DateTime StartedDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PackageName { get; set; }
        public string Surname { get; set; }
        public string MailAddress { get; set; }

       
    }
}
