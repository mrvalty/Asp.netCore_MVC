using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HRProject_NTier.CORE.Entities
{
    public class Payment
    {
        public int ID { get; set; }

        [Display(Name = "Açıklama")]
        [Required(ErrorMessage = "{0} alanı gereklidir.")]
       
        public string Description { get; set; }

        //girilecek tutar
        [Display(Name = "Ücret")]
        [Required(ErrorMessage = "{0} alanı gereklidir.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public float Amount { get; set; }
        


        [DataType(DataType.Date)]
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public bool IsActived { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public bool IsApproved { get; set; }
        public string Message { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime RequestDate { get; set; }


        //Relations

        //[ForeignKey("Manager")]
        //public int ManagerID { get; set; }

        [ForeignKey("Personnel")]
        public int PersonnelID { get; set; }

        public virtual Personnel Personnel { get; set; }
    }
}
