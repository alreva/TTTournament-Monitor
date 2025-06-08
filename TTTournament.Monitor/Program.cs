using TTTournament.Monitor;
using TTTournament.Monitor.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IPersistenceService, JsonPersistenceService>();
builder.Services.AddSingleton<INewsScraper, KoztsNewsScraper>();
builder.Services.AddSingleton<IOcrService, TesseractOcrService>();
builder.Services.AddSingleton<IOpenAiService, OpenAiService>();
builder.Services.AddSingleton<IGmailService, GmailDraftService>();
builder.Services.AddSingleton<ICalendarService, GoogleCalendarService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
