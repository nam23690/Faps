using FAP.Common.Domain.Base;

namespace FAP.Common.Infrastructure.EntitiesMaster
{
    public class Campus : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ConnectionString { get; set; }
    }
}

