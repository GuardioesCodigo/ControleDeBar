using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Dominio.Modulos.ModuloEstabelecimento;

public interface IRepositorioEstabelecimento : IRepositorio<Estabelecimento>
{
    Estabelecimento? SelecionarDoUsuarioAtual();
}
