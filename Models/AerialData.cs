using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriStream_ST10157545_Practium.Models
{
    public class AerialData
    {
        [Key]
        public int AerialDataId { get; set; }
        [ForeignKey("Site")]
        public string SiteId { get; set; }

        public string DataType { get; set; }

        public FileStream DataFile { get; set; }

        public string DataDescription { get; set; }

        public string FileName { get; set; }

        public DateTime UploadTime { get; set; } = DateTime.Now;


    }
}
