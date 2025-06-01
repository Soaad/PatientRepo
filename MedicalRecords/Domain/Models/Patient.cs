namespace MedicalRecords.Domain.Models;

public class Patient
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string EncryptedEmail { get; set; }
    public string EncryptedPhone { get; set; }
    public string Address { get; set; }
}