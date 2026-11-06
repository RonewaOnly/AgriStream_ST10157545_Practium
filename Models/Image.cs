using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriStream_ST10157545_Practium.Models
{
    public class Image
    {
        [Key]
        public int ImageId { get; set; }

        [ForeignKey("Site")]
        public string SiteId { get; set; }

        public string ImageName { get; set; }

        public FileStream ImageFile { get; set; }

        public string ImageDescription { get; set; }

        public string ImageLocation { get; set; }
        public DateOnly ImageDate { get; set; }

    }
}
