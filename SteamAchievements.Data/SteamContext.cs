using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SteamAchievements.Data
{
    public class SteamContext : IdentityDbContext<User, Role, int, UserLogin, UserRole, UserClaim>
    {
        public SteamContext()
            : base("name=SteamContext")
        {
            Database.Log = message => Debug.WriteLine(message);
        }

        public virtual DbSet<Achievement> Achievements { get; set; }
        public virtual DbSet<steam_AchievementName> AchievementNames { get; set; }
        public virtual DbSet<UserAchievement> UserAchievements { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Achievement>()
                .Property(e => e.ApiName)
                .IsUnicode(false);

            modelBuilder.Entity<Achievement>()
                .Property(e => e.ImageUrl)
                .IsUnicode(false);

            modelBuilder.Entity<Achievement>()
                .HasMany(e => e.AchievementNames)
                .WithRequired(e => e.Achievement)
                .HasForeignKey(e => e.AchievementId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Achievement>()
                .HasMany(e => e.UserAchievements)
                .WithRequired(e => e.Achievement)
                .HasForeignKey(e => e.AchievementId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<steam_AchievementName>()
                .Property(e => e.Language)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.SteamUserId)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Language)
                .IsUnicode(false);

            modelBuilder.Entity<UserAchievement>()
                .HasRequired(e => e.User)
                .WithMany(e => e.UserAchievements)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserAchievement>()
                .HasRequired(e => e.Achievement)
                .WithMany(e => e.UserAchievements)
                .HasForeignKey(e => e.AchievementId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Role>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<UserLogin>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<UserLogin>()
                .HasRequired(e => e.User)
                .WithMany(e => e.Logins)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserRole>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<UserRole>()
                .HasRequired(e => e.User)
                .WithMany(e => e.Roles)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<UserRole>()
                .HasRequired(e => e.Role)
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.RoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserClaim>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<UserClaim>()
                .HasRequired(e => e.User)
                .WithMany(e => e.Claims)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);
        }
    }
}