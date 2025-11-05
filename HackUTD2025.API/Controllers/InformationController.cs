using HackUTD2025.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace HackUTD2025.API.Controllers;

public class InformationController : Controller
{
    private readonly ILogger<InformationController> _logger;
    private readonly NetworkDto _networkDto;
    private readonly MarketDto _marketDto;
    private readonly IEnumerable<CourierDto> _couriers;
    private readonly IEnumerable<CauldronDto> _cauldrons;
    private readonly DirectedGraph _graph;

    public InformationController(ILogger<InformationController> logger, 
        NetworkDto network,
        MarketDto market,
        IEnumerable<CourierDto> couriers,
        IEnumerable<CauldronDto> cauldrons,
         DirectedGraph graph)
    {
        _logger = logger;
        _networkDto = network;
        _marketDto = market;
        _couriers = couriers;
        _cauldrons = cauldrons;
        _graph = graph;
    }
    
    [Route("network")]
    [HttpGet]
    public NetworkDto GetNetwork() => _networkDto;
    
    [Route("market")]
    [HttpGet]
    public MarketDto GetMarket() => _marketDto;

    [Route("couriers")]
    [HttpGet]
    public IEnumerable<CourierDto> GetCouriers() => _couriers;
    
    [Route("cauldrons")]
    [HttpGet]
    public IEnumerable<CauldronDto> GetCauldrons() => _cauldrons;

    [Route("graph/neighbors/{nodeId}")]
    [HttpGet]
    public IEnumerable<NeighborDto> GetNeighbors(string nodeId) => _graph.Neighbors(nodeId);
}