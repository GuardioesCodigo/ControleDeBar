using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeBar.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaEstabelecimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBGarcom_TBEstabelecimento",
                table: "TBGarcom");

            migrationBuilder.DropForeignKey(
                name: "FK_TBMesa_TBEstabelecimento",
                table: "TBMesa");

            migrationBuilder.DropForeignKey(
                name: "FK_TBProduto_TBEstabelecimento",
                table: "TBProduto");

            migrationBuilder.DropIndex(
                name: "IX_TBProduto_EstabelecimentoId",
                table: "TBProduto");

            migrationBuilder.DropIndex(
                name: "IX_TBGarcom_EstabelecimentoId",
                table: "TBGarcom");

            migrationBuilder.RenameColumn(
                name: "EstabelecimentoId",
                table: "TBProduto",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "EstabelecimentoId",
                table: "TBMesa",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "UQ_TBMesa_EstabelecimentoId_Numero",
                table: "TBMesa",
                newName: "UQ_TBMesa_UserId_Numero");

            migrationBuilder.RenameColumn(
                name: "EstabelecimentoId",
                table: "TBGarcom",
                newName: "UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "TBPedido",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "TBConta",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TBConta_UserId_Situacao",
                table: "TBConta",
                columns: new[] { "UserId", "Situacao" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TBConta_UserId_Situacao",
                table: "TBConta");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TBPedido");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TBConta");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TBProduto",
                newName: "EstabelecimentoId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TBMesa",
                newName: "EstabelecimentoId");

            migrationBuilder.RenameIndex(
                name: "UQ_TBMesa_UserId_Numero",
                table: "TBMesa",
                newName: "UQ_TBMesa_EstabelecimentoId_Numero");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TBGarcom",
                newName: "EstabelecimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBProduto_EstabelecimentoId",
                table: "TBProduto",
                column: "EstabelecimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBGarcom_EstabelecimentoId",
                table: "TBGarcom",
                column: "EstabelecimentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBGarcom_TBEstabelecimento",
                table: "TBGarcom",
                column: "EstabelecimentoId",
                principalTable: "TBEstabelecimento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TBMesa_TBEstabelecimento",
                table: "TBMesa",
                column: "EstabelecimentoId",
                principalTable: "TBEstabelecimento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TBProduto_TBEstabelecimento",
                table: "TBProduto",
                column: "EstabelecimentoId",
                principalTable: "TBEstabelecimento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
