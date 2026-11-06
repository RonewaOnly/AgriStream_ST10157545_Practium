using System.ComponentModel.DataAnnotations;

namespace AgriStream_ST10157545_Practium.Models
{
    public class Project
    {
        [Key]
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public List<string> ProjectStatus { get; set; } = ["Not Started", "In Progress", "Completed","Cancelled","On Hold"];

    }
}
