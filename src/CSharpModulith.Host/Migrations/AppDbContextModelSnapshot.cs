using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CSharpModulith.Host.Migrations;

[DbContext(typeof(AppDbContext))]
public sealed class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.5")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("App.Capability.Todos.Infrastructure.Persistence.EfCore.TodoItemEntity", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<bool>("IsCompleted")
                    .HasColumnType("boolean");

                b.Property<int>("SortOrder")
                    .HasColumnType("integer");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnType("character varying(500)");

                b.Property<Guid>("TodoListId")
                    .HasColumnType("uuid");

                b.HasKey("Id");

                b.HasIndex("TodoListId", "SortOrder");

                b.ToTable("todo_items", (string)null);

                b.HasOne("App.Capability.Todos.Infrastructure.Persistence.EfCore.TodoListEntity", "List")
                    .WithMany("Items")
                    .HasForeignKey("TodoListId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("List");
            });

        modelBuilder.Entity("App.Capability.Todos.Infrastructure.Persistence.EfCore.TodoListEntity", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnType("character varying(500)");

                b.HasKey("Id");

                b.ToTable("todo_lists", (string)null);

                b.Navigation("Items");
            });
#pragma warning restore 612, 618
    }
}
