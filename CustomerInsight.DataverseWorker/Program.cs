using CustomerInsight.DataverseWorker;
using CustomerInsight.DataverseWorker.Repositories;
using CustomerInsight.DataverseWorker.Services;
using CustomerInsights.DataverseWorker.Repositories;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<AccountRepository>();
builder.Services.AddSingleton<ContactRepository>();
builder.Services.AddSingleton<InteractionRepository>();
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<ContactService>();
builder.Services.AddSingleton<EmailService>();

IHost host = builder.Build();
host.Run();
