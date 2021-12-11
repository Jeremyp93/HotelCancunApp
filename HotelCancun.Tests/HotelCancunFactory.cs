using HotelCancun.Entities.DbContexts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoelCancun.Tests;
/// <summary>
/// Class to mock the API and replace the SQL database connection with inmemory database
/// </summary>
class HotelCancunFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var root = new InMemoryDatabaseRoot();

        builder.ConfigureServices(services =>
        {
            services.AddScoped(sp =>
            {
                // Replace SQL with the in memory provider for tests
                return new DbContextOptionsBuilder<HotelCancunContext>()
                            .UseInMemoryDatabase("Tests", root)
                            .UseApplicationServiceProvider(sp)
                            .Options;
            });
        });

        return base.CreateHost(builder);
    }
}