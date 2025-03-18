using AutoMapper;
using GestionContableYugen.Models;
using GestionContableYugen.ViewModels;

namespace GestionContableYugen.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Usuario, UsuarioViewModel>().ReverseMap();
            CreateMap<Cliente, ClienteViewModel>().ReverseMap();
            CreateMap<CajaChica, CajaChicaViewModel>().ReverseMap();
            CreateMap<Proveedor, ProveedorViewModel>().ReverseMap();
            CreateMap<CuentaPorPagarViewModel, CuentaPorPagar>().ReverseMap();
            CreateMap<CuentaPorCobrarViewModel, CuentaPorCobrar>().ReverseMap();
            CreateMap<ProductoViewModel, Producto>().ReverseMap();
            CreateMap<ActivoFijo, ActivoFijoViewModel>().ReverseMap();

        }

    }
}
