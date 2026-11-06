using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriStream_ST10157545_Practium.Models
{
    public class SoilTest
    {
        [Key]
        public string TestID { get; set; }
        [ForeignKey("SoilSample")]
        public string SampleID { get; set; }

        public List<String> TestType { get; set; } = ["pH", "Nutrients", "Organic Matter", "Contaminants"];

        public string TestResult { get; set; }

        public DateTime TestDate { get; set; } = DateTime.Now;

        public string Unit { get; set; } = "mg/kg";


    }
}
