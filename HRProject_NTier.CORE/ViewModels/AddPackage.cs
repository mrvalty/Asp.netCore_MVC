using HRProject_NTier.CORE.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HRProject_NTier.CORE.ViewModels
{

    public enum Time
    {
        [Display(Name = "3 Ay")]
        threeMonths,
        [Display(Name = "6 Ay")]
        sixMonths,
        [Display(Name = "1 Yıl")]
        oneYear
    }

    public class AddPackage:BaseEntity
    {
        public int PersonnelNumber { get; set; }

        public DateTime StartedDate { get; set; }

        public DateTime EndDate { get; set; }

        public  Time Time { get; set; }
        [Display(Name = "Fiyat")]
        public float Price { get; set; }
    }
}
