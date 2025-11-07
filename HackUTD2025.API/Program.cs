using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;

using HackUTD2025.API.Dtos;
using HackUTD2025.API.Utilities;

using Microsoft.AspNetCore.ResponseCompression;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
    .WriteTo.Async(a => a.Console())
    .Enrich.FromLogContext());

Stopwatch sp = Stopwatch.StartNew();

var dataTask = ReadHistoricalDataAsync();
var metadataTask = ReadMetadataAsync();
var dtosTask = ReadDataAsync();
var ticketsTask = ReadTicketDataAsync();

await Task.WhenAll(dataTask, metadataTask, dtosTask, ticketsTask);

sp.Stop();
Console.WriteLine("Data loaded in {0}.", sp.Elapsed);

var data = dataTask.Result;
var metadata = metadataTask.Result;
var dtos = dtosTask.Result;
var tickets = ticketsTask.Result;


var graph = new NodeGraph([.. dtos.cauldrons, dtos.enchanted_market], dtos.network.edges);

builder.Services.AddSingleton(graph);
builder.Services.AddSingleton(tickets);

builder.Services.AddSingleton(dtos.network);
builder.Services.AddSingleton(dtos.enchanted_market);
builder.Services.AddSingleton<IEnumerable<CourierDto>>(dtos.couriers);
builder.Services.AddSingleton<IEnumerable<CauldronDto>>(dtos.cauldrons);

builder.Services.AddSingleton(metadata);
builder.Services.AddSingleton(data);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddResponseCompression(opts => {
    opts.EnableForHttps = true; // compress over HTTPS
    opts.Providers.Add<BrotliCompressionProvider>();
    opts.Providers.Add<GzipCompressionProvider>();

    // Include common JSON/text types; static files inherit too if middleware order allows.
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat([
        "application/json",
        "text/html"
    ]);
});
builder.Services.Configure<BrotliCompressionProviderOptions>(o => {
    o.Level = CompressionLevel.Optimal; // fastest small payloads: Fastest
});
builder.Services.Configure<GzipCompressionProviderOptions>(o => {
    o.Level = CompressionLevel.Fastest;
});


builder.Logging.AddSerilog();

var app = builder.Build();

app.UseResponseCompression();

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HackUTD2025.API V1");
});
app.MapOpenApi();

app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = { "index.html" }
});
app.UseStaticFiles();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();

async Task<IEnumerable<HistoricalDataDto>> ReadHistoricalDataAsync()
{
    using StreamReader reader = new("Data/testhistorical_data 2.json");

    var data = await JsonSerializer.DeserializeAsync<IEnumerable<HistoricalDataDto>?>(reader.BaseStream);

    return data ?? throw new Exception("Where's our data???");
}

async Task<HistoricalDataMetadataDto> ReadMetadataAsync()
{
    using StreamReader reader = new("Data/testhistorical_metadata 2.json");

    var data = await JsonSerializer.DeserializeAsync<HistoricalDataMetadataDto?>(reader.BaseStream);

    return data ?? throw new Exception("Where's our history metadata???");
}

async Task<RootDto> ReadDataAsync()
{
    using StreamReader reader = new("Data/testcauldrons 1.json");

    var data = await JsonSerializer.DeserializeAsync<RootDto>(reader.BaseStream);

    return data ?? throw new Exception("Where's our cauldron data???");
}

async Task<TicketsDto> ReadTicketDataAsync()
{
    using StreamReader reader = new("Data/testtransport_tickets 2.json");

    var data = await JsonSerializer.DeserializeAsync<TicketsDto>(reader.BaseStream);

    return data ?? throw new Exception("Where's our ticket data???");
}

