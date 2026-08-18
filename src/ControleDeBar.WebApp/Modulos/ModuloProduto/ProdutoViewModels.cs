using System.ComponentModel.DataAnnotations;

namespace ControleDeBar.WebApp.Modulos.ModuloProduto;

public record ListarProdutoViewModel(
    Guid Id, 
    string Nome, 
    decimal Preco
);

public record CadastrarProdutoViewModel(
    [Required(ErrorMessage = "O campo \"Nome\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo \"Nome\" deve conter entre 2 e 100 caracteres.")]
    string Nome,

    [Required(ErrorMessage = "O campo \"Preço\" deve ser preenchido.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O campo \"Preço\" deve ser maior que zero.")]
    decimal Preco
);

public record EditarProdutoViewModel(
    [Required(ErrorMessage = "O campo \"Nome\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo \"Nome\" deve conter entre 2 e 100 caracteres.")]
    string Nome,

    [Required(ErrorMessage = "O campo \"Preço\" deve ser preenchido.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O campo \"Preço\" deve ser maior que zero.")]
    decimal Preco
);

public record ExcluirProdutoViewModel(
    Guid Id, 
    string Nome
);
