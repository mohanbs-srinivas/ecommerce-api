namespace ecommerce_api.Models.ViewModels
{
    public class RegistrationResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
        public string? UserId { get; set; }
        public bool RequiresEmailVerification { get; set; }
        public bool RequiresApproval { get; set; }
    }
}