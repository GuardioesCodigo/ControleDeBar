using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using FluentResults;

namespace ControleDeBar.Aplicacao.Modulos.ModuloMesa;

public sealed class ServicoMesa(
    IRepositorioMesa repositorioMesa
) : ServicoBase<Mesa>
{
    public Result Cadastrar(CadastrarMesaDto dto)
    {
        if (repositorioMesa.NumeroJaExiste(dto.Numero))
            return Falha(nameof(dto.Numero), "Já existe uma mesa com este número.");

        Mesa mesa = new()
        {
            Numero = dto.Numero,
            QuantidadeLugares = dto.QuantidadeLugares,
            Status = dto.Status
        };

        Result resultadoValidacao = ValidarEntidade(mesa);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioMesa.Cadastrar(mesa);

        return Result.Ok();
    }

    public Result Editar(EditarMesaDto dto)
    {
        Mesa? mesa = repositorioMesa.SelecionarPorId(dto.Id);

        if (mesa == null)
            return Falha(string.Empty, "Mesa não encontrada.");

        if (repositorioMesa.NumeroJaExiste(dto.Numero, dto.Id))
            return Falha(nameof(dto.Numero), "Já existe uma mesa com este número.");

        Mesa atualizada = new()
        {
            Numero = dto.Numero,
            QuantidadeLugares = dto.QuantidadeLugares,
            Status = dto.Status
        };

        Result resultadoValidacao = ValidarEntidade(atualizada);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioMesa.Editar(dto.Id, atualizada);

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Mesa? mesa = repositorioMesa.SelecionarPorId(id);

        if (mesa == null)
            return Falha(string.Empty, "Mesa não encontrada.");

        if (repositorioMesa.PossuiContaAbertaVinculada(id))
            return Falha(string.Empty, "Não é possível excluir uma mesa com conta aberta vinculada.");

        repositorioMesa.Excluir(id);

        return Result.Ok();
    }

    public List<ListarMesaDto> SelecionarTodos()
    {
        return repositorioMesa
            .SelecionarTodos()
            .OrderBy(m => m.Numero)
            .Select(m => new ListarMesaDto(m.Id, m.Numero, m.QuantidadeLugares, m.Status))
            .ToList();
    }

    public Result<DetalhesMesaDto> SelecionarPorId(Guid id)
    {
        Mesa? mesa = repositorioMesa.SelecionarPorId(id);

        if (mesa == null)
            return Result.Fail("Mesa não encontrada.");

        return Result.Ok(new DetalhesMesaDto(mesa.Id, mesa.Numero, mesa.QuantidadeLugares, mesa.Status));
    }
}
