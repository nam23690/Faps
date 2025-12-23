using FAP.Common.Domain.Base;
using System.ComponentModel.DataAnnotations;

namespace FAP.Common.Infrastructure.EntitiesMaster
{
    public class AppPermission : BaseEntity
    {
        public string Module { set; get; }
        public string Code { set; get; }
        public string Description { set; get; }
        public bool IsActive { set; get; }
    }
}

