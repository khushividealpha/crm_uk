namespace CRMUKMTPApi.Models
{
    public class SignUpMTModel
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Password { get; set; }
        public string? Group { get; set; }

    }
    public class SignupMt5UserResult
    {
        public bool status { get; set; }
        public string email { get; set; } = string.Empty;
        public string mt5Id { get; set; } = string.Empty;
    }
}
