using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriStream_ST10157545_Practium.Models
{
    public class Video
    {
        [Key]
        public int VideoId { get; set; }

        [ForeignKey("Site")]
        public string SiteId { get; set; }

        public string VideoName { get; set; }

        public FileStream VideoFile { get; set; }

        public string VideoDescription { get; set; }

        public DateTime UploadTime { get; set; } = DateTime.Now;
    }
}
