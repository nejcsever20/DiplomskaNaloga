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
        public int Rampa1 { get; set; }
        public int Rampa2 { get; set; }

        public string? CarinskaVrvicva { get; set; }
        public string? UstreznostVozilca { get; set; }

        public DateTime? ZavrnilZacetek { get; set; }

        // Optional, if not already present:
        public DateTime? ZacetekNaklada { get; set; }
        public DateTime? KonecNaklada { get; set; }

        public long TransportId { get; set; }
        public Transport Transport { get; set; }

    }
}
