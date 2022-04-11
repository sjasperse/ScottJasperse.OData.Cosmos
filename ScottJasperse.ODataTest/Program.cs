using Microsoft.Azure.Cosmos;
using YuKitsune.Configuration.Env;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddEnvFile(".env");

builder.Services
    .AddTransient<CosmosClient>(p => new CosmosClient(p.GetRequiredService<IConfiguration>().GetConnectionString("Cosmos")))
    .AddControllers()
    .AddOData(opt => {
        opt.Select().Filter().OrderBy().SetMaxTop(1000);
    });

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseRouting();
app.UseEndpoints(x => {
    x.MapControllers();
});

app.Run();

public partial class Program { }
