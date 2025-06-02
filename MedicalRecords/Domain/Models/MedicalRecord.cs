namespace MedicalRecords.Domain.Models;

public class MedicalRecord
{
    public Guid PatientId { get; set; }
    public string Diagnosis { get; set; }
    public string Notes { get; set; }
    public DateTime LastUpdated { get; set; }
}