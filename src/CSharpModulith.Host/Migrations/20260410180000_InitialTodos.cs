using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSharpModulith.Host.Migrations;

/// <inheritdoc />
[Migration("20260410180000_InitialTodos")]
public class InitialTodos : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "todo_lists",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_todo_lists", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "todo_items",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TodoListId = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                SortOrder = table.Column<int>(type: "integer", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_todo_items", x => x.Id);
                table.ForeignKey(
                    name: "FK_todo_items_todo_lists_TodoListId",
                    column: x => x.TodoListId,
                    principalTable: "todo_lists",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_todo_items_TodoListId_SortOrder",
            table: "todo_items",
            columns: new[] { "TodoListId", "SortOrder" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "todo_items");
        migrationBuilder.DropTable(name: "todo_lists");
    }
}
