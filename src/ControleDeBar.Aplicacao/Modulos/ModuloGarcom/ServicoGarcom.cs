using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using FluentResults;

namespace ControleDeBar.Aplicacao.Modulos.ModuloGarcom;

public sealed class ServicoGarcom(
    IRepositorioGarcom repositorioGarcom
) : ServicoBase<Garcom>
{
    public Result Cadastrar(CadastrarGarcomDto dto)
    {
        Garcom garcom = new() { Nome = dto.Nome };

        Result resultadoValidacao = ValidarEntidade(garcom);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioGarcom.Cadastrar(garcom);

        return Result.Ok();
    }

    public Result Editar(EditarGarcomDto dto)
    {
        Garcom? garcom = repositorioGarcom.SelecionarPorId(dto.Id);

        if (garcom == null)
            return Falha(string.Empty, "Garçom não encontrado.");

        Garcom atualizado = new() { Nome = dto.Nome };

        Result resultadoValidacao = ValidarEntidade(atualizado);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioGarcom.Editar(dto.Id, atualizado);

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Garcom? garcom = repositorioGarcom.SelecionarPorId(id);

        if (garcom == null)
            return Falha(string.Empty, "Garçom não encontrado.");

        if (repositorioGarcom.PossuiContaAbertaVinculada(id))
            return Falha(string.Empty, "Não é possível excluir um garçom com conta aberta vinculada.");

        repositorioGarcom.Excluir(id);

        return Result.Ok();
    }

    public List<ListarGarcomDto> SelecionarTodos()
    {
        return repositorioGarcom
            .SelecionarTodos()
            .OrderBy(g => g.Nome)
            .Select(g => new ListarGarcomDto(g.Id, g.Nome))
            .ToList();
    }

    public Result<DetalhesGarcomDto> SelecionarPorId(Guid id)
    {
        Garcom? garcom = repositorioGarcom.SelecionarPorId(id);

        if (garcom == null)
            return Result.Fail("Garçom não encontrado.");

        return Result.Ok(new DetalhesGarcomDto(garcom.Id, garcom.Nome));
    }
}
