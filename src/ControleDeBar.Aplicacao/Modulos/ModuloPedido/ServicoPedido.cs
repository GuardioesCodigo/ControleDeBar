using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloEstabelecimento;
using ControleDeBar.Dominio.Modulos.ModuloPedido;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using FluentResults;

namespace ControleDeBar.Aplicacao.Modulos.ModuloPedido;

public sealed class ServicoPedido(
    IRepositorioPedido repositorioPedido,
    IRepositorioConta repositorioConta,
    IRepositorioProduto repositorioProduto,
    IRepositorioEstabelecimento repositorioEstabelecimento
) : ServicoBase<Pedido>
{
    public Result Registrar(CadastrarPedidoDto dto)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Falha(string.Empty, "Estabelecimento não encontrado.");

        Conta? conta = repositorioConta.SelecionarPorId(dto.ContaId);

        if (conta == null || conta.Mesa.EstabelecimentoId != estabelecimento.Id)
            return Falha(nameof(dto.ContaId), "Conta não encontrada.");

        if (conta.Situacao == SituacaoConta.Fechada)
            return Falha(
                string.Empty,
                "Não é possível registrar pedidos em uma conta fechada."
            );

        Produto? produto = repositorioProduto.SelecionarPorId(dto.ProdutoId);

        if (produto == null || produto.EstabelecimentoId != estabelecimento.Id)
            return Falha(nameof(dto.ProdutoId), "Produto não encontrado.");

        Pedido pedido = new()
        {
            ContaId = dto.ContaId,
            ProdutoId = dto.ProdutoId,
            Quantidade = dto.Quantidade
        };

        Result resultadoValidacao = ValidarEntidade(pedido);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioPedido.Cadastrar(pedido);

        return Result.Ok();
    }

    public Result Remover(Guid id)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Falha(string.Empty, "Estabelecimento não encontrado.");

        Pedido? pedido = repositorioPedido.SelecionarPorId(id);

        if (pedido == null ||
            pedido.Produto.EstabelecimentoId != estabelecimento.Id)
            return Falha(string.Empty, "Pedido não encontrado.");

        repositorioPedido.Excluir(id);

        return Result.Ok();
    }

    public Result<List<ListarPedidoDto>> SelecionarPorContaId(Guid contaId)
    {
        Estabelecimento? estabelecimento =
            repositorioEstabelecimento.SelecionarDoUsuarioAtual();

        if (estabelecimento == null)
            return Result.Fail("Estabelecimento não encontrado.");

        Conta? conta = repositorioConta.SelecionarPorId(contaId);

        if (conta == null || conta.Mesa.EstabelecimentoId != estabelecimento.Id)
            return Result.Fail("Conta não encontrada.");

        List<ListarPedidoDto> pedidos = repositorioPedido
            .SelecionarPorContaId(contaId)
            .Select(p => new ListarPedidoDto(
                p.Id,
                p.Produto.Nome,
                p.Quantidade,
                p.Subtotal
            ))
            .ToList();

        return Result.Ok(pedidos);
    }
}