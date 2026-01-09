using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecursosHumanos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CascadaPaisDepartamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departamentos_Paises_PaisId",
                table: "Departamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Municipios_Departamentos_DepartamentoId",
                table: "Municipios");

            migrationBuilder.AddForeignKey(
                name: "FK_Departamentos_Paises_PaisId",
                table: "Departamentos",
                column: "PaisId",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Municipios_Departamentos_DepartamentoId",
                table: "Municipios",
                column: "DepartamentoId",
                principalTable: "Departamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departamentos_Paises_PaisId",
                table: "Departamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Municipios_Departamentos_DepartamentoId",
                table: "Municipios");

            migrationBuilder.AddForeignKey(
                name: "FK_Departamentos_Paises_PaisId",
                table: "Departamentos",
                column: "PaisId",
                principalTable: "Paises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Municipios_Departamentos_DepartamentoId",
                table: "Municipios",
                column: "DepartamentoId",
                principalTable: "Departamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
