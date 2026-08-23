using ControleDeBar.Dominio.Modulos.ModuloProduto;
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

        // O Program.cs lê essas configurações durante a criação da aplicação.
        // Por isso, elas precisam existir antes do host ser iniciado.
        Environment.SetEnvironmentVariable(
            "ASPNETCORE_ENVIRONMENT",
            "Testing");

        Environment.SetEnvironmentVariable(
            "Infra__NewRelic__Enabled",
            "false");

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
            services.RemoveAll<DbContextOptions<ControleDeBarDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ControleDeBarDbContext>>();

            // Cada execução do E2E recebe seu próprio banco InMemory.
            services.AddDbContext<ControleDeBarDbContext>(options =>
            {
                options.UseInMemoryDatabase(nomeBanco);
            });

            // Cria um escopo para acessar o DbContext.
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<ControleDeBarDbContext>();

            // Cria o banco.
            db.Database.EnsureCreated();

            // Popula os dados necessários para os testes E2E.
            SeedDatabase(db);
        });
    }

    private static void SeedDatabase(ControleDeBarDbContext db)
    {
        // Evita inserir os mesmos dados mais de uma vez.
        if (db.Produtos.Any())
            return;

        var produtos = new[]
        {
            new Produto
            {
                Nome = "Cerveja",
                Preco = 10.00m
            },

            new Produto
            {
                Nome = "Refrigerante",
                Preco = 8.00m
            },

            new Produto
            {
                Nome = "Batata Frita",
                Preco = 15.00m
            }
        };

        db.Produtos.AddRange(produtos);

        db.SaveChanges();
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