namespace NetWaveSupportAPI.Models
{
   
        public class Customer
        {
            public int Id { get; set; }

            public string FullName { get; set; }

            public string Email { get; set; }

            public string PasswordHash { get; set; }

            public string Role { get; set; }

            public string? RefreshTokenHash { get; set; }

            public DateTime? RefreshTokenExpiryTime { get; set; }

            public bool IsRefreshTokenRevoked { get; set; }
        }
    
}
