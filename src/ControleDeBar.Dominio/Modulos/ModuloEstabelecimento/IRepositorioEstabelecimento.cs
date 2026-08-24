using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Dominio.Modulos.ModuloEstabelecimento;

public interface IRepositorioEstabelecimento : IRepositorio<Estabelecimento>
{
    // O Query Filter global já restringe a consulta ao estabelecimento do usuário
    // autenticado, então basta pegar o único registro existente no contexto.
    Estabelecimento? SelecionarDoUsuarioAtual();
}
