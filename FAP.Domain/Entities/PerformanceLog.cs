using FAP.Common.Domain.Base;


namespace FAP.Common.Domain.Entities
{
    public class PerformanceLog:BaseEntity
    {
        public long Id { get; set; }
        public Guid TraceId { get; set; }
        public string UserName { get; set; }
        public string CampusCode { get; set; }
        public string FeatureName { get; set; }
        public int ElapsedMilliseconds { get; set; }
        public DateTime RequestTime { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public int StatusCode { get; set; }
        public string Host { get; set; }
    }
}
