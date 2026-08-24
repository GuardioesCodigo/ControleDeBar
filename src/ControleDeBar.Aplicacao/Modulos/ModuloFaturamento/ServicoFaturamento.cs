using ControleDeBar.Dominio.Modulos.ModuloConta;

namespace ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;

public sealed class ServicoFaturamento(
    IRepositorioConta repositorioConta
)
{
    // O IRepositorioConta já aplica o Query Filter por estabelecimento (via
    // ControleDeBarDbContext), então o resultado nunca inclui contas de outro dono.
    public FaturamentoDiarioDto SelecionarFaturamentoDoDia(DateOnly data)
    {
        List<Conta> contasFechadas = repositorioConta.SelecionarFechadasPorData(data);

        List<ContaFechadaDto> dtos = contasFechadas
            .Select(c => new ContaFechadaDto(c.Id, c.NomeCliente, c.Mesa.Numero, c.Garcom.Nome, c.ValorTotal))
            .ToList();

        return new FaturamentoDiarioDto(
            data,
            dtos.Sum(c => c.ValorTotal),
            dtos
        );
    }
}
