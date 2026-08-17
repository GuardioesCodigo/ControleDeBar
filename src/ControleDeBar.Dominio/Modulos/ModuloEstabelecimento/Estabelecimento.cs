using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Dominio.Modulos.ModuloEstabelecimento;

public sealed class Estabelecimento : EntidadeBase<Estabelecimento>
{
    public string Nome { get; set; } = string.Empty;
    public string? Endereco { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(Nome))
            erros.Add("O campo \"Nome\" deve ser preenchido.");
        else if (Nome.Trim().Length is < 2 or > 100)
            erros.Add("O campo \"Nome\" deve conter entre 2 e 100 caracteres.");

        if (!string.IsNullOrWhiteSpace(Endereco) && Endereco.Trim().Length > 200)
            erros.Add("O campo \"Endereço\" deve conter no máximo 200 caracteres.");

        return erros;
    }

    public override void Atualizar(Estabelecimento entidadeAtualizada)
    {
        Nome = entidadeAtualizada.Nome;
        Endereco = entidadeAtualizada.Endereco;
    }
}
