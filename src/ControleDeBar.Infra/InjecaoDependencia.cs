using ControleDeBar.Infra.Compartilhado.Logging;
using ControleDeBar.Infra.Compartilhado.Orm;
using ControleDeBar.Infra.Compartilhado.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ControleDeBar.Infra;

public static class InjecaoDependencia
{
    public static void AddInfraRepositories(
        this IServiceCollection services,
        IConfiguration configuration,
        ILoggingBuilder logging
    )
    {
        services.AddSerilogLogger(configuration, logging);

        services.AddDbContext<ControleDeBarDbContext>(options =>
       {
           string? connectionString = configuration.GetConnectionString("SqlServerEF");

           if (string.IsNullOrWhiteSpace(connectionString))
           {
               throw new InvalidOperationException(
                   $"A connection string \"SqlServerEF\" não foi encontrada."
               );
           }

           options.UseSqlServer(connectionString, opt =>
           {
               opt.EnableRetryOnFailure(3);
           });
       });
    }
}
