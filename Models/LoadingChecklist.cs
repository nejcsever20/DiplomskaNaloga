using static diplomska.Pages.Izmenovodja.LoadingCheckOutModel;

namespace diplomska.Models
{
    public class LoadingChecklist
    {
        public int id { get; set; }

        // Basic info
        public DateTime StartLoading { get; set; }
        public DateTime EndLoading { get; set; }
        public string? CmrNumber { get; set; }
        public string? TransportNumber { get; set; }
        public double LoadedQuantity { get; set; }
        public string? RegistrationPlates { get; set; }
        public string? Seal { get; set; }

        //Signatures
        public string? WarehouseSignature { get; set; }
        public string? DriverSignature { get; set; }

        // Checklist Items
        public ICollection<ChecklistAnswer> ChecklistAnswer { get; set; } = new List<ChecklistAnswer>();

    }
}
