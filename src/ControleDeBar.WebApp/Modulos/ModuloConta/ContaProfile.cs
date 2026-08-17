using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloConta;

namespace ControleDeBar.WebApp.Modulos.ModuloConta;

public class ContaProfile : Profile
{
    public ContaProfile()
    {
        CreateMap<ListarContaDto, ListarContaViewModel>();
        CreateMap<AbrirContaViewModel, CadastrarContaDto>();
        CreateMap<EditarContaViewModel, EditarContaDto>();
        CreateMap<ItemPedidoContaDto, ItemPedidoViewModel>();
        CreateMap<DetalhesContaDto, VisualizarContaViewModel>();
        CreateMap<DetalhesContaDto, EditarContaViewModel>();
    }
}
