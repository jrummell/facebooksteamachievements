using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace SteamAchievements.Data
{
    public class SteamContext : ApiAuthorizationDbContext<User>
    {
        public SteamContext(DbContextOptions<SteamContext> options,
                            IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {
        }

        public virtual DbSet<Achievement> Achievements { get; set; }
        public virtual DbSet<AchievementName> AchievementNames { get; set; }
        public virtual DbSet<UserAchievement> UserAchievements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Achievement>()
                        .Property(e => e.ApiName)
                        .IsUnicode(false);

            modelBuilder.Entity<Achievement>()
                        .Property(e => e.ImageUrl)
                        .IsUnicode(false);

            modelBuilder.Entity<Achievement>()
                        .HasMany(e => e.AchievementNames)
                        .WithOne(e => e.Achievement)
                        .HasForeignKey(e => e.AchievementId)
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Achievement>()
                        .HasMany(e => e.UserAchievements)
                        .WithOne(e => e.Achievement)
                        .HasForeignKey(e => e.AchievementId)
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AchievementName>()
                        .Property(e => e.Language)
                        .IsUnicode(false);

            modelBuilder.Entity<User>()
                        .Property(e => e.SteamUserId)
                        .IsUnicode(false);

            modelBuilder.Entity<User>()
                        .HasIndex(e => e.SteamUserId)
                        .IsUnique();

            modelBuilder.Entity<User>()
                        .Property(e => e.Language)
                        .IsUnicode(false);

            modelBuilder.Entity<User>()
                        .HasIndex(e => e.FacebookUserId)
                        .IsUnique();

            modelBuilder.Entity<UserAchievement>()
                        .HasOne(e => e.User)
                        .WithMany(e => e.UserAchievements)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserAchievement>()
                        .HasOne(e => e.Achievement)
                        .WithMany(e => e.UserAchievements)
                        .HasForeignKey(e => e.AchievementId)
                        .OnDelete(DeleteBehavior.NoAction);
        }
    }
}