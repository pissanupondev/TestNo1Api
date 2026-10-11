using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(254);
            entity.HasIndex(e => e.Email).IsUnique(); // Email ต้องไม่ซ้ำกัน
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.BirthDay).IsRequired().HasColumnType("date");
            entity.Property(e => e.Sex).IsRequired().HasMaxLength(6).IsUnicode(false);
            entity.ToTable(table =>table.HasCheckConstraint("CK_Users_Sex","[Sex] IN ('Male', 'Female')"));
            entity.Property(e => e.Occupation).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime2(0)").HasDefaultValueSql("SYSUTCDATETIME()");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Profile).IsRequired().HasColumnType("varchar(max)");
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            entity.Property(e => e.FileSize).IsRequired();

            // ความสัมพันธ์ One-to-One และตั้งค่า Cascade Delete
            entity.HasOne(e => e.User)
                  .WithOne(e => e.UserProfile)
                  .HasForeignKey<UserProfile>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}