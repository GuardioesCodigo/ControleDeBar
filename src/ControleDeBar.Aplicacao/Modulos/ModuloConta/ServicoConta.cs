using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using FluentResults;

namespace ControleDeBar.Aplicacao.Modulos.ModuloConta;

public sealed class ServicoConta(
    IRepositorioConta repositorioConta,
    IRepositorioMesa repositorioMesa,
    IRepositorioGarcom repositorioGarcom
) : ServicoBase<Conta>
{
    public Result Abrir(CadastrarContaDto dto)
    {
        Mesa? mesa = repositorioMesa.SelecionarPorId(dto.MesaId);

        if (mesa == null)
            return Falha(nameof(dto.MesaId), "Mesa não encontrada.");

        Garcom? garcom = repositorioGarcom.SelecionarPorId(dto.GarcomId);

        if (garcom == null)
            return Falha(nameof(dto.GarcomId), "Garçom não encontrado.");

        if (repositorioConta.MesaPossuiContaAberta(dto.MesaId))
            return Falha(nameof(dto.MesaId), "Esta mesa já possui uma conta em aberto.");

        Conta conta = new()
        {
            NomeCliente = dto.NomeCliente,
            MesaId = dto.MesaId,
            GarcomId = dto.GarcomId,
            DataAbertura = DateTime.UtcNow,
            Situacao = SituacaoConta.Aberta
        };

        Result resultadoValidacao = ValidarEntidade(conta);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioConta.Cadastrar(conta);

        return Result.Ok();
    }

    public Result Editar(EditarContaDto dto)
    {
        Conta? conta = repositorioConta.SelecionarPorId(dto.Id);

        if (conta == null)
            return Falha(string.Empty, "Conta não encontrada.");

        if (conta.Situacao == SituacaoConta.Fechada)
            return Falha(string.Empty, "Não é possível editar uma conta já fechada.");

        Mesa? mesa = repositorioMesa.SelecionarPorId(dto.MesaId);

        if (mesa == null)
            return Falha(nameof(dto.MesaId), "Mesa não encontrada.");

        Garcom? garcom = repositorioGarcom.SelecionarPorId(dto.GarcomId);

        if (garcom == null)
            return Falha(nameof(dto.GarcomId), "Garçom não encontrado.");

        if (repositorioConta.MesaPossuiContaAberta(dto.MesaId, dto.Id))
            return Falha(nameof(dto.MesaId), "Esta mesa já possui uma conta em aberto.");

        Conta atualizada = new()
        {
            NomeCliente = dto.NomeCliente,
            MesaId = dto.MesaId,
            GarcomId = dto.GarcomId,
            Situacao = conta.Situacao
        };

        Result resultadoValidacao = ValidarEntidade(atualizada);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioConta.Editar(dto.Id, atualizada);

        return Result.Ok();
    }

    public Result Fechar(Guid id)
    {
        Conta? conta = repositorioConta.SelecionarPorId(id);

        if (conta == null)
            return Falha(string.Empty, "Conta não encontrada.");

        if (conta.Situacao == SituacaoConta.Fechada)
            return Falha(string.Empty, "Esta conta já está fechada.");

        conta.Fechar();

        repositorioConta.Editar(id, conta);

        return Result.Ok();
    }

    public List<ListarContaDto> SelecionarTodos()
    {
        return repositorioConta
            .SelecionarTodos()
            .OrderByDescending(c => c.DataAbertura)
            .Select(Mapear)
            .ToList();
    }

    public List<ListarContaDto> SelecionarAbertas()
    {
        return repositorioConta
            .SelecionarAbertas()
            .OrderBy(c => c.Mesa.Numero)
            .Select(Mapear)
            .ToList();
    }

    public Result<DetalhesContaDto> SelecionarPorId(Guid id)
    {
        Conta? conta = repositorioConta.SelecionarPorId(id);

        if (conta == null)
            return Result.Fail("Conta não encontrada.");

        return Result.Ok(new DetalhesContaDto(
            conta.Id,
            conta.NomeCliente,
            conta.MesaId,
            conta.Mesa.Numero,
            conta.GarcomId,
            conta.Garcom.Nome,
            conta.DataAbertura,
            conta.Situacao,
            conta.Pedidos
                .Select(p => new ItemPedidoContaDto(p.Id, p.Produto.Nome, p.Quantidade, p.Subtotal))
                .ToList(),
            conta.ValorTotal
        ));
    }

    private static ListarContaDto Mapear(Conta conta)
    {
        return new ListarContaDto(
            conta.Id,
            conta.NomeCliente,
            conta.Mesa.Numero,
            conta.Garcom.Nome,
            conta.Situacao,
            conta.ValorTotal
        );
    }
}
