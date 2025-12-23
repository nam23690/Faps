using FAP.Common.Domain.Base;

namespace FAP.Common.Domain.Entities
{
    public class User:BaseEntity
    {
        public string Login { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public short? CampusID { get; set; }
        public string? Mark_Key { get; set; }
        public string? AlternativeEmail { get; set; }
        public string? IDFeID { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool? GAEnabled { get; set; }
        public string? GASecretKey { get; set; }
    }
}
