namespace SteamAchievements.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("steam_UserAchievement")]
    public partial class steam_UserAchievement
    {
        public long FacebookUserId { get; set; }

        public int AchievementId { get; set; }

        public DateTime Date { get; set; }

        public int Id { get; set; }

        public bool Published { get; set; }

        public bool Hidden { get; set; }

        public virtual steam_Achievement steam_Achievement { get; set; }

        public virtual steam_User steam_User { get; set; }
    }
}
