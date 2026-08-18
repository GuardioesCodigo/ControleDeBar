using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloEstabelecimento;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using FluentResults;

namespace ControleDeBar.Aplicacao.Modulos.ModuloProduto;

public sealed class ServicoProduto(
    IRepositorioProduto repositorioProduto,
    IRepositorioEstabelecimento repositorioEstabelecimento
) : ServicoBase<Produto>
{
    public Result Cadastrar(CadastrarProdutoDto dto)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Falha(string.Empty, "Estabelecimento não encontrado.");

        Produto produto = new()
        {
            Nome = dto.Nome,
            Preco = dto.Preco,
            EstabelecimentoId = estabelecimento.Id
        };

        Result resultadoValidacao = ValidarEntidade(produto);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioProduto.Cadastrar(produto);

        return Result.Ok();
    }

    public Result Editar(EditarProdutoDto dto)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Falha(string.Empty, "Estabelecimento não encontrado.");

        Produto? produto = repositorioProduto.SelecionarPorId(dto.Id);

        if (produto == null || produto.EstabelecimentoId != estabelecimento.Id)
            return Falha(string.Empty, "Produto não encontrado.");

        Produto atualizado = new()
        {
            Nome = dto.Nome,
            Preco = dto.Preco,
            EstabelecimentoId = estabelecimento.Id
        };

        Result resultadoValidacao = ValidarEntidade(atualizado);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioProduto.Editar(dto.Id, atualizado);

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Falha(string.Empty, "Estabelecimento não encontrado.");

        Produto? produto = repositorioProduto.SelecionarPorId(id);

        if (produto == null || produto.EstabelecimentoId != estabelecimento.Id)
            return Falha(string.Empty, "Produto não encontrado.");

        if (repositorioProduto.PossuiPedidoVinculado(id))
            return Falha(
                string.Empty,
                "Não é possível excluir um produto com pedidos vinculados."
            );

        repositorioProduto.Excluir(id);

        return Result.Ok();
    }

    public List<ListarProdutoDto> SelecionarTodos()
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return [];

        return repositorioProduto
            .SelecionarTodos()
            .Where(p => p.EstabelecimentoId == estabelecimento.Id)
            .OrderBy(p => p.Nome)
            .Select(p => new ListarProdutoDto(
                p.Id,
                p.Nome,
                p.Preco
            ))
            .ToList();
    }

    public Result<DetalhesProdutoDto> SelecionarPorId(Guid id)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Result.Fail("Estabelecimento não encontrado.");

        Produto? produto = repositorioProduto.SelecionarPorId(id);

        if (produto == null || produto.EstabelecimentoId != estabelecimento.Id)
            return Result.Fail("Produto não encontrado.");

        return Result.Ok(new DetalhesProdutoDto(
            produto.Id,
            produto.Nome,
            produto.Preco
        ));
    }
}