using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloEstabelecimento;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using FluentResults;

namespace ControleDeBar.Aplicacao.Modulos.ModuloGarcom;

public sealed class ServicoGarcom(
    IRepositorioGarcom repositorioGarcom,
    IRepositorioEstabelecimento repositorioEstabelecimento
) : ServicoBase<Garcom>
{
    public Result Cadastrar(CadastrarGarcomDto dto)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Falha(string.Empty, "Estabelecimento não encontrado.");

        Garcom garcom = new()
        {
            Nome = dto.Nome,
            EstabelecimentoId = estabelecimento.Id
        };

        Result resultadoValidacao = ValidarEntidade(garcom);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioGarcom.Cadastrar(garcom);

        return Result.Ok();
    }

    public Result Editar(EditarGarcomDto dto)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Falha(string.Empty, "Estabelecimento não encontrado.");

        Garcom? garcom = repositorioGarcom.SelecionarPorId(dto.Id);

        if (garcom == null || garcom.EstabelecimentoId != estabelecimento.Id)
            return Falha(string.Empty, "Garçom não encontrado.");

        Garcom atualizado = new()
        {
            Nome = dto.Nome,
            EstabelecimentoId = estabelecimento.Id
        };

        Result resultadoValidacao = ValidarEntidade(atualizado);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioGarcom.Editar(dto.Id, atualizado);

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Falha(string.Empty, "Estabelecimento não encontrado.");

        Garcom? garcom = repositorioGarcom.SelecionarPorId(id);

        if (garcom == null || garcom.EstabelecimentoId != estabelecimento.Id)
            return Falha(string.Empty, "Garçom não encontrado.");

        if (repositorioGarcom.PossuiContaAbertaVinculada(id))
            return Falha(
                string.Empty,
                "Não é possível excluir um garçom com conta aberta vinculada."
            );

        repositorioGarcom.Excluir(id);

        return Result.Ok();
    }

    public List<ListarGarcomDto> SelecionarTodos()
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return [];

        return repositorioGarcom
            .SelecionarTodos()
            .Where(g => g.EstabelecimentoId == estabelecimento.Id)
            .OrderBy(g => g.Nome)
            .Select(g => new ListarGarcomDto(g.Id, g.Nome))
            .ToList();
    }

    public Result<DetalhesGarcomDto> SelecionarPorId(Guid id)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Result.Fail("Estabelecimento não encontrado.");

        Garcom? garcom = repositorioGarcom.SelecionarPorId(id);

        if (garcom == null || garcom.EstabelecimentoId != estabelecimento.Id)
            return Result.Fail("Garçom não encontrado.");

        return Result.Ok(
            new DetalhesGarcomDto(garcom.Id, garcom.Nome)
        );
    }
}