using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HRProject_NTier.CORE.Entities
{
    public class ManagerComment
    {
        public int ID { get; set; }
        public string Comment { get; set; }
        [Required]
        public bool IsActived { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public bool IsApproved { get; set; }

        [ForeignKey("Manager")]
        public int ManagerID { get; set; }//FK
       
        public Manager Manager { get; set; }
    }
}
