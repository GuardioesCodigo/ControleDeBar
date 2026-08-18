using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Dominio.Modulos.ModuloMesa;

public enum StatusMesa
{
    Livre = 0,
    Ocupada = 1
}

public sealed class Mesa : EntidadeBase<Mesa>
{
    public int Numero { get; set; }
    public int QuantidadeLugares { get; set; }
    public StatusMesa Status { get; set; } = StatusMesa.Livre;

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (Numero <= 0)
            erros.Add("O campo \"Número\" deve ser maior que zero.");

        if (QuantidadeLugares <= 0)
            erros.Add("O campo \"Quantidade de Lugares\" deve ser maior que zero.");

        if (!Enum.IsDefined(Status))
            erros.Add("O campo \"Status\" deve ser \"Livre\" ou \"Ocupada\".");

        return erros;
    }

    public override void Atualizar(Mesa entidadeAtualizada)
    {
        Numero = entidadeAtualizada.Numero;
        QuantidadeLugares = entidadeAtualizada.QuantidadeLugares;
        Status = entidadeAtualizada.Status;
    }
}
