using System.ComponentModel.DataAnnotations;

namespace ControleDeBar.WebApp.Modulos.ModuloPedido;

public record AdicionarPedidoViewModel(
    Guid ContaId,

    Guid ProdutoId,

    [Required(ErrorMessage = "O campo \"Quantidade\" deve ser preenchido.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo \"Quantidade\" deve ser maior que zero.")]
    int Quantidade
);
