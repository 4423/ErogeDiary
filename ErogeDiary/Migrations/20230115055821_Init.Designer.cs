﻿// <auto-generated />
using System;
using ErogeDiary.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ErogeDiary.Migrations
{
    [DbContext(typeof(ErogeDiaryDbContext))]
    [Migration("20230115055821_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true);

            modelBuilder.Entity("ErogeDiary.Models.Database.Entities.Game", b =>
                {
                    b.Property<int>("GameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ClearedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("ErogameScapeGameId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExecutableFilePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageFileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("InstallationType")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastPlayedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("RegisteredAt")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("ReleaseDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan>("TotalPlayTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("WindowTitle")
                        .HasColumnType("TEXT");

                    b.HasKey("GameId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("ErogeDiary.Models.Database.Entities.PlayLog", b =>
                {
                    b.Property<int>("PlayLogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EndedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan>("PlayTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("PlayLogId");

                    b.HasIndex("GameId");

                    b.ToTable("PlayLogs");
                });

            modelBuilder.Entity("ErogeDiary.Models.Database.Entities.Root", b =>
                {
                    b.Property<int>("RootId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("ClearedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan>("PlayTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("RootId");

                    b.HasIndex("GameId");

                    b.ToTable("Roots");
                });

            modelBuilder.Entity("ErogeDiary.Models.Database.Entities.PlayLog", b =>
                {
                    b.HasOne("ErogeDiary.Models.Database.Entities.Game", "Game")
                        .WithMany("PlayLogs")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("ErogeDiary.Models.Database.Entities.Root", b =>
                {
                    b.HasOne("ErogeDiary.Models.Database.Entities.Game", "Game")
                        .WithMany("Roots")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("ErogeDiary.Models.Database.Entities.Game", b =>
                {
                    b.Navigation("PlayLogs");

                    b.Navigation("Roots");
                });
#pragma warning restore 612, 618
        }
    }
}