using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Application.Attributes
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AuditEntityAttribute : Attribute
    {
        public string EntityName { get; }

        public AuditEntityAttribute(string entityName)
        {
            EntityName = entityName;
        }
    }

}
