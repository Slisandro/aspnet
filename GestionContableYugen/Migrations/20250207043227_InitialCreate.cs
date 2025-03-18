using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionContableYugen.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          /*  migrationBuilder.CreateTable(
                name: "ActivosFijos",
                columns: table => new
                {
                    ActivoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreActivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaAdquisicion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValorInicial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VidaUtil = table.Column<int>(type: "int", nullable: false),
                    ValorDepreciado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivosFijos", x => x.ActivoID);
                });
          */
            migrationBuilder.CreateTable(
                name: "ConceptosCajaChica",
                columns: table => new
                {
                    idConcepto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConceptosCajaChica", x => x.idConcepto);
                });

            migrationBuilder.CreateTable(
                name: "Correos",
                columns: table => new
                {
                    idCorreo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DireccionCorreo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonaID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Correos", x => x.idCorreo);
                });

            migrationBuilder.CreateTable(
                name: "Cuentas",
                columns: table => new
                {
                    CuentaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoCuenta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoCuenta = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuentas", x => x.CuentaID);
                });

            migrationBuilder.CreateTable(
                name: "Descuentos",
                columns: table => new
                {
                    idDescuento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Porcentaje = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Descuentos", x => x.idDescuento);
                });

            migrationBuilder.CreateTable(
                name: "Impuestos",
                columns: table => new
                {
                    idImpuesto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Porcentaje = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Impuestos", x => x.idImpuesto);
                });

            migrationBuilder.CreateTable(
                name: "LibrosMayor",
                columns: table => new
                {
                    idLibroMayor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalDebe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalHaber = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaCorte = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibrosMayor", x => x.idLibroMayor);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    ProductoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreProducto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoBarras = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CantidadDisponible = table.Column<int>(type: "int", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.ProductoID);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    ProveedorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreProveedor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.ProveedorID);
                });

            migrationBuilder.CreateTable(
                name: "Provincias",
                columns: table => new
                {
                    idProvincia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreProvincia = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincias", x => x.idProvincia);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RolID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreRol = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RolID);
                });

            migrationBuilder.CreateTable(
                name: "Telefonos",
                columns: table => new
                {
                    idTelefono = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroTelefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoTelefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonaID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telefonos", x => x.idTelefono);
                });

            migrationBuilder.CreateTable(
                name: "TiposCorreo",
                columns: table => new
                {
                    idTipoCorreo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposCorreo", x => x.idTipoCorreo);
                });

            migrationBuilder.CreateTable(
                name: "TiposFactura",
                columns: table => new
                {
                    idTipoFactura = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposFactura", x => x.idTipoFactura);
                });

            migrationBuilder.CreateTable(
                name: "TiposPago",
                columns: table => new
                {
                    idTipoPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPago", x => x.idTipoPago);
                });

            migrationBuilder.CreateTable(
                name: "TiposTelefono",
                columns: table => new
                {
                    idTipoTelefono = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposTelefono", x => x.idTipoTelefono);
                });

            migrationBuilder.CreateTable(
                name: "AsientosContables",
                columns: table => new
                {
                    AsientoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Debe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Haber = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CuentaID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsientosContables", x => x.AsientoID);
                    table.ForeignKey(
                        name: "FK_AsientosContables_Cuentas_CuentaID",
                        column: x => x.CuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "CuentaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuentasPorPagar",
                columns: table => new
                {
                    CuentaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProveedorID = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasPorPagar", x => x.CuentaID);
                    table.ForeignKey(
                        name: "FK_CuentasPorPagar_Proveedores_ProveedorID",
                        column: x => x.ProveedorID,
                        principalTable: "Proveedores",
                        principalColumn: "ProveedorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cantones",
                columns: table => new
                {
                    idCanton = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCanton = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    idProvincia = table.Column<int>(type: "int", nullable: false),
                    ProvinciaidProvincia = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cantones", x => x.idCanton);
                    table.ForeignKey(
                        name: "FK_Cantones_Provincias_ProvinciaidProvincia",
                        column: x => x.ProvinciaidProvincia,
                        principalTable: "Provincias",
                        principalColumn: "idProvincia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contraseña = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RolID = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioID);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolID",
                        column: x => x.RolID,
                        principalTable: "Roles",
                        principalColumn: "RolID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LibrosDiario",
                columns: table => new
                {
                    idLibroDiario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AsientoContableID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibrosDiario", x => x.idLibroDiario);
                    table.ForeignKey(
                        name: "FK_LibrosDiario_AsientosContables_AsientoContableID",
                        column: x => x.AsientoContableID,
                        principalTable: "AsientosContables",
                        principalColumn: "AsientoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Distritos",
                columns: table => new
                {
                    idDistrito = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreDistrito = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    idCanton = table.Column<int>(type: "int", nullable: false),
                    CantonidCanton = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distritos", x => x.idDistrito);
                    table.ForeignKey(
                        name: "FK_Distritos_Cantones_CantonidCanton",
                        column: x => x.CantonidCanton,
                        principalTable: "Cantones",
                        principalColumn: "idCanton",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CajaChica",
                columns: table => new
                {
                    MovimientoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CajaChica", x => x.MovimientoID);
                    table.ForeignKey(
                        name: "FK_CajaChica_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ClienteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    idDistrito = table.Column<int>(type: "int", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DistritoidDistrito = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ClienteID);
                    table.ForeignKey(
                        name: "FK_Clientes_Distritos_DistritoidDistrito",
                        column: x => x.DistritoidDistrito,
                        principalTable: "Distritos",
                        principalColumn: "idDistrito",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CierresCaja",
                columns: table => new
                {
                    idCierreCaja = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    idCaja = table.Column<int>(type: "int", nullable: false),
                    CajaChicaMovimientoID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CierresCaja", x => x.idCierreCaja);
                    table.ForeignKey(
                        name: "FK_CierresCaja_CajaChica_CajaChicaMovimientoID",
                        column: x => x.CajaChicaMovimientoID,
                        principalTable: "CajaChica",
                        principalColumn: "MovimientoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuentasPorCobrar",
                columns: table => new
                {
                    CuentaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteID = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasPorCobrar", x => x.CuentaID);
                    table.ForeignKey(
                        name: "FK_CuentasPorCobrar_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ClienteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Facturas",
                columns: table => new
                {
                    FacturaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    ClienteID = table.Column<int>(type: "int", nullable: false),
                    idTipoFactura = table.Column<int>(type: "int", nullable: false),
                    idTipoPago = table.Column<int>(type: "int", nullable: false),
                    FechaFactura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Impuesto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TipoFacturaidTipoFactura = table.Column<int>(type: "int", nullable: false),
                    TipoPagoidTipoPago = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facturas", x => x.FacturaID);
                    table.ForeignKey(
                        name: "FK_Facturas_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ClienteID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Facturas_TiposFactura_TipoFacturaidTipoFactura",
                        column: x => x.TipoFacturaidTipoFactura,
                        principalTable: "TiposFactura",
                        principalColumn: "idTipoFactura",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Facturas_TiposPago_TipoPagoidTipoPago",
                        column: x => x.TipoPagoidTipoPago,
                        principalTable: "TiposPago",
                        principalColumn: "idTipoPago",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Facturas_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesFactura",
                columns: table => new
                {
                    DetalleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacturaID = table.Column<int>(type: "int", nullable: false),
                    ProductoID = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesFactura", x => x.DetalleID);
                    table.ForeignKey(
                        name: "FK_DetallesFactura_Facturas_FacturaID",
                        column: x => x.FacturaID,
                        principalTable: "Facturas",
                        principalColumn: "FacturaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesFactura_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ProductoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsientosContables_CuentaID",
                table: "AsientosContables",
                column: "CuentaID");

            migrationBuilder.CreateIndex(
                name: "IX_CajaChica_UsuarioID",
                table: "CajaChica",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Cantones_ProvinciaidProvincia",
                table: "Cantones",
                column: "ProvinciaidProvincia");

            migrationBuilder.CreateIndex(
                name: "IX_CierresCaja_CajaChicaMovimientoID",
                table: "CierresCaja",
                column: "CajaChicaMovimientoID");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_DistritoidDistrito",
                table: "Clientes",
                column: "DistritoidDistrito");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorCobrar_ClienteID",
                table: "CuentasPorCobrar",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorPagar_ProveedorID",
                table: "CuentasPorPagar",
                column: "ProveedorID");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesFactura_FacturaID",
                table: "DetallesFactura",
                column: "FacturaID");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesFactura_ProductoID",
                table: "DetallesFactura",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_Distritos_CantonidCanton",
                table: "Distritos",
                column: "CantonidCanton");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_ClienteID",
                table: "Facturas",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_TipoFacturaidTipoFactura",
                table: "Facturas",
                column: "TipoFacturaidTipoFactura");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_TipoPagoidTipoPago",
                table: "Facturas",
                column: "TipoPagoidTipoPago");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_UsuarioID",
                table: "Facturas",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_LibrosDiario_AsientoContableID",
                table: "LibrosDiario",
                column: "AsientoContableID");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolID",
                table: "Usuarios",
                column: "RolID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivosFijos");

            migrationBuilder.DropTable(
                name: "CierresCaja");

            migrationBuilder.DropTable(
                name: "ConceptosCajaChica");

            migrationBuilder.DropTable(
                name: "Correos");

            migrationBuilder.DropTable(
                name: "CuentasPorCobrar");

            migrationBuilder.DropTable(
                name: "CuentasPorPagar");

            migrationBuilder.DropTable(
                name: "Descuentos");

            migrationBuilder.DropTable(
                name: "DetallesFactura");

            migrationBuilder.DropTable(
                name: "Impuestos");

            migrationBuilder.DropTable(
                name: "LibrosDiario");

            migrationBuilder.DropTable(
                name: "LibrosMayor");

            migrationBuilder.DropTable(
                name: "Telefonos");

            migrationBuilder.DropTable(
                name: "TiposCorreo");

            migrationBuilder.DropTable(
                name: "TiposTelefono");

            migrationBuilder.DropTable(
                name: "CajaChica");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "Facturas");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "AsientosContables");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "TiposFactura");

            migrationBuilder.DropTable(
                name: "TiposPago");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Cuentas");

            migrationBuilder.DropTable(
                name: "Distritos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Cantones");

            migrationBuilder.DropTable(
                name: "Provincias");
        }
    }
}
