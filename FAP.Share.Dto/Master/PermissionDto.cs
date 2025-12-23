namespace FAP.Share.Dtos
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string Module { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class AssignPermissionRequest
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> PermissionCodes { get; set; } = new();
        public string? CampusCode { get; set; }
    }
}

