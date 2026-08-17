using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using FluentResults;

namespace ControleDeBar.Aplicacao.Modulos.ModuloProduto;

public sealed class ServicoProduto(
    IRepositorioProduto repositorioProduto
) : ServicoBase<Produto>
{
    public Result Cadastrar(CadastrarProdutoDto dto)
    {
        Produto produto = new() { Nome = dto.Nome, Preco = dto.Preco };

        Result resultadoValidacao = ValidarEntidade(produto);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioProduto.Cadastrar(produto);

        return Result.Ok();
    }

    public Result Editar(EditarProdutoDto dto)
    {
        Produto? produto = repositorioProduto.SelecionarPorId(dto.Id);

        if (produto == null)
            return Falha(string.Empty, "Produto não encontrado.");

        Produto atualizado = new() { Nome = dto.Nome, Preco = dto.Preco };

        Result resultadoValidacao = ValidarEntidade(atualizado);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioProduto.Editar(dto.Id, atualizado);

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Produto? produto = repositorioProduto.SelecionarPorId(id);

        if (produto == null)
            return Falha(string.Empty, "Produto não encontrado.");

        if (repositorioProduto.PossuiPedidoVinculado(id))
            return Falha(string.Empty, "Não é possível excluir um produto com pedidos vinculados.");

        repositorioProduto.Excluir(id);

        return Result.Ok();
    }

    public List<ListarProdutoDto> SelecionarTodos()
    {
        return repositorioProduto
            .SelecionarTodos()
            .OrderBy(p => p.Nome)
            .Select(p => new ListarProdutoDto(p.Id, p.Nome, p.Preco))
            .ToList();
    }

    public Result<DetalhesProdutoDto> SelecionarPorId(Guid id)
    {
        Produto? produto = repositorioProduto.SelecionarPorId(id);

        if (produto == null)
            return Result.Fail("Produto não encontrado.");

        return Result.Ok(new DetalhesProdutoDto(produto.Id, produto.Nome, produto.Preco));
    }
}
