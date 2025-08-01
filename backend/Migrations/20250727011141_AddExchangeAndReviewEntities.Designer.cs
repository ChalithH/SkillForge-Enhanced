﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillForge.Api.Data;

#nullable disable

namespace SkillForge.Api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250727011141_AddExchangeAndReviewEntities")]
    partial class AddExchangeAndReviewEntities
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SkillForge.Api.Models.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Comment")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<int>("ExchangeId")
                        .HasColumnType("int");

                    b.Property<int>("Rating")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<int>("ReviewedUserId")
                        .HasColumnType("int");

                    b.Property<int>("ReviewerId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ExchangeId");

                    b.HasIndex("ReviewedUserId");

                    b.HasIndex("ReviewerId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("SkillForge.Api.Models.Skill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Skills");
                });

            modelBuilder.Entity("SkillForge.Api.Models.SkillExchange", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<double>("Duration")
                        .HasColumnType("float");

                    b.Property<int>("LearnerId")
                        .HasColumnType("int");

                    b.Property<string>("MeetingLink")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Notes")
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<int>("OffererId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ScheduledAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("SkillId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.HasKey("Id");

                    b.HasIndex("LearnerId");

                    b.HasIndex("OffererId");

                    b.HasIndex("SkillId");

                    b.ToTable("SkillExchanges");
                });

            modelBuilder.Entity("SkillForge.Api.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Bio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfileImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TimeCredits")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(5);

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SkillForge.Api.Models.UserSkill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsOffering")
                        .HasColumnType("bit");

                    b.Property<int>("ProficiencyLevel")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<int>("SkillId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SkillId");

                    b.HasIndex("UserId");

                    b.ToTable("UserSkills");
                });

            modelBuilder.Entity("SkillForge.Api.Models.Review", b =>
                {
                    b.HasOne("SkillForge.Api.Models.SkillExchange", "Exchange")
                        .WithMany("Reviews")
                        .HasForeignKey("ExchangeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SkillForge.Api.Models.User", "ReviewedUser")
                        .WithMany("ReviewsReceived")
                        .HasForeignKey("ReviewedUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SkillForge.Api.Models.User", "Reviewer")
                        .WithMany("ReviewsGiven")
                        .HasForeignKey("ReviewerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Exchange");

                    b.Navigation("ReviewedUser");

                    b.Navigation("Reviewer");
                });

            modelBuilder.Entity("SkillForge.Api.Models.SkillExchange", b =>
                {
                    b.HasOne("SkillForge.Api.Models.User", "Learner")
                        .WithMany("LearnedExchanges")
                        .HasForeignKey("LearnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SkillForge.Api.Models.User", "Offerer")
                        .WithMany("OfferedExchanges")
                        .HasForeignKey("OffererId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SkillForge.Api.Models.Skill", "Skill")
                        .WithMany()
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Learner");

                    b.Navigation("Offerer");

                    b.Navigation("Skill");
                });

            modelBuilder.Entity("SkillForge.Api.Models.UserSkill", b =>
                {
                    b.HasOne("SkillForge.Api.Models.Skill", "Skill")
                        .WithMany("UserSkills")
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SkillForge.Api.Models.User", "User")
                        .WithMany("UserSkills")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Skill");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SkillForge.Api.Models.Skill", b =>
                {
                    b.Navigation("UserSkills");
                });

            modelBuilder.Entity("SkillForge.Api.Models.SkillExchange", b =>
                {
                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("SkillForge.Api.Models.User", b =>
                {
                    b.Navigation("LearnedExchanges");

                    b.Navigation("OfferedExchanges");

                    b.Navigation("ReviewsGiven");

                    b.Navigation("ReviewsReceived");

                    b.Navigation("UserSkills");
                });
#pragma warning restore 612, 618
        }
    }
}
