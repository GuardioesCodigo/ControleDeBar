using ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloPedido;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using ControleDeBar.Infra.Compartilhado.Orm;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloFaturamento;

[TestClass]
public sealed class ServicoFaturamentoTests : Compartilhado.Orm.RepositorioBaseEmOrmTests
{
    private ServicoFaturamento servicoFaturamento = null!;
    private readonly DateOnly hoje = DateOnly.FromDateTime(DateTime.UtcNow);

    [TestInitialize]
    public void InicializarServico()
    {
        servicoFaturamento = new ServicoFaturamento(repositorioConta);
    }

    private Conta AbrirEFecharConta(string cliente, int numeroMesa, decimal precoProduto, int quantidade, DateTime dataAbertura)
    {
        Mesa mesa = new() { Numero = numeroMesa, QuantidadeLugares = 4 };
        repositorioMesa.Cadastrar(mesa);

        Garcom garcom = new() { Nome = "João Silva" };
        repositorioGarcom.Cadastrar(garcom);

        Produto produto = new() { Nome = "Produto", Preco = precoProduto };
        repositorioProduto.Cadastrar(produto);

        Conta conta = new()
        {
            NomeCliente = cliente,
            MesaId = mesa.Id,
            GarcomId = garcom.Id,
            DataAbertura = dataAbertura,
            Situacao = SituacaoConta.Fechada
        };
        repositorioConta.Cadastrar(conta);

        repositorioPedido.Cadastrar(new Pedido { ContaId = conta.Id, ProdutoId = produto.Id, Quantidade = quantidade });

        return conta;
    }

    [TestMethod]
    public void SelecionarFaturamentoDoDia_DeveRetornarValorTotalDasContasFechadasNaData()
    {
        // CT-FAT-001
        AbrirEFecharConta("Carlos Andrade", 1, 28m, 2, DateTime.UtcNow);

        FaturamentoDiarioDto faturamento = servicoFaturamento.SelecionarFaturamentoDoDia(hoje);

        Assert.AreEqual(56m, faturamento.ValorTotal);
        Assert.AreEqual(1, faturamento.ContasFechadas.Count);
    }

    [TestMethod]
    public void SelecionarFaturamentoDoDia_NaoDeveConsiderarContasComSituacaoAberta()
    {
        // CT-FAT-002 / CT-FAT-003
        Mesa mesa = new() { Numero = 1, QuantidadeLugares = 4 };
        repositorioMesa.Cadastrar(mesa);
        Garcom garcom = new() { Nome = "João Silva" };
        repositorioGarcom.Cadastrar(garcom);

        // Uma conta fechada e uma aberta no mesmo dia
        AbrirEFecharConta("Cliente Fechado", 2, 10m, 1, DateTime.UtcNow);

        repositorioConta.Cadastrar(new Conta
        {
            NomeCliente = "Cliente Aberto",
            MesaId = mesa.Id,
            GarcomId = garcom.Id,
            DataAbertura = DateTime.UtcNow,
            Situacao = SituacaoConta.Aberta
        });

        FaturamentoDiarioDto faturamento = servicoFaturamento.SelecionarFaturamentoDoDia(hoje);

        Assert.AreEqual(10m, faturamento.ValorTotal);
        Assert.AreEqual(1, faturamento.ContasFechadas.Count);
    }

    [TestMethod]
    public void SelecionarFaturamentoDoDia_DeveRetornarZero_QuandoNaoHaContasFechadasNaData()
    {
        // CT-FAT-004
        FaturamentoDiarioDto faturamento = servicoFaturamento.SelecionarFaturamentoDoDia(hoje);

        Assert.AreEqual(0m, faturamento.ValorTotal);
        Assert.AreEqual(0, faturamento.ContasFechadas.Count);
    }

    [TestMethod]
    public void SelecionarFaturamentoDoDia_NaoDeveConsiderarContasDeOutroEstabelecimento()
    {
        // CT-FAT-005
        AbrirEFecharConta("Cliente do Bar do Zé", 1, 28m, 1, DateTime.UtcNow);

        using ControleDeBarDbContext contextoOutroEstabelecimento =
            CriarContextoParaOutroEstabelecimento(out _);

        Infra.Modulos.ModuloConta.RepositorioContaEmOrm repositorioContaOutro = new(contextoOutroEstabelecimento);
        ServicoFaturamento servicoFaturamentoOutro = new(repositorioContaOutro);

        FaturamentoDiarioDto faturamentoDoOutro = servicoFaturamentoOutro.SelecionarFaturamentoDoDia(hoje);

        Assert.AreEqual(0m, faturamentoDoOutro.ValorTotal);
    }

    [TestMethod]
    public void SelecionarFaturamentoDoDia_DeveConsolidarMultiplasContasFechadasNoMesmoDia()
    {
        // CT-FAT-006
        AbrirEFecharConta("Cliente 1", 1, 28m, 1, DateTime.UtcNow); // 28
        AbrirEFecharConta("Cliente 2", 2, 6m, 3, DateTime.UtcNow);  // 18
        AbrirEFecharConta("Cliente 3", 3, 22m, 2, DateTime.UtcNow); // 44

        FaturamentoDiarioDto faturamento = servicoFaturamento.SelecionarFaturamentoDoDia(hoje);

        Assert.AreEqual(90m, faturamento.ValorTotal); // 28 + 18 + 44
        Assert.AreEqual(3, faturamento.ContasFechadas.Count);
    }

    [TestMethod]
    public void SelecionarFaturamentoDoDia_NaoDeveConsiderarContasFechadasEmOutraData()
    {
        // Base de CT-FAT-001: consulta filtra por data, não traz o histórico inteiro
        AbrirEFecharConta("Cliente de Ontem", 1, 50m, 1, DateTime.UtcNow.AddDays(-1));

        FaturamentoDiarioDto faturamento = servicoFaturamento.SelecionarFaturamentoDoDia(hoje);

        Assert.AreEqual(0m, faturamento.ValorTotal);
    }

    [TestMethod]
    public void SelecionarFaturamentoDoDia_DeveAtualizarAutomaticamenteAoFecharNovaConta()
    {
        // CT-FAT-008
        FaturamentoDiarioDto faturamentoAntes = servicoFaturamento.SelecionarFaturamentoDoDia(hoje);
        Assert.AreEqual(0m, faturamentoAntes.ValorTotal);

        AbrirEFecharConta("Cliente Novo", 1, 40m, 1, DateTime.UtcNow);

        FaturamentoDiarioDto faturamentoDepois = servicoFaturamento.SelecionarFaturamentoDoDia(hoje);
        Assert.AreEqual(40m, faturamentoDepois.ValorTotal);
    }
}
