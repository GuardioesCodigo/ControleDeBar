using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Dominio.Modulos.ModuloGarcom;

public sealed class Garcom : EntidadeBase<Garcom>
{
    public string Nome { get; set; } = string.Empty;

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(Nome))
            erros.Add("O campo \"Nome\" deve ser preenchido.");
        else if (Nome.Trim().Length is < 2 or > 100)
            erros.Add("O campo \"Nome\" deve conter entre 2 e 100 caracteres.");

        return erros;
    }

    public override void Atualizar(Garcom entidadeAtualizada)
    {
        Nome = entidadeAtualizada.Nome;
    }
}
