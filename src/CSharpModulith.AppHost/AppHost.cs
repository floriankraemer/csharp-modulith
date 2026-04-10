var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CSharpModulith_Host>("host")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

builder.Build().Run();
