using HelpMyStreet.PostcodeCoordinates.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpMyStreet.PostcodeCoordinates.EF.Extensions
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Set up all postcode coordinate tables (Address.Postcode, Staging.Postcode_Switch, Staging.Postcode_Old and Staging.Postcode_Staging)
        /// </summary>
        public static void SetupPostcodeCoordinateTables(this ModelBuilder modelBuilder)
        {
            // these tables need to exactly the same 
            modelBuilder.Entity<PostcodeEntity>(entity =>
            {
                entity.ToTable("Postcode", "Address");
                SetupPostcodeCoordinateTables(entity);
            });

            modelBuilder.Entity<PostcodeEntitySwitchEntity>(entity =>
            {
                entity.ToTable("Postcode_Switch", "Staging");
                SetupPostcodeCoordinateTables(entity);
            });

            modelBuilder.Entity<PostcodeEntityOldEntity>(entity =>
            {
                entity.ToTable("Postcode_Old", "Staging");
                SetupPostcodeCoordinateTables(entity);
            });

            SetupPostcodeCoordinateStagingTable(modelBuilder);
        }

        /// <summary>
        /// Set up only the postcode staging table (Staging.Postcode_Staging - will only be used by the Address Service because it has different columns in Address.Postcode)
        /// </summary>
        public static void SetupPostcodeCoordinateStagingTable(this ModelBuilder modelBuilder)
        {
            // this table is used to load the postcode data
            modelBuilder.Entity<PostcodeStagingEntity>(entity =>
            {
                entity.ToTable("Postcode_Staging", "Staging");

                entity.Property(e => e.Id);

                entity.Property(x => x.Id).ValueGeneratedOnAdd();

                entity.HasKey(x => x.Id);

                entity.Property(e => e.Postcode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Latitude)
                    .IsRequired()
                    .HasColumnType("decimal(9,6)");

                entity.Property(e => e.Longitude)
                    .IsRequired()
                    .HasColumnType("decimal(9,6)");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);
            });
        }

        /// <summary>
        /// Set up default indexes on postcode tables (Address.Postcode, Staging.Postcode_Switch and Staging.Postcode_Old)
        /// </summary>
        public static void SetupPostcodeCoordinateDefaultIndexes(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PostcodeEntity>(entity =>
            {
                SetupPostcodeDefaultIndexes(entity);
            });

            modelBuilder.Entity<PostcodeEntitySwitchEntity>(entity =>
            {
                SetupPostcodeDefaultIndexes(entity);
            });

            modelBuilder.Entity<PostcodeEntityOldEntity>(entity =>
            {
                SetupPostcodeDefaultIndexes(entity);
            });
        }

        private static void SetupPostcodeCoordinateTables<T>(EntityTypeBuilder<T> entity) where T : PostcodeEntityBase
        {
            entity.Property(e => e.Id);

            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            entity.HasKey(x => x.Id);

            entity.Property(e => e.Postcode)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.Latitude)
                .IsRequired()
                .HasColumnType("decimal(9,6)");

            entity.Property(e => e.Longitude)
                .IsRequired()
                .HasColumnType("decimal(9,6)");

            entity.Property(e => e.LastUpdated)
                .IsRequired()
                .HasDefaultValueSql("GetUtcDate()")
                .HasColumnType("datetime2(0)");

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        }

        private static void SetupPostcodeDefaultIndexes<T>(EntityTypeBuilder<T> entity) where T : PostcodeEntityBase
        {
            entity.HasIndex(u => u.Postcode)
                .HasName("UX_Postcode_Postcode")
                .IsUnique();

            entity.HasIndex(u => new { u.Postcode, u.IsActive })
                .HasName("IX_Postcode_Postcode_IsActive")
                .ForSqlServerInclude(nameof(PostcodeEntityBase.Latitude), nameof(PostcodeEntityBase.Longitude));

            entity.HasIndex(u => new { u.Latitude, u.Longitude, u.IsActive })
                .ForSqlServerInclude(nameof(PostcodeEntityBase.Postcode))
                .HasName("IX_Postcode_Latitude_Longitude_IsActive");
        }

    }
}
