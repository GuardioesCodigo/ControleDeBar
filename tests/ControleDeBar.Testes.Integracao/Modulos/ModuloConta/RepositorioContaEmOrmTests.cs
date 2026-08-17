using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloConta;

[TestClass]
public sealed class RepositorioContaEmOrmTests : Compartilhado.Orm.RepositorioBaseEmOrmTests
{
    private Mesa CriarMesa(int numero = 1)
    {
        Mesa mesa = new() { Numero = numero, QuantidadeLugares = 4 };
        repositorioMesa.Cadastrar(mesa);
        return mesa;
    }

    private Garcom CriarGarcom()
    {
        Garcom garcom = new() { Nome = "João Silva" };
        repositorioGarcom.Cadastrar(garcom);
        return garcom;
    }

    [TestMethod]
    public void MesaPossuiContaAberta_DeveRetornarVerdadeiro_QuandoJaExisteContaAbertaNaMesa()
    {
        // CT-CTA-004: mesa já ocupada por outra conta aberta
        Mesa mesa = CriarMesa();
        Garcom garcom = CriarGarcom();

        repositorioConta.Cadastrar(new Conta
        {
            NomeCliente = "Carlos Andrade",
            MesaId = mesa.Id,
            GarcomId = garcom.Id,
            Situacao = SituacaoConta.Aberta
        });

        bool possuiContaAberta = repositorioConta.MesaPossuiContaAberta(mesa.Id);

        Assert.IsTrue(possuiContaAberta);
    }

    [TestMethod]
    public void MesaPossuiContaAberta_DeveRetornarFalso_QuandoContaDaMesaEstaFechada()
    {
        // CT-CTA-012: após o fechamento a mesa volta a poder receber novas contas
        Mesa mesa = CriarMesa();
        Garcom garcom = CriarGarcom();

        repositorioConta.Cadastrar(new Conta
        {
            NomeCliente = "Ana Paula",
            MesaId = mesa.Id,
            GarcomId = garcom.Id,
            Situacao = SituacaoConta.Fechada
        });

        bool possuiContaAberta = repositorioConta.MesaPossuiContaAberta(mesa.Id);

        Assert.IsFalse(possuiContaAberta);
    }

    [TestMethod]
    public void SelecionarAbertas_DeveRetornarApenasContasComSituacaoAberta()
    {
        // CT-CTA-009
        Mesa mesa1 = CriarMesa(1);
        Mesa mesa2 = CriarMesa(2);
        Garcom garcom = CriarGarcom();

        repositorioConta.Cadastrar(new Conta { NomeCliente = "Cliente Aberto", MesaId = mesa1.Id, GarcomId = garcom.Id, Situacao = SituacaoConta.Aberta });
        repositorioConta.Cadastrar(new Conta { NomeCliente = "Cliente Fechado", MesaId = mesa2.Id, GarcomId = garcom.Id, Situacao = SituacaoConta.Fechada });

        List<Conta> abertas = repositorioConta.SelecionarAbertas();

        Assert.AreEqual(1, abertas.Count);
        Assert.AreEqual("Cliente Aberto", abertas[0].NomeCliente);
    }

    [TestMethod]
    public void SelecionarTodos_NaoDeveRetornarContasDeOutroEstabelecimento()
    {
        // CT-CTA-008: isolamento por estabelecimento
        Mesa mesa = CriarMesa();
        Garcom garcom = CriarGarcom();

        repositorioConta.Cadastrar(new Conta { NomeCliente = "Cliente A", MesaId = mesa.Id, GarcomId = garcom.Id });

        using ControleDeBarDbContext contextoOutroEstabelecimento =
            CriarContextoParaOutroEstabelecimento(out _);

        Infra.Modulos.ModuloConta.RepositorioContaEmOrm repositorioContaOutro = new(contextoOutroEstabelecimento);

        List<Conta> contasDoOutroEstabelecimento = repositorioContaOutro.SelecionarTodos();

        Assert.AreEqual(0, contasDoOutroEstabelecimento.Count);
    }

    [TestMethod]
    public void ValorTotal_DeveSomarSubtotaisDosPedidosVinculados()
    {
        // CT-CTA-014 / CT-PED-009: cálculo do total com múltiplos pedidos
        Mesa mesa = CriarMesa();
        Garcom garcom = CriarGarcom();

        Dominio.Modulos.ModuloProduto.Produto hamburguer = new() { Nome = "Hambúrguer", Preco = 28m };
        Dominio.Modulos.ModuloProduto.Produto refrigerante = new() { Nome = "Refrigerante", Preco = 6m };
        repositorioProduto.Cadastrar(hamburguer);
        repositorioProduto.Cadastrar(refrigerante);

        Conta conta = new() { NomeCliente = "Carlos Andrade", MesaId = mesa.Id, GarcomId = garcom.Id };
        repositorioConta.Cadastrar(conta);

        repositorioPedido.Cadastrar(new Dominio.Modulos.ModuloPedido.Pedido { ContaId = conta.Id, ProdutoId = hamburguer.Id, Quantidade = 2 });
        repositorioPedido.Cadastrar(new Dominio.Modulos.ModuloPedido.Pedido { ContaId = conta.Id, ProdutoId = refrigerante.Id, Quantidade = 3 });

        Conta? contaComPedidos = repositorioConta.SelecionarPorId(conta.Id);

        Assert.IsNotNull(contaComPedidos);
        Assert.AreEqual(74m, contaComPedidos.ValorTotal); // 2*28 + 3*6 = 74
    }
}
