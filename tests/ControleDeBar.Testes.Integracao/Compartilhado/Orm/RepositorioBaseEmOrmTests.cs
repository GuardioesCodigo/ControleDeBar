using ControleDeBar.Infra.Compartilhado.Orm;
using ControleDeBar.Infra.Modulos.ModuloConta;
using ControleDeBar.Infra.Modulos.ModuloEstabelecimento;
using ControleDeBar.Infra.Modulos.ModuloGarcom;
using ControleDeBar.Infra.Modulos.ModuloMesa;
using ControleDeBar.Infra.Modulos.ModuloPedido;
using ControleDeBar.Infra.Modulos.ModuloProduto;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Testes.Integracao.Compartilhado.Orm;

public abstract class RepositorioBaseEmOrmTests
{
    protected ControleDeBarDbContext dbContext = null!;
    protected ProvedorDeUsuarioFake provedorDeUsuario = null!;

    protected RepositorioEstabelecimentoEmOrm repositorioEstabelecimento = null!;
    protected RepositorioMesaEmOrm repositorioMesa = null!;
    protected RepositorioGarcomEmOrm repositorioGarcom = null!;
    protected RepositorioProdutoEmOrm repositorioProduto = null!;
    protected RepositorioContaEmOrm repositorioConta = null!;
    protected RepositorioPedidoEmOrm repositorioPedido = null!;

    private string nomeBanco = string.Empty;

    [TestInitialize]
    public void InicializarContexto()
    {
        nomeBanco = $"integracao-{Guid.NewGuid():N}";
        provedorDeUsuario = new ProvedorDeUsuarioFake { Id = Guid.CreateVersion7() };

        dbContext = CriarDbContext();

        repositorioEstabelecimento = new RepositorioEstabelecimentoEmOrm(dbContext);
        repositorioMesa = new RepositorioMesaEmOrm(dbContext);
        repositorioGarcom = new RepositorioGarcomEmOrm(dbContext);
        repositorioProduto = new RepositorioProdutoEmOrm(dbContext);
        repositorioConta = new RepositorioContaEmOrm(dbContext);
        repositorioPedido = new RepositorioPedidoEmOrm(dbContext);
    }

    [TestCleanup]
    public void DescartarContexto()
    {
        dbContext.Dispose();
    }

    // Troca o "usuário autenticado" simulado mantendo o mesmo banco em memória,
    // para testar cenários de isolamento entre dois estabelecimentos distintos.
    protected ControleDeBarDbContext CriarContextoParaOutroEstabelecimento(out Guid novoUserId)
    {
        novoUserId = Guid.CreateVersion7();

        ProvedorDeUsuarioFake outroProvedor = new() { Id = novoUserId };

        DbContextOptions<ControleDeBarDbContext> options =
            new DbContextOptionsBuilder<ControleDeBarDbContext>()
                .UseInMemoryDatabase(nomeBanco)
                .Options;

        return new ControleDeBarDbContext(options, outroProvedor);
    }

    private ControleDeBarDbContext CriarDbContext()
    {
        DbContextOptions<ControleDeBarDbContext> options =
            new DbContextOptionsBuilder<ControleDeBarDbContext>()
                .UseInMemoryDatabase(nomeBanco)
                .Options;

        return new ControleDeBarDbContext(options, provedorDeUsuario);
    }
}
