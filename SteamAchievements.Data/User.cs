namespace SteamAchievements.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("steam_User")]
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            UserAchievements = new HashSet<UserAchievement>();
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
        public virtual ICollection<UserAchievement> UserAchievements { get; set; }
    }
}
