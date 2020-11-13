namespace Xu.Model.ViewModel
{
    public class TokenInfoViewModel
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public double Expires_in { get; set; }
        public string Token_type { get; set; }
    }
}