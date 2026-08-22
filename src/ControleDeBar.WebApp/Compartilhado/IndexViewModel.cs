namespace ControleDeBar.WebApp.Compartilhado;

public record IndexViewModel(
    string NomeEstabelecimento,
    int ContasAbertas,
    decimal FaturamentoDoDia,
    int MesasOcupadas,
    int TotalMesas
);
