using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FAP.Common.Domain.Entities
{
    public class Department
    {
        [Key]
        public short DepartmentID { get; set; }  
        public string DepartmentName { get; set; }
        public short CampusID { get; set; }
    }
}