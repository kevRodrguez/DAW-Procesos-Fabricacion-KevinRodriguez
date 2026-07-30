using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAW_Procesos_Fabricacion_KevinRodriguez.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrdenesProduccion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroOrden = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, collation: "NOCASE"),
                    ModeloCalzado = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    FechaEntregaEstimada = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesProduccion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcesosFabricacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, collation: "NOCASE"),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcesosFabricacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesProcesos",
                columns: table => new
                {
                    OrdenProduccionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProcesoFabricacionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "Pendiente"),
                    FechaCompletado = table.Column<DateOnly>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesProcesos", x => new { x.OrdenProduccionId, x.ProcesoFabricacionId });
                    table.ForeignKey(
                        name: "FK_OrdenesProcesos_OrdenesProduccion_OrdenProduccionId",
                        column: x => x.OrdenProduccionId,
                        principalTable: "OrdenesProduccion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesProcesos_ProcesosFabricacion_ProcesoFabricacionId",
                        column: x => x.ProcesoFabricacionId,
                        principalTable: "ProcesosFabricacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesProcesos_ProcesoFabricacionId",
                table: "OrdenesProcesos",
                column: "ProcesoFabricacionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesProduccion_NumeroOrden",
                table: "OrdenesProduccion",
                column: "NumeroOrden",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcesosFabricacion_Nombre",
                table: "ProcesosFabricacion",
                column: "Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdenesProcesos");

            migrationBuilder.DropTable(
                name: "OrdenesProduccion");

            migrationBuilder.DropTable(
                name: "ProcesosFabricacion");
        }
    }
}
