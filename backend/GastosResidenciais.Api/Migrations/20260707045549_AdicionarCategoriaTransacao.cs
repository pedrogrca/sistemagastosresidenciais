using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GastosResidenciais.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCategoriaTransacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Transacoes",
                type: "TEXT",
                maxLength: 30,
                nullable: false,
                defaultValue: "Outros");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Transacoes");
        }
    }
}
