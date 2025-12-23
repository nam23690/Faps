using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FAP.Common.Domain.Entities;

namespace FAP.Common.Domain.Entities
{
    public class Campus
    {
        [Key]
        public short CampusID { get; set; }  
        public string CampusName { get; set; }
        public int? LocationId { get; set; }
        public bool? IsEnglish { get; set; }
        public string ContactName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
}