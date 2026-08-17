namespace ControleDeBar.WebApp.Modulos.ModuloFaturamento;

public record ContaFechadaViewModel(
    Guid Id,
    string NomeCliente,
    int NumeroMesa,
    string NomeGarcom,
    decimal ValorTotal
);

public record VisualizarFaturamentoViewModel(
    DateOnly Data,
    decimal ValorTotal,
    List<ContaFechadaViewModel> ContasFechadas
);
