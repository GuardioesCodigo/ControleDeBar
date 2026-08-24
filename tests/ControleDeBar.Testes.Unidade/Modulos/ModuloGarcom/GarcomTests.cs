using ControleDeBar.Dominio.Modulos.ModuloGarcom;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloGarcom;

[TestClass]
public sealed class GarcomTests
{
    [TestMethod]
    public void Validar_DeveRetornarSemErros_QuandoNomeEstaPreenchido()
    {
        // CT-GAR-001
        Garcom garcom = new() { Nome = "João Silva" };

        Assert.AreEqual(0, garcom.Validar().Count);
    }

    [TestMethod]
    public void Validar_DeveRetornarErro_QuandoNomeEstaEmBranco()
    {
        // CT-GAR-002
        Garcom garcom = new() { Nome = "" };

        List<string> erros = garcom.Validar();

        Assert.IsTrue(erros.Any(e => e.Contains("Nome")));
    }
}
