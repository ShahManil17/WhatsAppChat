﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WhatsAppChat.Data;

#nullable disable

namespace WhatsAppChat.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WhatsAppChat.Data.DataModel.Communication", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FilePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("IsDelivered")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<int?>("IsRead")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ReceiverId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("SendTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("SenderId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ReceiverId");

                    b.HasIndex("SenderId");

                    b.ToTable("Communication");
                });

            modelBuilder.Entity("WhatsAppChat.Data.DataModel.GroupHasMembers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("GroupId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupHasMembers");
                });

            modelBuilder.Entity("WhatsAppChat.Data.DataModel.GroupMessages", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FilePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GroupId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("IsDelivered")
                        .HasColumnType("int");

                    b.Property<int?>("IsRead")
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("SendTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("SenderId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("SenderId");

                    b.ToTable("GroupMessages");
                });

            modelBuilder.Entity("WhatsAppChat.Data.DataModel.GroupUnreads", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("GroupId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("MessageId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("MessageId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupUnreads");
                });

            modelBuilder.Entity("WhatsAppChat.Data.DataModel.Groups", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("GroupIcon")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GroupName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("WhatsAppChat.Data.DataModel.RefreshTokens", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("ExpireTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("WhatsAppChat.Data.DataModel.Users", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ConnectionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirebaseToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<DateTime?>("LogoutTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfileImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WhatsAppChat.Data.DataModel.Communication", b =>
                {
                    b.HasOne("WhatsAppChat.Data.DataModel.Users", "Receiver")
                        .WithMany()
                        .HasForeignKey("ReceiverId");

                    b.HasOne("WhatsAppChat.Data.DataModel.Users", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId");

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("WhatsAppChat.Data.DataModel.GroupHasMembers", b =>
                {
                    b.HasOne("WhatsAppChat.Data.DataModel.Groups", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");

                    b.HasOne("WhatsAppChat.Data.DataModel.Users", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WhatsAppChat.Data.DataModel.GroupMessages", b =>
                {
                    b.HasOne("WhatsAppChat.Data.DataModel.Groups", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");

                    b.HasOne("WhatsAppChat.Data.DataModel.Users", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId");

                    b.Navigation("Group");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("WhatsAppChat.Data.DataModel.GroupUnreads", b =>
                {
                    b.HasOne("WhatsAppChat.Data.DataModel.Groups", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");

                    b.HasOne("WhatsAppChat.Data.DataModel.GroupMessages", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId");

                    b.HasOne("WhatsAppChat.Data.DataModel.Users", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Group");

                    b.Navigation("Message");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WhatsAppChat.Data.DataModel.RefreshTokens", b =>
                {
                    b.HasOne("WhatsAppChat.Data.DataModel.Users", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
