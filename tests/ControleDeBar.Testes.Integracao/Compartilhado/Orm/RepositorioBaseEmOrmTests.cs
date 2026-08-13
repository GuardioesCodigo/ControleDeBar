using ControleDeBar.Infra.Compartilhado.Orm;
using ControleDeBar.Infra.Modulos.ModuloCategoria;
using ControleDeBar.Infra.Modulos.ModuloCompromisso;
using ControleDeBar.Infra.Modulos.ModuloContato;
using ControleDeBar.Infra.Modulos.ModuloDespesa;
using ControleDeBar.Infra.Modulos.ModuloTarefa;
using Microsoft.EntityFrameworkCore;
using FizzWare.NBuilder;
using ControleDeBar.Dominio.Modulos.ModuloContato;
using ControleDeBar.Dominio.Modulos.ModuloCompromisso;
using ControleDeBar.Dominio.Modulos.ModuloCategoria;
using ControleDeBar.Dominio.Modulos.ModuloDespesa;
using ControleDeBar.Dominio.Modulos.ModuloTarefa;

namespace ControleDeBar.Testes.Integracao.Compartilhado.Orm;

public abstract class RepositorioBaseEmOrmTests
{
    protected ControleDeBarDbContext dbContext = null!;
    protected RepositorioContatoEmOrm repositorioContato = null!;
    protected RepositorioCompromissoEmOrm repositorioCompromisso = null!;
    protected RepositorioCategoriaEmOrm repositorioCategoria = null!;
    protected RepositorioDespesaEmOrm repositorioDespesa = null!;
    protected RepositorioTarefaEmOrm repositorioTarefa = null!;

    // Hooks / Ganchos
    [TestInitialize]
    public void InicializarContexto()
    {
        dbContext = CriarDbContext();

        // Contato
        repositorioContato = new RepositorioContatoEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Contato>(repositorioContato.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Contato>>((contatos) =>
        {
            foreach (Contato c in contatos)
                repositorioContato.Cadastrar(c);
        });

        // Compromisso
        repositorioCompromisso = new RepositorioCompromissoEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Compromisso>(repositorioCompromisso.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Compromisso>>((compromissos) =>
        {
            foreach (Compromisso c in compromissos)
                repositorioCompromisso.Cadastrar(c);
        });

        // Categoria
        repositorioCategoria = new RepositorioCategoriaEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Categoria>(repositorioCategoria.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Categoria>>((categorias) =>
        {
            foreach (Categoria c in categorias)
                repositorioCategoria.Cadastrar(c);
        });

        // Despesa
        repositorioDespesa = new RepositorioDespesaEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Despesa>(repositorioDespesa.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Despesa>>((despesas) =>
        {
            foreach (Despesa d in despesas)
                repositorioDespesa.Cadastrar(d);
        });

        // Tarefa
        repositorioTarefa = new RepositorioTarefaEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Tarefa>(repositorioTarefa.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Tarefa>>((tarefas) =>
        {
            foreach (Tarefa t in tarefas)
                repositorioTarefa.Cadastrar(t);
        });
    }

    [TestCleanup]
    public void DescartarContexto()
    {
        dbContext.Dispose();
    }

    private static ControleDeBarDbContext CriarDbContext()
    {
        DbContextOptions<ControleDeBarDbContext> options =
            new DbContextOptionsBuilder<ControleDeBarDbContext>()
                .UseInMemoryDatabase($"integracao-{Guid.NewGuid():N}")
                .Options;

        return new ControleDeBarDbContext(options);
    }
}
