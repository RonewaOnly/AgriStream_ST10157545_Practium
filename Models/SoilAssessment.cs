
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriStream_ST10157545_Practium.Models
{
    public class SoilAssessment
    {
        [Key]
        public string AssessmentId { get; set; }
        [ForeignKey("Site")]
        public string SiteId { get; set; }

        public DateOnly AssessmentDate { get; set; }

        public string SoilQuality { get; set; }

        public string Findings { get; set; }

        public string Recommendations { get; set; }
    }
}
