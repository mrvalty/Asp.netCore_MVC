using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HRProject_NTier.CORE.Entities
{
    public enum PackageTime
    {
        [Display(Name = "3 Ay")]
        threeMonths,
        [Display(Name = "6 Ay")]
        sixMonths,
        [Display(Name = "1 Yıl")]
        oneYear
    }
    //public enum PackageType { Gold, Premium}
    //public enum PackagePersonelStock { twentyfive =25, fifty=50}
    public class Package : BaseEntity
    {
        public int PersonnelNumber { get; set; }

        public DateTime StartedDate { get; set; }

        public DateTime EndDate { get; set; }

        public PackageTime PackageTime { get; set; }
        [Display(Name = "Fiyat")]
        public float Price { get; set; }
        //Relations
        public virtual ICollection<Company> Companies { get; set; }
    }
}
