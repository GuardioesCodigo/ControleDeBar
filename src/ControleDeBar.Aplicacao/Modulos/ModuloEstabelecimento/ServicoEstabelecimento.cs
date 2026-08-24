using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloEstabelecimento;
using FluentResults;

namespace ControleDeBar.Aplicacao.Modulos.ModuloEstabelecimento;

public sealed class ServicoEstabelecimento(
    IRepositorioEstabelecimento repositorioEstabelecimento
) : ServicoBase<Estabelecimento>
{
    // Chamado logo após o cadastro do usuário no Identity (ver AutenticacaoController).
    // Cada usuário nasce vinculado a exatamente um estabelecimento.
    public Result Cadastrar(string nome)
    {
        Estabelecimento estabelecimento = new()
        {
            Nome = nome
        };

        Result resultadoValidacao = ValidarEntidade(estabelecimento);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioEstabelecimento.Cadastrar(estabelecimento);

        return Result.Ok();
    }

    public Result Editar(EditarEstabelecimentoDto dto)
    {
        Estabelecimento? estabelecimento = repositorioEstabelecimento.SelecionarDoUsuarioAtual();

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
        Estabelecimento? estabelecimento = repositorioEstabelecimento.SelecionarDoUsuarioAtual();

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
