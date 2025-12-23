namespace FAP.Share.Dtos
{
    public class IdentityOperationResultDto
    {
        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    public class TokenResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

}
