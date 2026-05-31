using Envialo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Envialo.Infrastructure.Persistence;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<DriverProfile> DriverProfiles { get; set; }
    public virtual DbSet<ShipmentRequest> ShipmentRequests { get; set; }
    public virtual DbSet<FareOffer> FareOffers { get; set; }
    public virtual DbSet<Trip> Trips { get; set; }
    public virtual DbSet<Rating> Ratings { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");
            entity.ToTable("users");
            entity.HasIndex(e => new { e.Role, e.Status }, "idx_users_role_status");
            entity.HasIndex(e => e.Email, "uq_users_email").IsUnique().HasFilter("(deleted_at IS NULL)");
            entity.HasIndex(e => e.Phone, "uq_users_phone").IsUnique().HasFilter("(deleted_at IS NULL)");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
            entity.Property(e => e.FullName).HasMaxLength(120).HasColumnName("full_name");
            entity.Property(e => e.Email).HasMaxLength(180).HasColumnName("email");
            entity.Property(e => e.Phone).HasMaxLength(20).HasColumnName("phone");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Role).HasMaxLength(20).HasDefaultValueSql("'CLIENT'::character varying").HasColumnName("role");
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValueSql("'PENDING_VERIFICATION'::character varying").HasColumnName("status");
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refresh_tokens_pkey");
            entity.ToTable("refresh_tokens");
            entity.HasIndex(e => e.UserId, "idx_rt_user_id");
            entity.HasIndex(e => e.TokenHash, "idx_rt_token_hash");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.TokenHash).HasColumnName("token_hash");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("refresh_tokens_user_id_fkey");
        });

        modelBuilder.Entity<DriverProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("driver_profiles_pkey");
            entity.ToTable("driver_profiles");
            entity.HasIndex(e => e.Status, "idx_driver_status");
            entity.HasIndex(e => new { e.VehicleType, e.Status }, "idx_driver_vehicle_type");
            entity.HasIndex(e => e.LicenseNumber, "uq_driver_license").IsUnique();
            entity.HasIndex(e => e.VehiclePlate, "uq_driver_plate").IsUnique();
            entity.HasIndex(e => e.UserId, "uq_driver_user").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.LicenseNumber).HasMaxLength(30).HasColumnName("license_number");
            entity.Property(e => e.LicenseExpiry).HasColumnName("license_expiry");
            entity.Property(e => e.VehicleType).HasMaxLength(40).HasColumnName("vehicle_type");
            entity.Property(e => e.VehiclePlate).HasMaxLength(15).HasColumnName("vehicle_plate");
            entity.Property(e => e.VehicleBrand).HasMaxLength(60).HasColumnName("vehicle_brand");
            entity.Property(e => e.VehicleModel).HasMaxLength(60).HasColumnName("vehicle_model");
            entity.Property(e => e.VehicleYear).HasColumnName("vehicle_year");
            entity.Property(e => e.PayloadCapacityKg).HasPrecision(8, 2).HasColumnName("payload_capacity_kg");
            entity.Property(e => e.VolumeCapacityM3).HasPrecision(6, 2).HasColumnName("volume_capacity_m3");
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValueSql("'PENDING'::character varying").HasColumnName("status");
            entity.Property(e => e.AvgRating).HasPrecision(3, 2).HasDefaultValueSql("0.00").HasColumnName("avg_rating");
            entity.Property(e => e.TotalTrips).HasDefaultValue(0).HasColumnName("total_trips");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");

            entity.HasOne(d => d.User).WithOne(p => p.DriverProfile)
                .HasForeignKey<DriverProfile>(d => d.UserId)
                .HasConstraintName("driver_profiles_user_id_fkey");
        });

        modelBuilder.Entity<ShipmentRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shipment_requests_pkey");
            entity.ToTable("shipment_requests");
            entity.HasIndex(e => e.ClientId, "idx_sr_client_id");
            entity.HasIndex(e => e.ExpiresAt, "idx_sr_expires_at").HasFilter("((status)::text = 'OPEN'::text)");
            entity.HasIndex(e => new { e.Status, e.CreatedAt }, "idx_sr_status_created").IsDescending(false, true).HasFilter("(deleted_at IS NULL)");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.OriginAddress).HasColumnName("origin_address");
            entity.Property(e => e.OriginReference).HasMaxLength(200).HasColumnName("origin_reference");
            entity.Property(e => e.DestinationAddress).HasColumnName("destination_address");
            entity.Property(e => e.DestReference).HasMaxLength(200).HasColumnName("dest_reference");
            entity.Property(e => e.CargoDescription).HasColumnName("cargo_description");
            entity.Property(e => e.WeightKg).HasPrecision(8, 2).HasColumnName("weight_kg");
            entity.Property(e => e.WidthCm).HasPrecision(6, 2).HasColumnName("width_cm");
            entity.Property(e => e.HeightCm).HasPrecision(6, 2).HasColumnName("height_cm");
            entity.Property(e => e.DepthCm).HasPrecision(6, 2).HasColumnName("depth_cm");
            entity.Property(e => e.RequiresRefrigeration).HasDefaultValue(false).HasColumnName("requires_refrigeration");
            entity.Property(e => e.Fragile).HasDefaultValue(false).HasColumnName("fragile");
            entity.Property(e => e.PhotoUrl).HasColumnName("photo_url");
            entity.Property(e => e.SuggestedPrice).HasPrecision(10, 2).HasColumnName("suggested_price");
            entity.Property(e => e.Currency).HasMaxLength(3).HasDefaultValueSql("'PEN'::bpchar").IsFixedLength().HasColumnName("currency");
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValueSql("'OPEN'::character varying").HasColumnName("status");
            entity.Property(e => e.CancelReason).HasColumnName("cancel_reason");
            entity.Property(e => e.ExpiresAt).HasDefaultValueSql("(now() + '02:00:00'::interval)").HasColumnName("expires_at");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

            entity.HasOne(d => d.Client).WithMany(p => p.ShipmentRequests)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shipment_requests_client_id_fkey");
        });

        modelBuilder.Entity<FareOffer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("fare_offers_pkey");
            entity.ToTable("fare_offers");
            entity.HasIndex(e => e.DriverId, "idx_fo_driver_id");
            entity.HasIndex(e => new { e.ShipmentId, e.Status }, "idx_fo_shipment_status");
            entity.HasIndex(e => new { e.ShipmentId, e.DriverId }, "uq_fo_one_pending_per_driver").IsUnique().HasFilter("((status)::text = 'PENDING'::text)");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
            entity.Property(e => e.ShipmentId).HasColumnName("shipment_id");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.OfferedPrice).HasPrecision(10, 2).HasColumnName("offered_price");
            entity.Property(e => e.Currency).HasMaxLength(3).HasDefaultValueSql("'PEN'::bpchar").IsFixedLength().HasColumnName("currency");
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValueSql("'PENDING'::character varying").HasColumnName("status");
            entity.Property(e => e.DriverNote).HasMaxLength(300).HasColumnName("driver_note");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");

            entity.HasOne(d => d.Driver).WithMany(p => p.FareOffers)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fare_offers_driver_id_fkey");

            entity.HasOne(d => d.Shipment).WithMany(p => p.FareOffers)
                .HasForeignKey(d => d.ShipmentId)
                .HasConstraintName("fare_offers_shipment_id_fkey");
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("trips_pkey");
            entity.ToTable("trips");
            entity.HasIndex(e => new { e.DriverId, e.Status }, "idx_trip_driver_status");
            entity.HasIndex(e => e.Status, "idx_trip_status");
            entity.HasIndex(e => e.ShipmentId, "uq_trip_shipment").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
            entity.Property(e => e.ShipmentId).HasColumnName("shipment_id");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.AcceptedOfferId).HasColumnName("accepted_offer_id");
            entity.Property(e => e.FinalPrice).HasPrecision(10, 2).HasColumnName("final_price");
            entity.Property(e => e.Currency).HasMaxLength(3).HasDefaultValueSql("'PEN'::bpchar").IsFixedLength().HasColumnName("currency");
            entity.Property(e => e.PaymentMethod).HasMaxLength(30).HasDefaultValueSql("'CASH_ON_DELIVERY'::character varying").HasColumnName("payment_method");
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValueSql("'CONFIRMED'::character varying").HasColumnName("status");
            entity.Property(e => e.CancelReason).HasColumnName("cancel_reason");
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");

            entity.HasOne(d => d.AcceptedOffer).WithMany(p => p.Trips)
                .HasForeignKey(d => d.AcceptedOfferId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("trips_accepted_offer_id_fkey");

            entity.HasOne(d => d.Driver).WithMany(p => p.Trips)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("trips_driver_id_fkey");

            entity.HasOne(d => d.Shipment).WithOne(p => p.Trip)
                .HasForeignKey<Trip>(d => d.ShipmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("trips_shipment_id_fkey");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ratings_pkey");
            entity.ToTable("ratings");
            entity.HasIndex(e => e.RatedUserId, "idx_rating_rated_user");
            entity.HasIndex(e => new { e.TripId, e.RatedByUserId }, "uq_rating_per_user_per_trip").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").HasColumnName("id");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.RatedByUserId).HasColumnName("rated_by_user_id");
            entity.Property(e => e.RatedUserId).HasColumnName("rated_user_id");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.Comment).HasMaxLength(500).HasColumnName("comment");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");

            entity.HasOne(d => d.RatedByUser).WithMany(p => p.RatingRatedByUsers)
                .HasForeignKey(d => d.RatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ratings_rated_by_user_id_fkey");

            entity.HasOne(d => d.RatedUser).WithMany(p => p.RatingRatedUsers)
                .HasForeignKey(d => d.RatedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ratings_rated_user_id_fkey");

            entity.HasOne(d => d.Trip).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.TripId)
                .HasConstraintName("ratings_trip_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}