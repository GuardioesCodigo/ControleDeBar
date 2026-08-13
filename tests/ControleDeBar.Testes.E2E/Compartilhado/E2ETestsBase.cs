using Microsoft.Playwright.MSTest;

namespace ControleDeBar.Testes.E2E.Compartilhado;

public abstract class E2ETestsBase : PageTest
{
    private TestApplicationFactory aplicacao = null!;

    protected string UrlBase { get; set; } = string.Empty;

    [TestInitialize]
    public void InicializarAplicacao()
    {
        aplicacao = new TestApplicationFactory();

        UrlBase = aplicacao.UrlBase;
    }

    [TestCleanup]
    public async Task EncerrarAplicacao()
    {
        try
        {
            if (aplicacao is not null)
                await aplicacao.DisposeAsync();
        }
        finally
        {
            aplicacao = null!;
        }
    }
}
