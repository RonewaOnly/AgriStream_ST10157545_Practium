using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriStream_ST10157545_Practium.Models
{
    public class Site
    {
        [Key]
        public string SiteId { get; set; }
        public string SiteName { get; set; }
        public string SiteDescription { get; set; }

        public string Longitude { get; set; }
        public string Latitude { get; set; }

        public string Location { get; set; }

        [ForeignKey("Project")]
        public string ProjectId { get; set; }
    }
}
