namespace ControleDeBar.Aplicacao.Modulos.ModuloPedido;

public record CadastrarPedidoDto(
    Guid ContaId,
    Guid ProdutoId,
    int Quantidade
);

public record ListarPedidoDto(
    Guid Id,
    string NomeProduto,
    int Quantidade,
    decimal Subtotal
);
