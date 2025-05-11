namespace APP.Users
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public double TokenExpirationInMinutes { get; set; }
        public int RefreshTokenExpirationInDays { get; set; }
    }
}