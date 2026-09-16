using System.ComponentModel.DataAnnotations;

namespace NetWaveSupportAPI.DTOs
{
    public class CreateTicketDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
    }
}