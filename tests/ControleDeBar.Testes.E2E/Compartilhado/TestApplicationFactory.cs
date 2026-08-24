using ControleDeBar.Infra.Compartilhado.Orm;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ControleDeBar.Testes.E2E.Compartilhado;

public sealed class TestApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string nomeBanco;

    public string UrlBase { get; }

    public TestApplicationFactory()
    {
        nomeBanco = $"e2e-{Guid.NewGuid():N}";

        // Ambiente de testes.
        Environment.SetEnvironmentVariable(
            "ASPNETCORE_ENVIRONMENT",
            "Testing");

        // Desabilita o New Relic durante os testes.
        Environment.SetEnvironmentVariable(
            "Infra__NewRelic__Enabled",
            "false");

        // Kestrel em porta dinâmica.
        UseKestrel(0);

        StartServer();

        UrlBase = ObterUrlKestrel();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove a configuração original do DbContext.
            services.RemoveAll<
                DbContextOptions<ControleDeBarDbContext>>();

            services.RemoveAll<
                IDbContextOptionsConfiguration<ControleDeBarDbContext>>();

            // Banco exclusivo para esta execução da Factory.
            services.AddDbContext<ControleDeBarDbContext>(options =>
            {
                options.UseInMemoryDatabase(nomeBanco);
            });
        });
    }

    private string ObterUrlKestrel()
    {
        var servidor = Services.GetRequiredService<IServer>();

        var enderecos = servidor.Features
            .Get<IServerAddressesFeature>();

        if (enderecos is null)
        {
            throw new InvalidOperationException(
                "Não foi possível obter a URL do servidor.");
        }

        return enderecos.Addresses.Single();
    }
}