using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloEstabelecimento;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using FluentResults;

namespace ControleDeBar.Aplicacao.Modulos.ModuloMesa;

public sealed class ServicoMesa(
    IRepositorioMesa repositorioMesa,
    IRepositorioEstabelecimento repositorioEstabelecimento
) : ServicoBase<Mesa>
{
    public Result Cadastrar(CadastrarMesaDto dto)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Falha(string.Empty, "Estabelecimento não encontrado.");

        if (repositorioMesa.NumeroJaExiste(dto.Numero, estabelecimento.Id))
            return Falha(nameof(dto.Numero), "Já existe uma mesa com este número.");

        Mesa mesa = new()
        {
            Numero = dto.Numero,
            QuantidadeLugares = dto.QuantidadeLugares,
            Status = dto.Status,
            EstabelecimentoId = estabelecimento.Id
        };

        Result resultadoValidacao = ValidarEntidade(mesa);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioMesa.Cadastrar(mesa);

        return Result.Ok();
    }

    public Result Editar(EditarMesaDto dto)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Falha(string.Empty, "Estabelecimento não encontrado.");

        Mesa? mesa = repositorioMesa.SelecionarPorId(dto.Id);

        if (mesa == null || mesa.EstabelecimentoId != estabelecimento.Id)
            return Falha(string.Empty, "Mesa não encontrada.");

        if (repositorioMesa.NumeroJaExiste(dto.Numero, estabelecimento.Id, dto.Id))
            return Falha(nameof(dto.Numero), "Já existe uma mesa com este número.");

        Mesa atualizada = new()
        {
            Numero = dto.Numero,
            QuantidadeLugares = dto.QuantidadeLugares,
            Status = dto.Status,
            EstabelecimentoId = estabelecimento.Id
        };

        Result resultadoValidacao = ValidarEntidade(atualizada);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioMesa.Editar(dto.Id, atualizada);

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Falha(string.Empty, "Estabelecimento não encontrado.");

        Mesa? mesa = repositorioMesa.SelecionarPorId(id);

        if (mesa == null || mesa.EstabelecimentoId != estabelecimento.Id)
            return Falha(string.Empty, "Mesa não encontrada.");

        if (repositorioMesa.PossuiContaAbertaVinculada(id))
            return Falha(
                string.Empty,
                "Não é possível excluir uma mesa com conta aberta vinculada."
            );

        repositorioMesa.Excluir(id);

        return Result.Ok();
    }

    public List<ListarMesaDto> SelecionarTodos()
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return [];

        return repositorioMesa
            .SelecionarTodos()
            .Where(m => m.EstabelecimentoId == estabelecimento.Id)
            .OrderBy(m => m.Numero)
            .Select(m => new ListarMesaDto(
                m.Id,
                m.Numero,
                m.QuantidadeLugares,
                m.Status
            ))
            .ToList();
    }

    public Result<DetalhesMesaDto> SelecionarPorId(Guid id)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Result.Fail("Estabelecimento não encontrado.");

        Mesa? mesa = repositorioMesa.SelecionarPorId(id);

        if (mesa == null || mesa.EstabelecimentoId != estabelecimento.Id)
            return Result.Fail("Mesa não encontrada.");

        return Result.Ok(new DetalhesMesaDto(
            mesa.Id,
            mesa.Numero,
            mesa.QuantidadeLugares,
            mesa.Status
        ));
    }
}