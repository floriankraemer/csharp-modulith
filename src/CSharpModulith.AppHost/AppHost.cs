var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");
var rabbitmq = builder.AddRabbitMQ("messaging");
var sqlite = builder.AddSqlite("sqlite");
var mailpit = builder.AddMailPit("mailpit");

builder.AddProject<Projects.CSharpModulith_Host>("host")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WaitFor(postgresdb)
    .WaitFor(rabbitmq)
    .WaitFor(mailpit)
    .WithReference(postgresdb)
    .WithReference(rabbitmq)
    .WithReference(sqlite)
    .WithReference(mailpit);

builder.Build().Run();
