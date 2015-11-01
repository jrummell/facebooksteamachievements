using System.Data.Entity;

namespace SteamAchievements.Data
{
    public class SteamContext : DbContext
    {
        public SteamContext()
            : base("name=SteamContext")
        {
        }

        public virtual DbSet<steam_Achievement> steam_Achievement { get; set; }
        public virtual DbSet<steam_AchievementName> steam_AchievementName { get; set; }
        public virtual DbSet<steam_User> steam_User { get; set; }
        public virtual DbSet<steam_UserAchievement> steam_UserAchievement { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<steam_Achievement>()
                .Property(e => e.ApiName)
                .IsUnicode(false);

            modelBuilder.Entity<steam_Achievement>()
                .Property(e => e.ImageUrl)
                .IsUnicode(false);

            modelBuilder.Entity<steam_Achievement>()
                .HasMany(e => e.steam_AchievementName)
                .WithRequired(e => e.steam_Achievement)
                .HasForeignKey(e => e.AchievementId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<steam_Achievement>()
                .HasMany(e => e.steam_UserAchievement)
                .WithRequired(e => e.steam_Achievement)
                .HasForeignKey(e => e.AchievementId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<steam_AchievementName>()
                .Property(e => e.Language)
                .IsUnicode(false);

            modelBuilder.Entity<steam_User>()
                .Property(e => e.SteamUserId)
                .IsUnicode(false);

            modelBuilder.Entity<steam_User>()
                .Property(e => e.AccessToken)
                .IsUnicode(false);

            modelBuilder.Entity<steam_User>()
                .Property(e => e.Language)
                .IsUnicode(false);

            modelBuilder.Entity<steam_User>()
                .HasMany(e => e.steam_UserAchievement)
                .WithRequired(e => e.steam_User)
                .WillCascadeOnDelete(false);
        }
    }
}