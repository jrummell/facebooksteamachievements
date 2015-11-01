namespace SteamAchievements.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("steam_Achievement")]
    public partial class steam_Achievement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public steam_Achievement()
        {
            steam_AchievementName = new HashSet<steam_AchievementName>();
            steam_UserAchievement = new HashSet<steam_UserAchievement>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ApiName { get; set; }

        public int GameId { get; set; }

        [Required]
        [StringLength(200)]
        public string ImageUrl { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<steam_AchievementName> steam_AchievementName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<steam_UserAchievement> steam_UserAchievement { get; set; }
    }
}
