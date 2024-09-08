using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HRProject_NTier.CORE.Entities
{
   public class Calender
    {
        public int ID { get; set; }
        public bool IsActived { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public string title { get; set; }
        public string description { get; set; }
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }

        [ForeignKey("Manager")]
        public int ManagerID { get; set; }

        //Relations
        public Manager Manager { get; set; }
        public virtual ICollection<CalenderTopic> CalenderTopics { get; set; }
    }
}
