using ControleDeBar.Aplicacao.Modulos.ModuloConta;
using ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;
using ControleDeBar.Aplicacao.Modulos.ModuloGarcom;
using ControleDeBar.Aplicacao.Modulos.ModuloMesa;
using ControleDeBar.Aplicacao.Modulos.ModuloProduto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ControleDeBar.Aplicacao;

public static class InjecaoDependencia
{
    public static void AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<ServicoConta>();
        services.AddScoped<ServicoProduto>();
        services.AddScoped<ServicoMesa>();
        services.AddScoped<ServicoGarcom>();
        services.AddScoped<ServicoFaturamento>();
    }
}
