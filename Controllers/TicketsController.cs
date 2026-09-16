using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetWaveSupportAPI.Data;
using System.Security.Claims;
using NetWaveSupportAPI.DTOs;
using NetWaveSupportAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace NetWaveSupportAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController : ControllerBase
    {



        private readonly AppDbContext _context;

        private readonly ILogger<TicketsController> _logger;

        public TicketsController(AppDbContext context, ILogger<TicketsController> logger)
        {
            _context = context;
            _logger = logger;

        }



        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketDto dto)
        {
            int customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            SupportTicket ticket = new SupportTicket
            {
                CustomerId = customerId,
                Title = dto.Title,
                Description = dto.Description,
                Status = "Open",
                CreatedAt = DateTime.UtcNow
            };

            _context.SupportTickets.Add(ticket);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTicketById),new { id = ticket.Id },ticket);
        }



        [HttpGet]
        public async Task<IActionResult> GetMyTickets()
        {
            int customerId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            var tickets = await _context.SupportTickets
                .Where(t => t.CustomerId == customerId)
                .ToListAsync();

            return Ok(tickets);
        }



        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _context.SupportTickets.ToListAsync();

            return Ok(tickets);
        }




        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            var ticket = await _context.SupportTickets
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return NotFound();

            int customerId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier));


            string role = User.FindFirstValue(ClaimTypes.Role);


            if (ticket.CustomerId != customerId && role != "Admin")
            {
                _logger.LogWarning(
                    "Forbidden access attempt by User {UserId} to Ticket {TicketId}",
                    customerId,
                    ticket.Id);

                return Forbid();
            }

            return Ok(ticket);
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, UpdateTicketDto dto)
        {
            var ticket = await _context.SupportTickets
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return NotFound();

            int customerId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            string role = User.FindFirstValue(ClaimTypes.Role);

            if (ticket.CustomerId != customerId && role != "Admin")
            {
                _logger.LogWarning(
                    "Forbidden access attempt by User {UserId} to Ticket {TicketId}",
                    customerId,
                    ticket.Id);

                return Forbid();
            }

            ticket.Title = dto.Title;
            ticket.Description = dto.Description;

            await _context.SaveChangesAsync();

            return Ok(ticket);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticket = await _context.SupportTickets
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return NotFound();

            int customerId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            string role = User.FindFirstValue(ClaimTypes.Role);


            if (ticket.CustomerId != customerId && role != "Admin")
            {
                _logger.LogWarning(
                    "Forbidden access attempt by User {UserId} to Ticket {TicketId}",
                    customerId,
                    ticket.Id);

                return Forbid();
            }

    

           _context.SupportTickets.Remove(ticket);
            await _context.SaveChangesAsync();



            _logger.LogInformation("Ticket {TicketId} deleted by User {UserId}",ticket.Id,
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value);


            return NoContent();
        }
    }
}