using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Dominio.Modulos.ModuloProduto;

public sealed class Produto : EntidadeBase<Produto>
{
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(Nome))
            erros.Add("O campo \"Nome\" deve ser preenchido.");
        else if (Nome.Trim().Length is < 2 or > 100)
            erros.Add("O campo \"Nome\" deve conter entre 2 e 100 caracteres.");

        if (Preco <= 0)
            erros.Add("O campo \"Preço\" deve ser maior que zero.");

        return erros;
    }

    public override void Atualizar(Produto entidadeAtualizada)
    {
        Nome = entidadeAtualizada.Nome;
        Preco = entidadeAtualizada.Preco;
    }
}
