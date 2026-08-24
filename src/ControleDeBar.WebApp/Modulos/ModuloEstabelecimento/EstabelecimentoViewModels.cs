using System.ComponentModel.DataAnnotations;

namespace ControleDeBar.WebApp.Modulos.ModuloEstabelecimento;

public record DetalhesEstabelecimentoViewModel(
    Guid Id,
    string Nome,
    string? Endereco,
    DateTime DataCriacao
);

public record EditarEstabelecimentoViewModel(
    [Required(ErrorMessage = "O campo \"Nome\" deve ser preenchido.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "O campo \"Nome\" deve conter entre 2 e 100 caracteres."
    )]
    string Nome,

    [StringLength(
        200,
        ErrorMessage = "O campo \"Endereço\" deve conter no máximo 200 caracteres."
    )]
    string? Endereco
);