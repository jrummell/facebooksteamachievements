using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SteamAchievements.Data
{
    public class SteamContext : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public virtual DbSet<Achievement> Achievements { get; set; }
        public virtual DbSet<AchievementName> AchievementNames { get; set; }
        public virtual DbSet<UserAchievement> UserAchievements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            modelBuilder.Entity<Role>()
                        .HasKey(e => e.Id);

            modelBuilder.Entity<UserLogin>()
                        .HasKey(e => e.Id);
            modelBuilder.Entity<UserLogin>()
                        .HasOne(e => e.User)
                        .WithMany(e => e.Logins)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserRole>()
                        .HasKey(e => e.Id);
            modelBuilder.Entity<UserRole>()
                        .HasOne(e => e.User)
                        .WithMany(e => e.Roles)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<UserRole>()
                        .HasOne(e => e.Role)
                        .WithMany(e => e.Users)
                        .HasForeignKey(e => e.RoleId)
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserClaim>()
                        .HasKey(e => e.Id);
            modelBuilder.Entity<UserClaim>()
                        .HasOne(e => e.User)
                        .WithMany(e => e.Claims)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.NoAction);
        }
    }
}