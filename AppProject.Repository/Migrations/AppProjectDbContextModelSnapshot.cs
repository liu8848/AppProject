﻿// <auto-generated />
using AppProject.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AppProject.Repository.Migrations
{
    [DbContext(typeof(AppProjectDbContext))]
    partial class AppProjectDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("AppProject.Model.TestModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<string>("Desc")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("desc");

                    b.HasKey("id");

                    b.ToTable("test_model");
                });
#pragma warning restore 612, 618
        }
    }
}