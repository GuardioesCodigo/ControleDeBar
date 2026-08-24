using System.ComponentModel.DataAnnotations;
using ControleDeBar.WebApp.Modulos.ModuloAutenticacao;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloAutenticacao;

[TestClass]
public sealed class RegistrarViewModelTests
{
    private static List<ValidationResult> Validar(RegistrarViewModel viewModel)
    {
        List<ValidationResult> resultados = [];
        ValidationContext contexto = new(viewModel);

        Validator.TryValidateObject(viewModel, contexto, resultados, validateAllProperties: true);

        return resultados;
    }

    [TestMethod]
    public void Validar_NaoDeveRetornarErros_QuandoTodosOsCamposSaoValidos()
    {
        RegistrarViewModel viewModel = new()
        {
            Email = "usuario@bar.com",
            Senha = "SenhaForte@123",
            ConfirmarSenha = "SenhaForte@123",
            NomeEstabelecimento = "Bar do Zé"
        };

        Assert.AreEqual(0, Validar(viewModel).Count);
    }

    [TestMethod]
    public void Validar_DeveRetornarErros_QuandoCamposObrigatoriosEstaoEmBranco()
    {
        // CT-USR-002
        RegistrarViewModel viewModel = new()
        {
            Email = "",
            Senha = "",
            ConfirmarSenha = "",
            NomeEstabelecimento = ""
        };

        List<ValidationResult> erros = Validar(viewModel);

        Assert.IsTrue(erros.Any(e => e.MemberNames.Contains(nameof(RegistrarViewModel.Email))));
        Assert.IsTrue(erros.Any(e => e.MemberNames.Contains(nameof(RegistrarViewModel.Senha))));
        Assert.IsTrue(erros.Any(e => e.MemberNames.Contains(nameof(RegistrarViewModel.ConfirmarSenha))));
        Assert.IsTrue(erros.Any(e => e.MemberNames.Contains(nameof(RegistrarViewModel.NomeEstabelecimento))));
    }

    [TestMethod]
    public void Validar_DeveRetornarErro_QuandoEmailTemFormatoInvalido()
    {
        // CT-USR-003
        RegistrarViewModel viewModel = new()
        {
            Email = "nao-e-um-email",
            Senha = "SenhaForte@123",
            ConfirmarSenha = "SenhaForte@123",
            NomeEstabelecimento = "Bar do Zé"
        };

        List<ValidationResult> erros = Validar(viewModel);

        Assert.IsTrue(erros.Any(e => e.MemberNames.Contains(nameof(RegistrarViewModel.Email))));
    }

    [TestMethod]
    public void Validar_DeveRetornarErro_QuandoSenhaTemMenosDeOitoCaracteres()
    {
        // CT-USR-005
        RegistrarViewModel viewModel = new()
        {
            Email = "usuario@bar.com",
            Senha = "123",
            ConfirmarSenha = "123",
            NomeEstabelecimento = "Bar do Zé"
        };

        List<ValidationResult> erros = Validar(viewModel);

        Assert.IsTrue(erros.Any(e => e.MemberNames.Contains(nameof(RegistrarViewModel.Senha))));
    }
}
