using System.ComponentModel.DataAnnotations;
using ControleDeBar.WebApp.Modulos.ModuloAutenticacao;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloAutenticacao;

[TestClass]
public sealed class EntrarViewModelTests
{
    [TestMethod]
    public void Validar_DeveRetornarErros_QuandoCamposObrigatoriosEstaoEmBranco()
    {
        // CT-USR-010
        EntrarViewModel viewModel = new() { Email = "", Senha = "" };

        List<ValidationResult> erros = [];
        ValidationContext contexto = new(viewModel);

        Validator.TryValidateObject(viewModel, contexto, erros, validateAllProperties: true);

        Assert.IsTrue(erros.Any(e => e.MemberNames.Contains(nameof(EntrarViewModel.Email))));
        Assert.IsTrue(erros.Any(e => e.MemberNames.Contains(nameof(EntrarViewModel.Senha))));
    }
}
