using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloPedido;

namespace ControleDeBar.WebApp.Modulos.ModuloPedido;

public class PedidoProfile : Profile
{
    public PedidoProfile()
    {
        CreateMap<AdicionarPedidoViewModel, CadastrarPedidoDto>();
    }
}
