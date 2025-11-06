using CustomerInsights.GoogleFormsWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<FormsIngestWorker>();

var host = builder.Build();
host.Run();
