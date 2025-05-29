using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace diplomska.Models
{
    public class ChecklistAnswer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Question { get; set; }

        [Required]
        public string? Answer { get; set; }

        public string? Comment { get; set; }

        public int LoadingChecklistId { get; set; }

        [ForeignKey(nameof(LoadingChecklistId))]
        public LoadingChecklist LoadingChecklist { get; set; }
    }
}
