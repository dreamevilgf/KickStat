using KickStat.Data.Domain;
using KickStat.Data.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace KickStat.Data;

public partial class KickStatDbContext : IdentityDbContext<KickStatUser, KickStatRole, Guid, IdentityUserClaim<Guid>,
    IdentityUserRole<Guid>,
    IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>, KickStatUserToken>
{
    public DbSet<AppSetting> AppSettings { get; set; } = null!;
    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<Player> Players { get; set; } = null!;


    private readonly string? _connectionString;

    public KickStatDbContext()
    {
    }

    public KickStatDbContext(DbContextOptions<KickStatDbContext> options) : base(options)
    {
    }

    public KickStatDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public static KickStatDbContext CreateReadOnly()
    {
        var context = new KickStatDbContext();
        context.ChangeTracker.AutoDetectChangesEnabled = false;
        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        return context;
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!string.IsNullOrEmpty(_connectionString))
            optionsBuilder.UseNpgsql(_connectionString);


        optionsBuilder.UseSnakeCaseNamingConvention();

#if DEBUG
        optionsBuilder.LogTo(message => Debug.WriteLine(message), Microsoft.Extensions.Logging.LogLevel.Information);
#endif
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        #region .NET Core Identity

        modelBuilder.Entity<KickStatUser>().ToTable("identity_users");
        modelBuilder.Entity<KickStatRole>().ToTable("identity_roles");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("identity_user_roles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("identity_user_claims");
        modelBuilder.Entity<KickStatUserToken>().ToTable("identity_user_tokens");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("identity_user_logins");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("identity_role_claims");

        #endregion

    }


    public KickStatDbContext AsReadOnly()
    {
        this.ChangeTracker.AutoDetectChangesEnabled = false;
        this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        return this;
    }

    public KickStatDbContext AsReadWrite()
    {
        this.ChangeTracker.AutoDetectChangesEnabled = true;
        this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

        return this;
    }
}