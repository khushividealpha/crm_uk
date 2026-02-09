namespace CRMUKMTPApi.Models
{
    public sealed class CreateClientRequest
    {
        public string? ClientType { get; set; }
        public string ClientStatus { get; set; } = string.Empty;
        public string EmploymentStatus { get; set; } = string.Empty;
        public string EmploymentIndus{ get; set; } = string.Empty;
        public string PersonEdu{ get; set; } = string.Empty;
        public string Gender{ get; set; } = string.Empty;
        public string SourceOfWealth{ get; set; } = string.Empty;
        public string PreferredCommunication{ get; set; } = string.Empty;
        public string ClientOrigin{ get; set; } = string.Empty;
        public string AssignedManager{ get; set; } = string.Empty;
        public string PersonCity{ get; set; } = string.Empty;
        public string PersonCountry { get; set; } = string.Empty;
        public string PersonState{ get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;

        public int AnnualIncome{ get; set; } = 0;
        public int NetWorth { get; set; }= 0;
        public int AnnaulDeposit { get; set; } = 0;

        public string? PersonName { get; set; }
        public string? PersonLastName { get; set; }
        public string? PersonMiddleName { get; set; }
        public string? PersonEmail { get; set; }
        public string? PersonPhone { get; set; }
        public string? KycStatus { get; set; }

        public string? PersonDocument { get; set; }
        public string? PersonDocumentExtra { get; set; }
        public DateTime? PersonDocumentDate { get; set; }
        public DateTime? PersonDocumentExpiration { get; set; }

        public DateTime? PersonBirthDate { get; set; }
        public double PersonAnnualIncome { get; set; }= 0;
        public string? PersonAddress { get; set; }
    }
    public class CreateClientResponse
    {
        public bool status { get; set; }
        public string? clientId { get; set; }
        public string? reason { get; set; }
    }


}
