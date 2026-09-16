using System.ComponentModel.DataAnnotations;

namespace NetWaveSupportAPI.DTOs
{
    public class LogoutDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}