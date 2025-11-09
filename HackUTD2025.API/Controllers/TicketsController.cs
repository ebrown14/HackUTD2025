using HackUTD2025.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace HackUTD2025.API.Controllers;

[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ILogger<TicketsController> _logger;
    private readonly TicketsDto _ticketsDto;

    public TicketsController(ILogger<TicketsController> logger, 
        TicketsDto tickets)
    {
        _logger = logger;
        _ticketsDto = tickets;
    }
    
    /// <summary>
    /// Retrieves all tickets.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Every time a courier witch buys brew and hauls it away, she creates an invoice ticket. <br/>
    /// It will contain the amount purchased, the date, the courier witch's unique id, the cauldron she took it from, and the ticket id. <br/>
    /// Please make sure these ticket are honest and complete because some witches are known to be evil. <br/>
    /// See (<a href="https://hackutd2025.eog.systems/api/Data">/api/Data</a>) to ensure these tickets are honest.
    /// </remarks>
    [HttpGet]
    public TicketsDto GetTickets() => _ticketsDto;
    
}