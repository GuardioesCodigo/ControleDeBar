using ControleDeBar.Dominio.Compartilhado.Identity;

namespace ControleDeBar.Dominio.Compartilhado;

public abstract class EntidadeBase<T> : IEntidadeDoUsuario
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    // Id do usuário dono do estabelecimento ao qual o registro pertence.
    // Usado pelos Query Filters do EF Core para segregar os dados por estabelecimento.
    public Guid UserId { get; set; } = Guid.Empty;

    public abstract List<string> Validar();
    public abstract void Atualizar(T entidadeAtualizada);
}
