using System.ComponentModel.DataAnnotations;

namespace NetWaveSupportAPI.DTOs
{
    public class RefreshTokenDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}