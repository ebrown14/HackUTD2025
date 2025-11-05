using HackUTD2025.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace HackUTD2025.API.Controllers;

public class TicketsController : Controller
{
    private readonly ILogger<TicketsController> _logger;
    private readonly TicketsDto _ticketsDto;

    public TicketsController(ILogger<TicketsController> logger, 
        TicketsDto tickets)
    {
        _logger = logger;
        _ticketsDto = tickets;
    }
    
    [HttpGet]
    public TicketsDto GetTickets() => _ticketsDto;
    
}