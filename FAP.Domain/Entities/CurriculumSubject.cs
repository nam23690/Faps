using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using System.Text;

namespace FAP.Common.Domain.Entities
{
    public class CurriculumSubject
    {
        public string CurriculumCode { set; get; }
        public short SubjectID { set; get; }
        public bool IsRequired { set; get; }
        public int TermNo { get; set; } 

    }
}