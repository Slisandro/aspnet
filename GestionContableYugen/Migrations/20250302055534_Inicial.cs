using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionContableYugen.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CajaChica_Usuarios_UsuarioID",
                table: "CajaChica");

            migrationBuilder.DropForeignKey(
                name: "FK_CierresCaja_CajaChica_CajaChicaMovimientoID",
                table: "CierresCaja");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Distritos_DistritoidDistrito",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_DistritoidDistrito",
                table: "Clientes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CajaChica",
                table: "CajaChica");

            migrationBuilder.DropColumn(
                name: "Contraseña",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "DistritoidDistrito",
                table: "Clientes");

            migrationBuilder.RenameTable(
                name: "CajaChica",
                newName: "Caja_Chica");

            migrationBuilder.RenameIndex(
                name: "IX_CajaChica_UsuarioID",
                table: "Caja_Chica",
                newName: "IX_Caja_Chica_UsuarioID");

            migrationBuilder.AlterColumn<string>(
                name: "NombreUsuario",
                table: "Usuarios",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Contrasena",
                table: "Usuarios",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Caja_Chica",
                table: "Caja_Chica",
                column: "MovimientoID");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_idDistrito",
                table: "Clientes",
                column: "idDistrito");

            migrationBuilder.AddForeignKey(
                name: "FK_Caja_Chica_Usuarios_UsuarioID",
                table: "Caja_Chica",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "UsuarioID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CierresCaja_Caja_Chica_CajaChicaMovimientoID",
                table: "CierresCaja",
                column: "CajaChicaMovimientoID",
                principalTable: "Caja_Chica",
                principalColumn: "MovimientoID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Distritos_idDistrito",
                table: "Clientes",
                column: "idDistrito",
                principalTable: "Distritos",
                principalColumn: "idDistrito",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Caja_Chica_Usuarios_UsuarioID",
                table: "Caja_Chica");

            migrationBuilder.DropForeignKey(
                name: "FK_CierresCaja_Caja_Chica_CajaChicaMovimientoID",
                table: "CierresCaja");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Distritos_idDistrito",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_idDistrito",
                table: "Clientes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Caja_Chica",
                table: "Caja_Chica");

            migrationBuilder.DropColumn(
                name: "Contrasena",
                table: "Usuarios");

            migrationBuilder.RenameTable(
                name: "Caja_Chica",
                newName: "CajaChica");

            migrationBuilder.RenameIndex(
                name: "IX_Caja_Chica_UsuarioID",
                table: "CajaChica",
                newName: "IX_CajaChica_UsuarioID");

            migrationBuilder.AlterColumn<string>(
                name: "NombreUsuario",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Contraseña",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DistritoidDistrito",
                table: "Clientes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CajaChica",
                table: "CajaChica",
                column: "MovimientoID");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_DistritoidDistrito",
                table: "Clientes",
                column: "DistritoidDistrito");

            migrationBuilder.AddForeignKey(
                name: "FK_CajaChica_Usuarios_UsuarioID",
                table: "CajaChica",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "UsuarioID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CierresCaja_CajaChica_CajaChicaMovimientoID",
                table: "CierresCaja",
                column: "CajaChicaMovimientoID",
                principalTable: "CajaChica",
                principalColumn: "MovimientoID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Distritos_DistritoidDistrito",
                table: "Clientes",
                column: "DistritoidDistrito",
                principalTable: "Distritos",
                principalColumn: "idDistrito",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
