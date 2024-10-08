﻿// <auto-generated />
using System;
using DataLayer.Model_DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataLayer.Migrations
{
    [DbContext(typeof(Blog_DB))]
    [Migration("20240821082656_initBlog")]
    partial class initBlog
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DataLayer.Model_Blog.Kategori", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("MakaleId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MakaleId");

                    b.ToTable("kategoris");
                });

            modelBuilder.Entity("DataLayer.Model_Blog.Makale", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Baslik")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Icerik")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("makales");
                });

            modelBuilder.Entity("DataLayer.Model_Blog.Resim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("MakaleId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MakaleId");

                    b.ToTable("resims");
                });

            modelBuilder.Entity("DataLayer.Model_Blog.Yazar", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("MakaleId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MakaleId");

                    b.ToTable("yazars");
                });

            modelBuilder.Entity("DataLayer.Model_Kullanicilar.Kullanici", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("kullanicis");
                });

            modelBuilder.Entity("DataLayer.Model_Blog.Kategori", b =>
                {
                    b.HasOne("DataLayer.Model_Blog.Makale", null)
                        .WithMany("kategori")
                        .HasForeignKey("MakaleId");
                });

            modelBuilder.Entity("DataLayer.Model_Blog.Resim", b =>
                {
                    b.HasOne("DataLayer.Model_Blog.Makale", null)
                        .WithMany("resim")
                        .HasForeignKey("MakaleId");
                });

            modelBuilder.Entity("DataLayer.Model_Blog.Yazar", b =>
                {
                    b.HasOne("DataLayer.Model_Blog.Makale", null)
                        .WithMany("yazar")
                        .HasForeignKey("MakaleId");
                });

            modelBuilder.Entity("DataLayer.Model_Blog.Makale", b =>
                {
                    b.Navigation("kategori");

                    b.Navigation("resim");

                    b.Navigation("yazar");
                });
#pragma warning restore 612, 618
        }
    }
}
