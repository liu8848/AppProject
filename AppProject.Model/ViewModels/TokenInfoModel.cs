namespace AppProject.Model.ViewModels;

public class TokenInfoModel
{
    public bool success { get; set; }
    public string token { get; set; }
    public double expires_in { get; set; }
    public string token_type { get; set; }
}