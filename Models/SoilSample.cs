using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriStream_ST10157545_Practium.Models
{
    public class SoilSample
    {
        [Key]
        public string SampleId { get; set; }

        [ForeignKey("Site")]
        public string SiteId { get; set; }

        public string SampleName { get; set; }

        public DateOnly SampleDate { get; set; }

        public string SampleDescription { get; set; }

        public string SampleLocation { get; set; }
    }
}
