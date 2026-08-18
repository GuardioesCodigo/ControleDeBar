using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloProduto;

namespace ControleDeBar.Dominio.Modulos.ModuloPedido;

public sealed class Pedido : EntidadeBase<Pedido>
{
    public Guid ContaId { get; set; }

    public Guid ProdutoId { get; set; }
    public Produto Produto { get; set; } = null!;

    public int Quantidade { get; set; }

    // Não mapeado: calculado a partir do preço do produto no momento da consulta.
    public decimal Subtotal => Produto is null ? 0 : Produto.Preco * Quantidade;

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (ContaId == Guid.Empty)
            erros.Add("O campo \"Conta\" deve ser preenchido.");

        if (ProdutoId == Guid.Empty)
            erros.Add("O campo \"Produto\" deve ser preenchido.");

        if (Quantidade <= 0)
            erros.Add("O campo \"Quantidade\" deve ser maior que zero.");

        return erros;
    }

    public override void Atualizar(Pedido entidadeAtualizada)
    {
        ProdutoId = entidadeAtualizada.ProdutoId;
        Quantidade = entidadeAtualizada.Quantidade;
    }
}
