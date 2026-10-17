using Agents_v._1.Interface;
using Agents_v._1.Service;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Your existing dependency injection registrations
// Register implementations for interfaces
builder.Services.AddScoped<IPromptBuilder, PromptBuilder>();
builder.Services.AddScoped<IResumeTextExtractor, ResumeTextExtractor>();
// Register AI model client implementation
builder.Services.AddTransient<IAiModelClient, ClineModelClient>();
// with IConfiguration available as builder.Configuration
//builder.Services.AddHttpClient<ClineModelClient>(client =>
//{
//    client.BaseAddress = new Uri("https://api.cline.bot/"); // set actual base URL
//    var apiKey = builder.Configuration["Cline:ApiKey"];
//    if (!string.IsNullOrEmpty(apiKey))
//    {
//        client.DefaultRequestHeaders.Authorization =
//            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
//    }
//});


var apiKey = builder.Configuration["OpenAI:ApiKey"];
if (string.IsNullOrWhiteSpace(apiKey))
    throw new InvalidOperationException("OpenAI API key not configured.");

builder.Services.AddHttpClient("openai", client =>
{
    client.BaseAddress = new Uri("https://api.openai.com");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
