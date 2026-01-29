namespace CRMUKMTPApi.Models;

public class ParamModel
{
    //int page= 1,int limit = 25 ,string loginid = "",string  sort = "-dealid" ,string filter = "comment" ,string filtervalue = "by" ,DateTime? from = null,DateTime? to = null,bool topdf = false ,bool tocsv = false
    public int Page { get; set; }
    public int Limit { get; set; }
    public string Loginid { get; set; } = string.Empty;
    public string Symbols { get; set; } = string.Empty;
    public string Sort { get; set; } = string.Empty;
    public string Filter { get; set; } = string.Empty;
    public string Filtervalue { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public bool ToPdf { get; set; }
    public bool ToCSV { get; set; }
}
