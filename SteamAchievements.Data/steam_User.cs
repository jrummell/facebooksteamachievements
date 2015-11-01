namespace SteamAchievements.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("steam_User")]
    public partial class steam_User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public steam_User()
        {
            steam_UserAchievement = new HashSet<steam_UserAchievement>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long FacebookUserId { get; set; }

        [Required]
        [StringLength(50)]
        public string SteamUserId { get; set; }

        [Required]
        [StringLength(250)]
        public string AccessToken { get; set; }

        public bool AutoUpdate { get; set; }

        public bool PublishDescription { get; set; }

        [StringLength(20)]
        public string Language { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<steam_UserAchievement> steam_UserAchievement { get; set; }
    }
}
