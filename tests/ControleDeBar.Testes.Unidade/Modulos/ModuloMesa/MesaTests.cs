using ControleDeBar.Dominio.Modulos.ModuloMesa;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloMesa;

[TestClass]
public sealed class MesaTests
{
    [TestMethod]
    public void Validar_DeveRetornarSemErros_QuandoMesaEstaValida()
    {
        // CT-MSA-001
        Mesa mesa = new() { Numero = 1, QuantidadeLugares = 4, Status = StatusMesa.Livre };

        List<string> erros = mesa.Validar();

        Assert.AreEqual(0, erros.Count);
    }

    [TestMethod]
    public void Validar_DeveRetornarErro_QuandoNumeroForZeroOuNegativo()
    {
        // CT-MSA-006 (mesma regra de "maior que zero" aplicada ao número)
        Mesa mesa = new() { Numero = 0, QuantidadeLugares = 4 };

        List<string> erros = mesa.Validar();

        Assert.IsTrue(erros.Any(e => e.Contains("Número")));
    }

    [TestMethod]
    public void Validar_DeveRetornarErro_QuandoQuantidadeDeLugaresForZero()
    {
        // CT-MSA-006
        Mesa mesa = new() { Numero = 1, QuantidadeLugares = 0 };

        List<string> erros = mesa.Validar();

        Assert.IsTrue(erros.Any(e => e.Contains("Quantidade de Lugares")));
    }

    [TestMethod]
    public void Validar_DeveRetornarErro_QuandoStatusForInvalido()
    {
        // CT-MSA-005: status fora de Livre/Ocupada
        Mesa mesa = new() { Numero = 1, QuantidadeLugares = 4, Status = (StatusMesa)99 };

        List<string> erros = mesa.Validar();

        Assert.IsTrue(erros.Any(e => e.Contains("Status")));
    }

    [TestMethod]
    public void Atualizar_DeveSubstituirTodosOsCampos()
    {
        // CT-MSA-009 / CT-MSA-010
        Mesa mesa = new() { Numero = 1, QuantidadeLugares = 4, Status = StatusMesa.Livre };
        Mesa novosDados = new() { Numero = 2, QuantidadeLugares = 6, Status = StatusMesa.Ocupada };

        mesa.Atualizar(novosDados);

        Assert.AreEqual(2, mesa.Numero);
        Assert.AreEqual(6, mesa.QuantidadeLugares);
        Assert.AreEqual(StatusMesa.Ocupada, mesa.Status);
    }
}
