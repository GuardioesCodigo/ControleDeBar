using System.ComponentModel.DataAnnotations;
using ControleDeBar.Dominio.Modulos.ModuloMesa;

namespace ControleDeBar.WebApp.Modulos.ModuloMesa;

public record ListarMesaViewModel(
    Guid Id,
    int Numero,
    int QuantidadeLugares,
    StatusMesa Status
);

public record CadastrarMesaViewModel(
    [Required(ErrorMessage = "O campo \"Número\" deve ser preenchido.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo \"Número\" deve ser maior que zero.")]
    int Numero,

    [Required(ErrorMessage = "O campo \"Quantidade de Lugares\" deve ser preenchido.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo \"Quantidade de Lugares\" deve ser maior que zero.")]
    int QuantidadeLugares,

    StatusMesa Status
);

public record EditarMesaViewModel(
    [Required(ErrorMessage = "O campo \"Número\" deve ser preenchido.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo \"Número\" deve ser maior que zero.")]
    int Numero,

    [Required(ErrorMessage = "O campo \"Quantidade de Lugares\" deve ser preenchido.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo \"Quantidade de Lugares\" deve ser maior que zero.")]
    int QuantidadeLugares,

    StatusMesa Status
);

public record ExcluirMesaViewModel(
    Guid Id,
    int Numero
);
