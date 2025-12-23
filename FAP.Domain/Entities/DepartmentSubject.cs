using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FAP.Common.Domain.Entities
{
    public class DepartmentSubject
    {
        public short DepartmentID { get; set; }  
        public short SubjectID { get; set; }  
    }
}