using ControleDeBar.Dominio.Compartilhado.Identity;

namespace ControleDeBar.Testes.Integracao.Compartilhado;

// Provedor de usuário controlável em teste: permite simular a troca de
// estabelecimento (usuário) autenticado para validar os Query Filters do EF.
public sealed class ProvedorDeUsuarioFake : IProvedorDeUsuario
{
    public Guid? Id { get; set; }
    public bool EstaAutenticado => Id != null;
}
