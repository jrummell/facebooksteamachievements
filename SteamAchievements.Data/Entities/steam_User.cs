using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SteamAchievements.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("steam_User")]
    public partial class steam_User : IdentityUser<int, UserLogin, UserRole, UserClaim>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public steam_User()
        {
            UserAchievements = new HashSet<steam_UserAchievement>();
        }

        [Obsolete]
        [Index(IsUnique = true)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long FacebookUserId { get; set; }

        [Required]
        [StringLength(50)]
        public string SteamUserId { get; set; }

        [Obsolete]
        [Required]
        [StringLength(250)]
        public string AccessToken { get; set; }

        [Obsolete]
        public bool AutoUpdate { get; set; }

        public bool PublishDescription { get; set; }

        [StringLength(20)]
        public string Language { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<steam_UserAchievement> UserAchievements { get; set; }
    }
}
