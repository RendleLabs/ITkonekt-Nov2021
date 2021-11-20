using System.Collections.Generic;
using System.Threading;
using GrpcTestHelper;
using Ingredients.Data;
using Ingredients.Protos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;

namespace Ingredients.Tests;

public class IngredientsApplicationFactory : WebApplicationFactory<Marker>
{
    public IngredientsService.IngredientsServiceClient CreateGrpcClient()
    {
        var channel = this.CreateGrpcChannel();
        return new IngredientsService.IngredientsServiceClient(channel);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IToppingData>();
            services.RemoveAll<ICrustData>();

            var toppingSub = Substitute.For<IToppingData>();

            var toppings = new List<ToppingEntity>
            {
                new ToppingEntity("cheese", "Cheese", 0.5d, 10),
                new ToppingEntity("tomato", "Tomato", 0.5d, 10)
            };

            toppingSub.GetAsync(Arg.Any<CancellationToken>())
                .Returns(toppings);

            services.AddSingleton(toppingSub);

            var crustsSub = Substitute.For<ICrustData>();

            var crusts = new List<CrustEntity>
            {
                new CrustEntity("thin", "Thin", 9, 5d, 10),
            };

            crustsSub.GetAsync(Arg.Any<CancellationToken>())
                .Returns(crusts);

            services.AddSingleton(crustsSub);
        });
        base.ConfigureWebHost(builder);
    }
}