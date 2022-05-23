﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TodoApp;

#nullable disable

namespace TodoApp.Migrations
{
    [DbContext(typeof(TodoAppContext))]
    partial class TodoAppContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TodoApp.Domain.Aggregates.Identity.TodoUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("LoginName")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TodoUser", (string)null);
                });

            modelBuilder.Entity("TodoApp.Domain.Aggregates.Todo.TodoItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AuthorId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreationTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<string>("Detail")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModificationTime")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("current_timestamp");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("TodoItem", (string)null);
                });

            modelBuilder.Entity("TodoApp.Domain.Aggregates.Identity.TodoUser", b =>
                {
                    b.OwnsOne("TodoApp.Domain.Aggregates.Identity.UserName", "Name", b1 =>
                        {
                            b1.Property<int>("TodoUserId")
                                .HasColumnType("integer");

                            b1.Property<string>("FirstName")
                                .HasColumnType("text");

                            b1.Property<string>("LastName")
                                .HasColumnType("text");

                            b1.HasKey("TodoUserId");

                            b1.ToTable("TodoUser");

                            b1.WithOwner()
                                .HasForeignKey("TodoUserId");
                        });

                    b.Navigation("Name");
                });
#pragma warning restore 612, 618
        }
    }
}
