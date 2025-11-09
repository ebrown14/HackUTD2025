using HackUTD2025.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace HackUTD2025.API.Controllers;

[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly ILogger<DataController> _logger;
    private readonly IEnumerable<HistoricalDataDto> _historicalDataDtos;
    private readonly HistoricalDataMetadataDto _historicalDataMetadataDto;

    public DataController(ILogger<DataController> logger, 
        IEnumerable<HistoricalDataDto> data,
        HistoricalDataMetadataDto metadata)
    {
        _logger = logger;
        _historicalDataDtos = data;
        _historicalDataMetadataDto = metadata;
    }

    /// <summary>
    /// Retrieves historical data within the specified date range.
    /// </summary>
    /// <param name="start">The start date as a Unix timestamp (optional).</param>
    /// <param name="end">The end date as a Unix timestamp (optional).</param>
    /// <returns>A collection of HistoricalDataDto objects within the specified date range.</returns>
    /// <remarks>
    /// Gives each cauldron level over time.<br/>
    /// The trend line will go up as the liquid is brewed.<br/>
    /// It will go down when it is hauled away by a courier witch.<br/>
    /// When a courier witch hauls it away, she SHOULD create a ticket at the end of the day (see <a href="https://hackutd2025.eog.systems/api/Tickets">/api/Tickets</a>) with how much she brew she bought, but sometimes witches aren't the most honest people.<br/>
    /// Example, use <a href="https://hackutd2025.eog.systems/api/Data">https://hackutd2025.eog.systems/api/Data</a> to get all data. <br/>
    /// Use <a href="https://hackutd2025.eog.systems/api/Data/?start_date=1762478048&amp;end_date=1762482227">https://hackutd2025.eog.systems/api/Data?start_date=1762478048&amp;end_date=1762482227</a> for a subset between the given time range.
    /// </remarks>
    [HttpGet]
    public IEnumerable<HistoricalDataDto> Get([FromQuery(Name = "start_date")] long? start, [FromQuery(Name = "end_date")] long? end)
    {
        DateTimeOffset? startTime = null;
        if (start.HasValue)
            startTime = DateTimeOffset.FromUnixTimeSeconds(start.Value);

        DateTimeOffset? endTime = null;
        if (end.HasValue)
            endTime = DateTimeOffset.FromUnixTimeSeconds(end.Value);

        var data = _historicalDataDtos;
        
        if (startTime is not null)
            data = data.Where(dto => dto.timestamp >= startTime);
        
        if (endTime is not null)
            data = data.Where(dto => dto.timestamp <= endTime);

        data.TryGetNonEnumeratedCount(out var count);
        
        return data;
    }
    
    /// <summary>
    /// Retrieves metadata about the historical data.
    /// </summary>
    [Route("metadata")]
    [HttpGet]
    public HistoricalDataMetadataDto GetMetadata() => _historicalDataMetadataDto;
}