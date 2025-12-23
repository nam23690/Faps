using FAP.Common.Domain.Base;


namespace FAP.Common.Domain.Entities
{
    public class AuditLog:BaseEntity
    {
       
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public string? Action { get; set; }
        public string? Entity { get; set; }
        public string? EntityId { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? CampusCode { get; set; }
        public string? IpAddress { get; set; }
        
    }

}
