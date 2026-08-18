using ControleDeBar.Dominio.Modulos.ModuloConta;

namespace ControleDeBar.Aplicacao.Modulos.ModuloConta;

public record CadastrarContaDto(
    string NomeCliente,
    Guid MesaId,
    Guid GarcomId
);

public record EditarContaDto(
    Guid Id,
    string NomeCliente,
    Guid MesaId,
    Guid GarcomId
);

public record ListarContaDto(
    Guid Id,
    string NomeCliente,
    int NumeroMesa,
    string NomeGarcom,
    SituacaoConta Situacao,
    decimal ValorTotal
);

public record ItemPedidoContaDto(
    Guid Id,
    string NomeProduto,
    int Quantidade,
    decimal Subtotal
);

public record DetalhesContaDto(
    Guid Id,
    string NomeCliente,
    Guid MesaId,
    int NumeroMesa,
    Guid GarcomId,
    string NomeGarcom,
    DateTime DataAbertura,
    SituacaoConta Situacao,
    List<ItemPedidoContaDto> Pedidos,
    decimal ValorTotal
);
