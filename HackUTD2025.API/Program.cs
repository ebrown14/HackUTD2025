using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;
using HackUTD2025.API.Dtos;
using HackUTD2025.API.Utilities;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
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
builder.Services.AddSwaggerGen(o => {
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HackUTD2025.API",
        Version = "v1",
        Description = "API for HackUTD 2025 - Cauldron Monitoring and Transport System",
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

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
builder.Services.Configure<GzipCompressionProviderOptions>(o => { o.Level = CompressionLevel.Fastest; });


builder.Logging.AddSerilog();

var app = builder.Build();

app.UseResponseCompression();

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HackUTD2025.API V1");
    c.DocumentTitle = "HackUTD2025 API - Connection Instructions";
    c.HeadContent = @"
        <div style='background: #f0f0f0; padding: 15px; margin-bottom: 20px; border-radius: 5px;'>
            <h3 style='margin-top: 0;'>🔗 How to Connect to the API</h3>
            <p><strong>Base URL:</strong> <code>https://hackutd2025.eog.systems</code></p>
            <p><strong>Main Endpoints:</strong></p>
            <ul>
                <li><code>GET https://hackutd2025.eog.systems/api/Data</code> - Historical cauldron data</li>
                <li><code>GET https://hackutd2025.eog.systems/api/Tickets</code> - Transport tickets</li>
                <li><code>GET https://hackutd2025.eog.systems/api/Information/cauldrons</code> - All cauldrons</li>
            </ul>
            <p><strong>Example:</strong> <code>https://hackutd2025.eog.systems/api/Data?start_date=0&end_date=2000000000</code></p>
        </div>
    ";
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