using HackUTD2025.API.Dtos;
using HackUTD2025.API.Utilities;

using Microsoft.AspNetCore.Mvc;

namespace HackUTD2025.API.Controllers;

[Route("api/[controller]")]
public class InformationController : ControllerBase
{
    private readonly ILogger<InformationController> _logger;
    private readonly NetworkDto _networkDto;
    private readonly MarketDto _marketDto;
    private readonly IEnumerable<CourierDto> _couriers;
    private readonly IEnumerable<CauldronDto> _cauldrons;
    private readonly NodeGraph _graph;

    public InformationController(ILogger<InformationController> logger, 
        NetworkDto network,
        MarketDto market,
        IEnumerable<CourierDto> couriers,
        IEnumerable<CauldronDto> cauldrons,
         NodeGraph graph)
    {
        _logger = logger;
        _networkDto = network;
        _marketDto = market;
        _couriers = couriers;
        _cauldrons = cauldrons;
        _graph = graph;
    }
    
    /// <summary>
    /// Retrieves network information.
    /// </summary>
    /// <remarks>
    /// It takes some amount of time for a courier witch to travel from cauldron to caldron or from market to cauldron. <br/>
    /// This data ultimately creates a graph. <br/>
    /// This data is critical for calculating an optimal routing algorithm for the courier witches.
    /// </remarks>
    [Route("network")]
    [HttpGet]
    public NetworkDto GetNetwork() => _networkDto;
    
    /// <summary>
    /// Retrieves market information.
    /// </summary>
    /// <remarks>
    /// This returns the latitude and longitude and name of the market. <br/>
    /// There will only ever be one market, and the information is purely for visualization purposes.
    /// </remarks>
    [Route("market")]
    [HttpGet]
    public MarketDto GetMarket() => _marketDto;

    /// <summary>
    /// Retrieves courier witches information.
    /// </summary>
    /// <remarks>
    /// This shows each courier witch and the amount of brew then can haul to the market before their broom caldron is full. <br/>
    /// This information is important when determining the minimal amount of witches needed and their optimal route.
    /// </remarks>
    [Route("couriers")]
    [HttpGet]
    public IEnumerable<CourierDto> GetCouriers() => _couriers;
    
    /// <summary>
    /// Retrieves cauldron information.
    /// </summary>
    /// <remarks>
    /// This shows the amount of brew each cauldron can hold before it overflow. <br/>
    /// When planning how the courier witches should haul brew, it's important these cauldron don't overflow ever (also see /api/Data). <br/>
    /// The latitude and longitude can be used for visualization purposes.
    /// </remarks>
    [Route("cauldrons")]
    [HttpGet]
    public IEnumerable<CauldronDto> GetCauldrons() => _cauldrons;

    /// <summary>
    /// Retrieves undirected neighbors for a given node.
    /// </summary>
    /// <param name="nodeId"></param>
    /// <remarks>
    /// Using this may a bit more helpful than the directed neighbors. <br/>
    /// Routes between cauldrons are bidirectional.
    /// </remarks>
    [Route("graph/neighbors/{nodeId}")]
    [HttpGet]
    public IEnumerable<NeighborDto> GetUndirectedNeighbors(string nodeId) => _graph.UndirectedNeighbords(nodeId);

    [Route("graph/neighbors/directed/{nodeId}")]
    [HttpGet]
    public IEnumerable<NeighborDto> GetNeighbors(string nodeId) => _graph.Neighbors(nodeId);
}