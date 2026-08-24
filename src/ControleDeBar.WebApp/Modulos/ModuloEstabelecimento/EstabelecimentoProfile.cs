using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloEstabelecimento;

namespace ControleDeBar.WebApp.Modulos.ModuloEstabelecimento;

public class EstabelecimentoProfile : Profile
{
    public EstabelecimentoProfile()
    {
        CreateMap<DetalhesEstabelecimentoDto, DetalhesEstabelecimentoViewModel>();
        CreateMap<DetalhesEstabelecimentoDto, EditarEstabelecimentoViewModel>();
        CreateMap<EditarEstabelecimentoViewModel, EditarEstabelecimentoDto>();
    }
}