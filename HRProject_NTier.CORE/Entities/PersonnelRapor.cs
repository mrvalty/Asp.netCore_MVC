using System;
using System.Collections.Generic;
using System.Text;

namespace HRProject_NTier.CORE.Entities
{
    public class PersonnelRapor
    {
        public int ID { get; set; }
        public int? TotalPermission { get; set; }//izin
        public int? TotalAdvance { get; set; }//avans


        public int? PersonnelID { get; set; }
        public int ManagerID { get; set; }
        public virtual Manager Manager { get; set; }//relation



    }
}
