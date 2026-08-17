using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloPedido;

namespace ControleDeBar.Dominio.Modulos.ModuloConta;

public enum SituacaoConta
{
    Aberta = 0,
    Fechada = 1
}

public sealed class Conta : EntidadeBase<Conta>
{
    public string NomeCliente { get; set; } = string.Empty;

    public Guid MesaId { get; set; }
    public Mesa Mesa { get; set; } = null!;

    public Guid GarcomId { get; set; }
    public Garcom Garcom { get; set; } = null!;

    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
    public SituacaoConta Situacao { get; set; } = SituacaoConta.Aberta;

    public List<Pedido> Pedidos { get; set; } = [];

    // Não mapeado: calculado a partir dos pedidos vinculados.
    public decimal ValorTotal => Pedidos.Sum(p => p.Subtotal);

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(NomeCliente))
            erros.Add("O campo \"Nome do Cliente\" deve ser preenchido.");
        else if (NomeCliente.Trim().Length is < 2 or > 100)
            erros.Add("O campo \"Nome do Cliente\" deve conter entre 2 e 100 caracteres.");

        if (MesaId == Guid.Empty)
            erros.Add("O campo \"Mesa\" deve ser preenchido.");

        if (GarcomId == Guid.Empty)
            erros.Add("O campo \"Garçom\" deve ser preenchido.");

        if (!Enum.IsDefined(Situacao))
            erros.Add("O campo \"Situação\" deve ser \"Aberta\" ou \"Fechada\".");

        return erros;
    }

    public void Fechar()
    {
        Situacao = SituacaoConta.Fechada;
    }

    public override void Atualizar(Conta entidadeAtualizada)
    {
        NomeCliente = entidadeAtualizada.NomeCliente;
        MesaId = entidadeAtualizada.MesaId;
        GarcomId = entidadeAtualizada.GarcomId;
        Situacao = entidadeAtualizada.Situacao;
    }
}
