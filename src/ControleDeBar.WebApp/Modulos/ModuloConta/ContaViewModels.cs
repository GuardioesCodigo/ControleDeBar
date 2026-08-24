using System.ComponentModel.DataAnnotations;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControleDeBar.WebApp.Modulos.ModuloConta;

public record ListarContaViewModel(
    Guid Id,
    string NomeCliente,
    int NumeroMesa,
    string NomeGarcom,
    SituacaoConta Situacao,
    decimal ValorTotal
);

public record AbrirContaViewModel(
    [Required(ErrorMessage = "O campo \"Nome do Cliente\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo \"Nome do Cliente\" deve conter entre 2 e 100 caracteres.")]
    string NomeCliente,

    [Required(ErrorMessage = "O campo \"Mesa\" deve ser preenchido.")]
    Guid MesaId,

    [Required(ErrorMessage = "O campo \"Garçom\" deve ser preenchido.")]
    Guid GarcomId
)
{
    public List<SelectListItem> Mesas { get; set; } = [];
    public List<SelectListItem> Garcons { get; set; } = [];
}

public record EditarContaViewModel(
    Guid Id,

    [Required(ErrorMessage = "O campo \"Nome do Cliente\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo \"Nome do Cliente\" deve conter entre 2 e 100 caracteres.")]
    string NomeCliente,

    [Required(ErrorMessage = "O campo \"Mesa\" deve ser preenchido.")]
    Guid MesaId,

    [Required(ErrorMessage = "O campo \"Garçom\" deve ser preenchido.")]
    Guid GarcomId
)
{
    public List<SelectListItem> Mesas { get; set; } = [];
    public List<SelectListItem> Garcons { get; set; } = [];
    public List<ItemPedidoContaViewModel> Pedidos { get; set; } = [];
}

public record ItemPedidoViewModel(
    Guid Id,
    string NomeProduto,
    int Quantidade,
    decimal Subtotal
);

public record VisualizarContaViewModel(
    Guid Id,
    string NomeCliente,
    int NumeroMesa,
    string NomeGarcom,
    DateTime DataAbertura,
    SituacaoConta Situacao,
    List<ItemPedidoViewModel> Pedidos,
    decimal ValorTotal
);

public record ItemPedidoContaViewModel(
    Guid Id,
    string NomeProduto,
    int Quantidade,
    decimal Subtotal
);
