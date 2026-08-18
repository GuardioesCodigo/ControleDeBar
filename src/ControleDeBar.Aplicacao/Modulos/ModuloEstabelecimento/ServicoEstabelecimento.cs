using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Dominio.Modulos.ModuloEstabelecimento;
using FluentResults;

namespace ControleDeBar.Aplicacao.Modulos.ModuloEstabelecimento;

public sealed class ServicoEstabelecimento(
    IRepositorioEstabelecimento repositorioEstabelecimento,
    IProvedorDeUsuario provedorDeUsuario
) : ServicoBase<Estabelecimento>
{
    public Result Cadastrar(string nome)
    {
        if (!provedorDeUsuario.EstaAutenticado || provedorDeUsuario.Id is null)
            return Falha(string.Empty, "Usuário não autenticado.");

        Estabelecimento estabelecimento = new()
        {
            Nome = nome,
            UserId = provedorDeUsuario.Id.Value
        };

        Result resultadoValidacao = ValidarEntidade(estabelecimento);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioEstabelecimento.Cadastrar(estabelecimento);

        return Result.Ok();
    }

    public Result Editar(EditarEstabelecimentoDto dto)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Falha(string.Empty, "Estabelecimento não encontrado.");

        Estabelecimento atualizado = new()
        {
            Nome = dto.Nome,
            Endereco = dto.Endereco
        };

        Result resultadoValidacao = ValidarEntidade(atualizado);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioEstabelecimento.Editar(estabelecimento.Id, atualizado);

        return Result.Ok();
    }

    public Result<DetalhesEstabelecimentoDto> SelecionarAtual()
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Result.Fail("Estabelecimento não encontrado.");

        return Result.Ok(new DetalhesEstabelecimentoDto(
            estabelecimento.Id,
            estabelecimento.Nome,
            estabelecimento.Endereco,
            estabelecimento.DataCriacao
        ));
    }
}