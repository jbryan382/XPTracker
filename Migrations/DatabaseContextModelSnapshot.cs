﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using XPTracker.Models;

namespace XPTracker.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("XPTracker.Models.SessionTrackerModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CombatXP")
                        .HasColumnType("integer");

                    b.Property<int>("ExplorationXP")
                        .HasColumnType("integer");

                    b.Property<string>("SessionDescription")
                        .HasColumnType("text");

                    b.Property<int>("SocialInteractionXP")
                        .HasColumnType("integer");

                    b.Property<int>("XPId")
                        .HasColumnType("integer");

                    b.Property<int?>("XPTrackerModelId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("XPTrackerModelId");

                    b.ToTable("SessionTracker");
                });

            modelBuilder.Entity("XPTracker.Models.XPTrackerModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("TotalLevel")
                        .HasColumnType("integer");

                    b.Property<int>("TotalXP")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("XPTracker");
                });

            modelBuilder.Entity("XPTracker.Models.SessionTrackerModel", b =>
                {
                    b.HasOne("XPTracker.Models.XPTrackerModel", "XPTrackerModel")
                        .WithMany("Sessions")
                        .HasForeignKey("XPTrackerModelId");
                });
#pragma warning restore 612, 618
        }
    }
}
