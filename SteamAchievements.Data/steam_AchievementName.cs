namespace SteamAchievements.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("steam_AchievementName")]
    public partial class steam_AchievementName
    {
        public int Id { get; set; }

        public int AchievementId { get; set; }

        [Required]
        [StringLength(20)]
        public string Language { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        public virtual steam_Achievement steam_Achievement { get; set; }
    }
}
