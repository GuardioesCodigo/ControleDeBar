using ControleDeBar.Dominio.Modulos.ModuloMesa;

namespace ControleDeBar.Aplicacao.Modulos.ModuloMesa;

public record CadastrarMesaDto(
    int Numero,
    int QuantidadeLugares,
    StatusMesa Status
);

public record EditarMesaDto(
    Guid Id,
    int Numero,
    int QuantidadeLugares,
    StatusMesa Status
);

public record ListarMesaDto(
    Guid Id,
    int Numero,
    int QuantidadeLugares,
    StatusMesa Status
);

public record DetalhesMesaDto(
    Guid Id,
    int Numero,
    int QuantidadeLugares,
    StatusMesa Status
);
