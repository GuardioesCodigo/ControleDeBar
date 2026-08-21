namespace ControleDeBar.Aplicacao.Modulos.ModuloEstabelecimento;

public record CadastrarEstabelecimento(
    string Nome,
    string? Endereco
);
public record EditarEstabelecimentoDto(
    string Nome,
    string? Endereco
);

public record DetalhesEstabelecimentoDto(
    Guid Id,
    string Nome,
    string? Endereco,
    DateTime DataCriacao
);
