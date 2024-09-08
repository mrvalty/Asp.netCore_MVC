using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HRProject_NTier.CORE.ViewModels
{
    public class ManagerCommentVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Comment { get; set; }
        [Required]
        public bool IsActived { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public bool IsApproved { get; set; }
    }
}
