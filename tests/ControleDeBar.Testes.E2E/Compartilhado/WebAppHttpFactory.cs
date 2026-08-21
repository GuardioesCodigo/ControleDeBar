using ControleDeBar.Infra.Compartilhado.Orm;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ControleDeBar.Testes.E2E.Compartilhado;

// Fabrica leve para testes de integracao HTTP (sem navegador real via Playwright):
// usa TestServer em processo, muito mais rapida que subir Kestrel + Chromium.
// Usada para os casos de Autenticacao que dependem do ASP.NET Core Identity real
// mas nao precisam de interacao visual com a pagina.
public sealed class WebAppHttpFactory : WebApplicationFactory<Program>
{
    private readonly string nomeBanco = $"http-{Guid.NewGuid():N}";

    public WebAppHttpFactory()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("Infra__NewRelic__Enabled", "false");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ControleDeBarDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ControleDeBarDbContext>>();

            services.AddDbContext<ControleDeBarDbContext>(options =>
            {
                options.UseInMemoryDatabase(nomeBanco);
            });
        });
    }

    // Cliente que NÃO segue redirecionamentos automaticamente: assim conseguimos
    // verificar o status 302 (sucesso, com redirect) vs 200 (formulário reexibido
    // com erros de validação/autenticação) sem precisar de um navegador real.
    public HttpClient CriarClienteSemRedirecionamento()
    {
        return CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }
}
