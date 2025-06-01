using MedicalRecords.Domain.Models;

namespace MedicalRecords.Domain.Contracts;

public interface IPatientRepository
{
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(Guid id);
    Task<(IEnumerable<Patient> Data, int TotalCount)> GetPagedAsync(int page, int size);
    Task AddAsync(Patient patient);
    Task UpdateAsync(Patient patient);
    Task DeleteAsync(Guid id);
}