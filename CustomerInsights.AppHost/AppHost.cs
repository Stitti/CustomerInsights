IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<RedisResource> cache = builder.AddRedis("customer-insights-cache")
                                               .WithRedisInsight()
                                               .WithRedisCommander();

IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("customer-insights-postgres")
                                                           //.WithDataVolume()
                                                           .WithPgAdmin()
                                                           .WithDbGate();

IResourceBuilder<PostgresDatabaseResource> customerVoiceDb = postgres.AddDatabase("customer-insights-db");

IResourceBuilder<ProjectResource> customerVoiceNlpService = builder.AddProject<Projects.CustomerInsights_NlpService>("customer-insights-nlp-service");

IResourceBuilder<ProjectResource> inferenceWorker = builder.AddProject<Projects.CustomerInsights_InferenceWorker>("customer-insights-inference-worker")
                                                           .WaitFor(customerVoiceDb)
                                                           .WaitFor(customerVoiceNlpService)
                                                           .WaitFor(cache)
                                                           .WithReference(customerVoiceDb)
                                                           .WithReference(customerVoiceNlpService)
                                                           .WithReference(cache);

IResourceBuilder<ProjectResource> signalWorker = builder.AddProject<Projects.CustomerInsights_SignalWorker>("customer-insights-signal-worker")
                                                        .WaitFor(customerVoiceDb)
                                                        .WaitFor(cache)
                                                        .WithReference(customerVoiceDb)
                                                        .WithReference(cache);

IResourceBuilder<ProjectResource> customerVoiceDocumentService = builder.AddProject<Projects.CustomerInsights_DocumentService>("customer-insights-document-service");

IResourceBuilder<ProjectResource> customerVoiceApi = builder.AddProject<Projects.CustomerInsights_ApiService>("customer-insights-api-service")
                                                            .WaitFor(customerVoiceDb)
                                                            .WaitFor(customerVoiceNlpService)
                                                            .WaitFor(cache)
                                                            .WithReference(customerVoiceDb)
                                                            .WithReference(customerVoiceNlpService)
                                                            .WaitFor(cache);
                                                            
builder.AddViteApp("customer-insights-client", "../CustomerInsights.Client")
       .WithNpmPackageInstallation()
       .WaitFor(customerVoiceApi)
       .WaitFor(customerVoiceDocumentService)
       .WithReference(customerVoiceApi)
       .WithReference(customerVoiceDocumentService)
       .WithEnvironment("VITE_CUSTOMER_VOICE_API", customerVoiceApi.GetEndpoint("http"))
       .WithEnvironment("VITE_CUSTOMER_VOICE_DOCUMENT_API", customerVoiceDocumentService.GetEndpoint("http"));

builder.Build().Run();
