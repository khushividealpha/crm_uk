

namespace CRMUKMTPApi.Models;

public class ApiResponse
{
    public object data { get; set; }
    public int page { get; set; }
    public int pageSize { get; set; }
    public int result { get; set; }
    public string status { get; set; }
    public int totalPages { get; set; }
    public int totalRecords { get; set; }
}
