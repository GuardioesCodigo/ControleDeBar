using System.Net;
using ControleDeBar.Testes.E2E.Compartilhado;

namespace ControleDeBar.Testes.E2E.Modulos;

// Testes HTTP diretos (sem Playwright): mais rápidos que o navegador real, mas
// ainda exercitam o ASP.NET Core Identity de verdade contra o EF InMemory,
// cobrindo os casos de Autenticação que a jornada completa (Playwright) só
// testa implicitamente.
[TestClass]
public sealed class AutenticacaoHttpTests
{
    private WebAppHttpFactory fabrica = null!;
    private HttpClient cliente = null!;

    [TestInitialize]
    public void Inicializar()
    {
        fabrica = new WebAppHttpFactory();
        cliente = fabrica.CriarClienteSemRedirecionamento();
    }

    [TestCleanup]
    public void Encerrar()
    {
        cliente.Dispose();
        fabrica.Dispose();
    }

    private static FormUrlEncodedContent FormularioRegistro(string email, string senha, string confirmarSenha, string nomeEstabelecimento) =>
        new(new Dictionary<string, string>
        {
            ["Email"] = email,
            ["Senha"] = senha,
            ["ConfirmarSenha"] = confirmarSenha,
            ["NomeEstabelecimento"] = nomeEstabelecimento
        });

    private static FormUrlEncodedContent FormularioLogin(string email, string senha) =>
        new(new Dictionary<string, string>
        {
            ["Email"] = email,
            ["Senha"] = senha
        });

    [TestMethod]
    public async Task Registrar_DeveRedirecionar_QuandoDadosSaoValidos()
    {
        // CT-USR-001
        string email = $"{Guid.NewGuid():N}@barteste.com";

        HttpResponseMessage resposta = await cliente.PostAsync(
            "/Autenticacao/Registrar",
            FormularioRegistro(email, "SenhaForte@123", "SenhaForte@123", "Bar do Zé"));

        Assert.AreEqual(HttpStatusCode.Redirect, resposta.StatusCode);
    }

    [TestMethod]
    public async Task Registrar_DeveReexibirFormularioComErro_QuandoEmailJaExiste()
    {
        // CT-USR-004
        string email = $"{Guid.NewGuid():N}@barteste.com";

        await cliente.PostAsync("/Autenticacao/Registrar",
            FormularioRegistro(email, "SenhaForte@123", "SenhaForte@123", "Bar do Zé"));

        // O primeiro registro já autentica o cliente (SignInAsync); é preciso
        // sair antes da segunda tentativa, senão o controller redireciona
        // direto para a Home antes mesmo de validar o e-mail duplicado.
        await cliente.PostAsync("/Autenticacao/Sair", content: null);

        HttpResponseMessage segundaTentativa = await cliente.PostAsync("/Autenticacao/Registrar",
            FormularioRegistro(email, "OutraSenha@123", "OutraSenha@123", "Boteco do Chico"));

        Assert.AreEqual(HttpStatusCode.OK, segundaTentativa.StatusCode);

        string corpo = await segundaTentativa.Content.ReadAsStringAsync();
        Assert.IsTrue(corpo.Contains("already taken") || corpo.Contains("Username") || corpo.Contains("DuplicateUserName"));
    }

    [TestMethod]
    public async Task Entrar_DeveRedirecionar_QuandoCredenciaisSaoValidas()
    {
        // CT-USR-007
        string email = $"{Guid.NewGuid():N}@barteste.com";

        await cliente.PostAsync("/Autenticacao/Registrar",
            FormularioRegistro(email, "SenhaForte@123", "SenhaForte@123", "Bar do Zé"));
        await cliente.PostAsync("/Autenticacao/Sair", content: null);

        HttpResponseMessage resposta = await cliente.PostAsync(
            "/Autenticacao/Entrar", FormularioLogin(email, "SenhaForte@123"));

        Assert.AreEqual(HttpStatusCode.Redirect, resposta.StatusCode);
    }

    [TestMethod]
    public async Task Entrar_DeveReexibirFormularioComErro_QuandoEmailNaoEstaCadastrado()
    {
        // CT-USR-008
        HttpResponseMessage resposta = await cliente.PostAsync(
            "/Autenticacao/Entrar",
            FormularioLogin($"{Guid.NewGuid():N}@naoexiste.com", "QualquerSenha@123"));

        Assert.AreEqual(HttpStatusCode.OK, resposta.StatusCode);

        string corpo = await resposta.Content.ReadAsStringAsync();
        Assert.IsTrue(corpo.Contains("E-mail ou senha"));
    }

    [TestMethod]
    public async Task Entrar_DeveReexibirFormularioComErro_QuandoSenhaEstaIncorreta()
    {
        // CT-USR-009
        string email = $"{Guid.NewGuid():N}@barteste.com";

        await cliente.PostAsync("/Autenticacao/Registrar",
            FormularioRegistro(email, "SenhaForte@123", "SenhaForte@123", "Bar do Zé"));
        await cliente.PostAsync("/Autenticacao/Sair", content: null);

        HttpResponseMessage resposta = await cliente.PostAsync(
            "/Autenticacao/Entrar", FormularioLogin(email, "SenhaErrada@999"));

        Assert.AreEqual(HttpStatusCode.OK, resposta.StatusCode);

        string corpo = await resposta.Content.ReadAsStringAsync();
        Assert.IsTrue(corpo.Contains("E-mail ou senha"));
    }
}
