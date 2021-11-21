using Grpc.Net.Client.Configuration;
using Ingredients.Protos;
using JaegerTracing;
using Orders.Protos;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.AddJaegerTracing();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddGrpcClient<IngredientsService.IngredientsServiceClient>((provider, options) =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var uri = configuration.GetServiceUri("Ingredients");
        options.Address = uri ?? new Uri("https://localhost:5003");
    });

builder.Services.AddGrpcClient<OrderService.OrderServiceClient>((provider, options) =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var uri = configuration.GetServiceUri("Orders");
    options.Address = uri ?? new Uri("https://localhost:5005");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();