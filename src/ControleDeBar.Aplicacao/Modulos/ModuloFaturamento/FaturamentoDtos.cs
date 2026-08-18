namespace ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;

public record ContaFechadaDto(
    Guid Id,
    string NomeCliente,
    int NumeroMesa,
    string NomeGarcom,
    decimal ValorTotal
);

public record FaturamentoDiarioDto(
    DateOnly Data,
    decimal ValorTotal,
    List<ContaFechadaDto> ContasFechadas
);
