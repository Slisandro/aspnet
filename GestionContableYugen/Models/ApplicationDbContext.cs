using Microsoft.EntityFrameworkCore;
using GestionContableYugen.Models;  


namespace GestionContableYugen.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // DbSets para todas las tablas de la base de datos
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Canton> Cantones { get; set; }
        public DbSet<Distrito> Distritos { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<TipoFactura> TiposFactura { get; set; }
        public DbSet<TipoPago> TipoPago { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<DetalleFactura> DetalleFactura { get; set; }
        public DbSet<CuentaPorCobrar> CuentasPorCobrar { get; set; }
        public DbSet<CuentaPorPagar> CuentasPorPagar { get; set; }
        public DbSet<CajaChica> Caja_Chica { get; set; }
        public DbSet<CierreCaja> CierresCaja { get; set; }
        public DbSet<Inventario> Inventario { get; set; }
        public DbSet<Descuento> Descuentos { get; set; }
        public DbSet<ConceptoCajaChica> ConceptosCajaChica { get; set; }
        public DbSet<TipoCorreo> TiposCorreo { get; set; }
        public DbSet<Correo> Correos { get; set; }
        public DbSet<Telefono> Telefonos { get; set; }
        public DbSet<TipoTelefono> TiposTelefono { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<AsientoContable> AsientosContables { get; set; }
        public DbSet<LibroDiario> LibrosDiario { get; set; }
        public DbSet<LibroMayor> LibrosMayor { get; set; }
        public DbSet<ActivoFijo> ActivosFijos { get; set; }
        public DbSet<Impuesto> Impuestos { get; set; }
        public DbSet<MovimientoInventario> MovimientoInventario { get; set; }

        public DbSet<CajaChicaConfig> CajaChicaConfig { get; set; }



    }
}

