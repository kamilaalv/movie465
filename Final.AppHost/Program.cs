var builder = DistributedApplication.CreateBuilder(args);


builder.AddProject<Projects.API_Users>("api-users");

builder.AddProject<Projects.API_Gateway>("api-gateway");
builder.AddProject<Projects.API_Movies>("api-movies");

builder.Build().Run();
