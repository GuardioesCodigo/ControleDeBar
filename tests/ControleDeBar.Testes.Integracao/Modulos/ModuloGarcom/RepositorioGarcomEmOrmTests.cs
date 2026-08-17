using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloGarcom;

[TestClass]
public sealed class RepositorioGarcomEmOrmTests : Compartilhado.Orm.RepositorioBaseEmOrmTests
{
    [TestMethod]
    public void PossuiContaAbertaVinculada_DeveRetornarVerdadeiro_QuandoGarcomTemContaAberta()
    {
        // CT-GAR-009: bloqueio de exclusão de garçom com conta aberta
        Mesa mesa = new() { Numero = 1, QuantidadeLugares = 4 };
        repositorioMesa.Cadastrar(mesa);

        Garcom garcom = new() { Nome = "Maria Souza" };
        repositorioGarcom.Cadastrar(garcom);

        repositorioConta.Cadastrar(new Conta
        {
            NomeCliente = "Cliente X",
            MesaId = mesa.Id,
            GarcomId = garcom.Id,
            Situacao = SituacaoConta.Aberta
        });

        bool possuiVinculo = repositorioGarcom.PossuiContaAbertaVinculada(garcom.Id);

        Assert.IsTrue(possuiVinculo);
    }

    [TestMethod]
    public void PossuiContaAbertaVinculada_DeveRetornarFalso_QuandoGarcomNaoTemContas()
    {
        // CT-GAR-008: exclusão permitida sem vínculo
        Garcom garcom = new() { Nome = "Pedro Lima" };
        repositorioGarcom.Cadastrar(garcom);

        bool possuiVinculo = repositorioGarcom.PossuiContaAbertaVinculada(garcom.Id);

        Assert.IsFalse(possuiVinculo);
    }
}
