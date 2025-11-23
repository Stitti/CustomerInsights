IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ParameterResource> rabbitUser = builder.AddParameter("rabbitUser", "guest", secret: true);
IResourceBuilder<ParameterResource> rabbitPassword = builder.AddParameter("rabbitPassword", "guest", secret: true);

IResourceBuilder<RedisResource> cache = builder.AddRedis("customerinsights-cache")
                                               .WithRedisInsight()
                                               .WithRedisCommander()
                                               .WithDbGate();

IResourceBuilder <RabbitMQServerResource> rabbitmq = builder.AddRabbitMQ("messaging", rabbitUser, rabbitPassword)
                                                            .WithManagementPlugin()
                                                            .WithDataVolume();

IResourceBuilder<ContainerResource> presidioAnalyzer = builder.AddContainer("presidio-analyzer", "mcr.microsoft.com/presidio-analyzer", "latest")
                                                              .WithHttpEndpoint(port: 5001, targetPort: 3000);

IResourceBuilder<ContainerResource> presidioAnonymizer = builder.AddContainer("presidio-anonymizer", "mcr.microsoft.com/presidio-anonymizer", "latest")
                                                                .WithHttpEndpoint(port: 5002, targetPort: 3000);

IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("customer-insights-postgres")
                                                           .WithPgAdmin()
                                                           .WithDbGate();

IResourceBuilder<PostgresDatabaseResource> customerInsightsDb = postgres.AddDatabase("customer-insights-db");


//IResourceBuilder<ContainerResource> ollama = builder.AddContainer("ollama", "ollama/ollama", "latest")
//                                                    .WithHttpEndpoint(port: 11434, targetPort: 11434);

//IResourceBuilder<ProjectResource> embeddingService = builder.AddProject<Projects.CustomerInsights_EmbeddingService>("customerinsights-embeddingservice")
//                                                            .WithEnvironment("EmbeddingService__OnnxModelPath", "Models/model.onnx")
//                                                            .WaitFor(customerInsightsDb)
//                                                            .WithReference(customerInsightsDb);

//IResourceBuilder<ProjectResource> ragService = builder.AddProject<Projects.CustomerInsights_RagService>("customerinsights-ragservice")
//                                                      .WaitFor(customerInsightsDb)
//                                                      .WaitFor(ollama)
//                                                      .WithReference(customerInsightsDb)
//                                                      .WithEnvironment("ChatService__BaseUrl", ollama.GetEndpoint("http"));

IResourceBuilder<ProjectResource> customerInsightsDocumentService = builder.AddProject<Projects.CustomerInsights_DocumentService>("customerinsights-document-service");

IResourceBuilder<ProjectResource> customerInsightsApi = builder.AddProject<Projects.CustomerInsights_ApiService>("customerinsights-apiservice")
                                                            .WaitFor(customerInsightsDb)
                                                            .WaitFor(cache)
                                                            .WaitFor(rabbitmq)
                                                            .WithReference(customerInsightsDb)
                                                            .WithReference(cache)
                                                            .WithReference(rabbitmq);

builder.AddViteApp("customerinsights-client", "../CustomerInsights.Client")
       .WithNpmPackageInstallation()
       .WaitFor(customerInsightsApi)
       .WaitFor(customerInsightsDocumentService)
       .WithReference(customerInsightsApi)
       .WithReference(customerInsightsDocumentService)
       .WithEnvironment("VITE_CUSTOMER_VOICE_API", customerInsightsApi.GetEndpoint("http"))
       .WithEnvironment("VITE_CUSTOMER_VOICE_DOCUMENT_API", customerInsightsDocumentService.GetEndpoint("http"));

builder.AddViteApp("customerinsights-documentation", "../CustomerInsights.Documentation")
       .WithNpmPackageInstallation();

//builder.AddProject<Projects.CustomerInsights_EmailService>("customerinsights-emailservice")
//       .WaitFor(rabbitmq)
//       .WaitFor(customerInsightsDb)
//       .WithReference(rabbitmq)
//       .WithReference(customerInsightsDb);

builder.AddProject<Projects.CustomerInsights_NlpService>("customerinsights-nlpservice")
       .WaitFor(rabbitmq)
       .WaitFor(customerInsightsDb)
       .WaitFor(presidioAnalyzer)
       .WaitFor(presidioAnonymizer)
       .WithReference(rabbitmq)
       .WithReference(customerInsightsDb)
       .WithEnvironment("Presidio__AnalyzerUrl", presidioAnalyzer.GetEndpoint("http"))
       .WithEnvironment("Presidio__AnonymizerUrl", presidioAnonymizer.GetEndpoint("http"));

builder.AddProject<Projects.CustomerInsights_SatisfactionIndexService>("customerinsights-satisfactionindexservice")
       .WaitFor(rabbitmq)
       .WaitFor(customerInsightsDb)
       .WithReference(rabbitmq)
       .WithReference(customerInsightsDb);

builder.Build().Run();