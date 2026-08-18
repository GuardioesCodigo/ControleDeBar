using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloEstabelecimento;

namespace ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;

public sealed class ServicoFaturamento(
    IRepositorioConta repositorioConta,
    IRepositorioEstabelecimento repositorioEstabelecimento
)
{
    public FaturamentoDiarioDto SelecionarFaturamentoDoDia(DateOnly data)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return new FaturamentoDiarioDto(data, 0, []);

        List<Conta> contasFechadas = repositorioConta
            .SelecionarFechadasPorData(data)
            .Where(c => c.Mesa.EstabelecimentoId == estabelecimento.Id)
            .ToList();

        List<ContaFechadaDto> dtos = contasFechadas
            .Select(c => new ContaFechadaDto(
                c.Id,
                c.NomeCliente,
                c.Mesa.Numero,
                c.Garcom.Nome,
                c.ValorTotal
            ))
            .ToList();

        return new FaturamentoDiarioDto(
            data,
            dtos.Sum(c => c.ValorTotal),
            dtos
        );
    }
}