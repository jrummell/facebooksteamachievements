using System.Data.Entity;
using System.Diagnostics;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SteamAchievements.Data
{
    public class SteamContext : IdentityDbContext<User>
    {
        public SteamContext()
            : base("name=SteamContext")
        {
            Database.Log = message => Debug.WriteLine(message);
        }

        public virtual DbSet<Achievement> Achievements { get; set; }
        public virtual DbSet<AchievementName> AchievementNames { get; set; }
        public virtual DbSet<UserAchievement> UserAchievements { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
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

            modelBuilder.Entity<AchievementName>()
                .Property(e => e.Language)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.SteamUserId)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.AccessToken)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Language)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserAchievements)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);
        }
    }
}