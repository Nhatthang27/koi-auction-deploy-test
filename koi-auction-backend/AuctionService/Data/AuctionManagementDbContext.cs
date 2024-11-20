using System;
using System.Collections.Generic;
using AuctionService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;

public partial class AuctionManagementDbContext : DbContext
{
    public AuctionManagementDbContext()
    {
    }

    public AuctionManagementDbContext(DbContextOptions<AuctionManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Auction> Auctions { get; set; }

    public virtual DbSet<AuctionDeposit> AuctionDeposits { get; set; }

    public virtual DbSet<AuctionLot> AuctionLots { get; set; }

    public virtual DbSet<AuctionLotStatus> AuctionLotStatuses { get; set; }

    public virtual DbSet<AuctionMethod> AuctionMethods { get; set; }

    public virtual DbSet<AuctionStatus> AuctionStatuses { get; set; }

    public virtual DbSet<BidLog> BidLogs { get; set; }

    public virtual DbSet<KoiFish> KoiFishes { get; set; }

    public virtual DbSet<KoiMedia> KoiMedia { get; set; }

    public virtual DbSet<Lot> Lots { get; set; }

    public virtual DbSet<LotStatus> LotStatuses { get; set; }

    public virtual DbSet<SoldLot> SoldLots { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auction>(entity =>
        {
            entity.HasKey(e => e.AuctionId).HasName("PK__Auction__51004A4C41F9D350");

            entity.ToTable("Auction", tb => tb.HasTrigger("trg_UpdateTimestamp_Auction"));

            entity.HasIndex(e => e.AuctionName, "UQ_Auction_AuctionName").IsUnique();

            entity.Property(e => e.AuctionName).HasMaxLength(100);
            entity.Property(e => e.AuctionStatusId).HasDefaultValue(1);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");

            entity.HasOne(d => d.AuctionStatus).WithMany(p => p.Auctions)
                .HasForeignKey(d => d.AuctionStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Auction_AuctionStatus");
        });

        modelBuilder.Entity<AuctionDeposit>(entity =>
        {
            entity.HasKey(e => e.AuctionDepositId).HasName("PK__AuctionD__5BDF0B615851EA3A");

            entity.ToTable("AuctionDeposit", tb => tb.HasTrigger("trg_UpdateTimestamp_AuctionDeposit"));

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AuctionDepositStatus).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))");

            entity.HasOne(d => d.AuctionLot).WithMany(p => p.AuctionDeposits)
                .HasForeignKey(d => d.AuctionLotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AuctionDe__Aucti__503BEA1C");
        });

        modelBuilder.Entity<AuctionLot>(entity =>
        {
            entity.HasKey(e => e.AuctionLotId).HasName("PK__AuctionL__8A0269CF6F45A3A7");

            entity.ToTable("AuctionLot", tb => tb.HasTrigger("trg_UpdateTimestamp_AuctionLot"));

            entity.Property(e => e.AuctionLotId).ValueGeneratedNever();
            entity.Property(e => e.AuctionLotStatusId).HasDefaultValue(1);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.StepPercent).HasDefaultValue(0);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Auction).WithMany(p => p.AuctionLots)
                .HasForeignKey(d => d.AuctionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuctionLot_Auction");

            entity.HasOne(d => d.AuctionLotNavigation).WithOne(p => p.AuctionLot)
                .HasForeignKey<AuctionLot>(d => d.AuctionLotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuctionLot_Lot");

            entity.HasOne(d => d.AuctionLotStatus).WithMany(p => p.AuctionLots)
                .HasForeignKey(d => d.AuctionLotStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuctionLot_AuctionLotStatus");
        });

        modelBuilder.Entity<AuctionLotStatus>(entity =>
        {
            entity.HasKey(e => e.AuctionLotStatusId).HasName("PK__AuctionL__07A5E3C3219CCA3D");

            entity.ToTable("AuctionLotStatus");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");
            entity.Property(e => e.StatusName).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<AuctionMethod>(entity =>
        {
            entity.HasKey(e => e.AuctionMethodId).HasName("PK__AuctionM__FCD1A18BBDBDB361");

            entity.ToTable("AuctionMethod");

            entity.Property(e => e.AuctionMethodName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<AuctionStatus>(entity =>
        {
            entity.HasKey(e => e.AuctionStatusId).HasName("PK__AuctionS__B2535E958E1BCE35");

            entity.ToTable("AuctionStatus");

            entity.Property(e => e.AuctionStatusName).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<BidLog>(entity =>
        {
            entity.HasKey(e => e.BidLogId).HasName("PK__BidLog__A459EE9EB705A49A");

            entity.ToTable("BidLog");

            entity.Property(e => e.BidAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BidTime)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");

            entity.HasOne(d => d.AuctionLot).WithMany(p => p.BidLogs)
                .HasForeignKey(d => d.AuctionLotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BidLog_AuctionLot");
        });

        modelBuilder.Entity<KoiFish>(entity =>
        {
            entity.HasKey(e => e.KoiFishId).HasName("PK__KoiFish__44D35C25EB29C4A2");

            entity.ToTable("KoiFish");

            entity.Property(e => e.KoiFishId).ValueGeneratedNever();
            entity.Property(e => e.SizeCm)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("SizeCM");
            entity.Property(e => e.Variety).HasMaxLength(500);
            entity.Property(e => e.WeightKg)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("WeightKG");

            entity.HasOne(d => d.KoiFishNavigation).WithOne(p => p.KoiFish)
                .HasForeignKey<KoiFish>(d => d.KoiFishId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KoiFish_Lot");
        });

        modelBuilder.Entity<KoiMedia>(entity =>
        {
            entity.HasKey(e => e.KoiMediaId).HasName("PK__KoiMedia__4CC780832EF9B140");

            entity.HasIndex(e => e.FilePath, "UQ_KoiMedia_FilePath").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");
            entity.Property(e => e.FilePath).HasMaxLength(500);

            entity.HasOne(d => d.KoiFish).WithMany(p => p.KoiMedia)
                .HasForeignKey(d => d.KoiFishId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KoiMedia_KoiFish");
        });

        modelBuilder.Entity<Lot>(entity =>
        {
            entity.HasKey(e => e.LotId).HasName("PK__Lot__4160EFADFD0A2547");

            entity.ToTable("Lot", tb => tb.HasTrigger("trg_UpdateTimestamp_Lot"));

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");
            entity.Property(e => e.LotStatusId).HasDefaultValue(1);
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasDefaultValue("TEMP-SKU")
                .HasColumnName("SKU");
            entity.Property(e => e.StartingPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");

            entity.HasOne(d => d.AuctionMethod).WithMany(p => p.Lots)
                .HasForeignKey(d => d.AuctionMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lot_AuctionMethod");

            entity.HasOne(d => d.LotStatus).WithMany(p => p.Lots)
                .HasForeignKey(d => d.LotStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lot_LotStatus");
        });

        modelBuilder.Entity<LotStatus>(entity =>
        {
            entity.HasKey(e => e.LotStatusId).HasName("PK__LotStatu__B5B838777103AE5C");

            entity.ToTable("LotStatus");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");
            entity.Property(e => e.LotStatusName).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<SoldLot>(entity =>
        {
            entity.HasKey(e => e.SoldLotId).HasName("PK__SoldLot__A006956D74F831B3");

            entity.ToTable("SoldLot", tb => tb.HasTrigger("trg_UpdateTimestamp_SoldLot"));

            entity.Property(e => e.SoldLotId).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpTime).HasColumnType("datetime");
            entity.Property(e => e.FinalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'SE Asia Standard Time'))")
                .HasColumnType("datetime");

            entity.HasOne(d => d.SoldLotNavigation).WithOne(p => p.SoldLot)
                .HasForeignKey<SoldLot>(d => d.SoldLotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SoldLot_AuctionLot");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
