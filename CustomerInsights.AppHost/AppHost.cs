IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<RedisResource> cache = builder.AddRedis("customer-insights-cache")
                                               .WithRedisInsight()
                                               .WithRedisCommander();

IResourceBuilder<RabbitMQServerResource> rabbitmq = builder.AddRabbitMQ("messaging");

IResourceBuilder<ContainerResource> presidioAnalyzer = builder.AddContainer("presidio-analyzer", "mcr.microsoft.com/presidio-analyzer", "latest")
                                                              .WithHttpEndpoint(port: 5001, targetPort: 3000);

IResourceBuilder<ContainerResource> presidioAnonymizer = builder.AddContainer("presidio-anonymizer", "mcr.microsoft.com/presidio-anonymizer", "latest")
                                                                .WithHttpEndpoint(port: 5002, targetPort: 3000);

IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("customer-insights-postgres")
                                                           //.WithDataVolume()
                                                           .WithPgAdmin()
                                                           .WithDbGate();

IResourceBuilder<PostgresDatabaseResource> customerVoiceDb = postgres.AddDatabase("customer-insights-db");

IResourceBuilder<ProjectResource> customerVoiceNlpService = builder.AddProject<Projects.CustomerInsights_NlpService>("customer-insights-nlp-service");

/*
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
*/

IResourceBuilder<ProjectResource> customerVoiceDocumentService = builder.AddProject<Projects.CustomerInsights_DocumentService>("customer-insights-document-service");

IResourceBuilder<ProjectResource> customerVoiceApi = builder.AddProject<Projects.CustomerInsights_ApiService>("customer-insights-api-service")
                                                            .WaitFor(customerVoiceDb)
                                                            .WaitFor(customerVoiceNlpService)
                                                            .WaitFor(cache)
                                                            .WithReference(customerVoiceDb)
                                                            .WithReference(customerVoiceNlpService)
                                                            .WithReference(cache);



IResourceBuilder<ContainerResource> ollama = builder.AddContainer("ollama", "ollama/ollama", "latest")
                                                    .WithHttpEndpoint(port: 11434, targetPort: 11434);

//IResourceBuilder<ProjectResource> embeddingService = builder.AddProject<Projects.CustomerInsights_EmbeddingService>("customer-insights-embedding-service")
//                                                            .WithEnvironment("EmbeddingService__OnnxModelPath", "Models/model.onnx");

//IResourceBuilder<ProjectResource> ragService = builder.AddProject<Projects.CustomerInsights_RagService>("customer-insights-rag-service")
//                                                      .WaitFor(customerVoiceDb)
//                                                      .WaitFor(embeddingService)
//                                                      .WaitFor(ollama)
//                                                      .WithReference(customerVoiceDb)
//                                                      .WithEnvironment("EmbeddingService__BaseUrl", embeddingService.GetEndpoint("http"))
//                                                      .WithEnvironment("ChatService__BaseUrl", ollama.GetEndpoint("http"));

builder.AddViteApp("customer-insights-client", "../CustomerInsights.Client")
       .WithNpmPackageInstallation()
       .WaitFor(customerVoiceApi)
       .WaitFor(customerVoiceDocumentService)
       .WithReference(customerVoiceApi)
       .WithReference(customerVoiceDocumentService)
       .WithEnvironment("VITE_CUSTOMER_VOICE_API", customerVoiceApi.GetEndpoint("http"))
       .WithEnvironment("VITE_CUSTOMER_VOICE_DOCUMENT_API", customerVoiceDocumentService.GetEndpoint("http"));

builder.Build().Run();