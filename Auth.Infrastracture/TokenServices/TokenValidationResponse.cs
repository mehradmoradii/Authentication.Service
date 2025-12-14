namespace Auth.Infrastracture.TokenServices
{
    public class TokenValidationResponse
    {
        public bool IsValid { get; set; }
        public string? Error { get; set; }
        public string? UserId { get; set; }
        public DateTime Expiry { get; set; }
    }
}
