using MedicalRecords.Domain.Models;

namespace MedicalRecords.Domain.Contracts;

public interface IPatientService
{
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(Guid id);
    Task<(IEnumerable<Patient> Data, int TotalCount)> GetPagedAsync(int page, int size);

    Task<Patient> CreateAsync(Patient patient);
    Task<bool> UpdateAsync(Guid id, Patient patient);
    Task<bool> DeleteAsync(Guid id);
    Task<MedicalRecord?> GetMedicalRecord(Guid patientId);
}