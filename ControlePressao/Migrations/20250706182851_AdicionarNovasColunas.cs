using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlePressao.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarNovasColunas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "Pressao",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "Glicose",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Periodo",
                table: "Glicose",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "Pressao");

            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "Glicose");

            migrationBuilder.DropColumn(
                name: "Periodo",
                table: "Glicose");
        }
    }
}
