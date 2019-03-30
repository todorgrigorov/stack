using System;
using System.ComponentModel.DataAnnotations;

namespace Stack
{
    public class VersionInfo : Entity
    {
        [Required]
        [StringLength(500)]
        public virtual string Version { get; set; }

        public Version ToVersion()
        {
            return System.Version.Parse(Version);
        }
    }
}
