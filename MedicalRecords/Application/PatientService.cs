using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using MedicalRecords.Domain.Contracts;
using MedicalRecords.Domain.Models;
using MedicalRecords.Middlewares.Custom_Exceptions;

namespace MedicalRecords.Application;

public class PatientService: IPatientService
{
    private readonly IPatientRepository _repository;
    private readonly ILogger <PatientService>_logger;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PatientService(IPatientRepository repository, ILogger <PatientService> logger, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _logger = logger;
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        try
        {
            return await _repository.GetAllAsync();
         
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in Patientervice.");
            throw new Exception("An error occurred while retrieving  Patients.");
        }
        
    }

    public async Task<Patient?> GetByIdAsync(Guid id)
    {
        
        try
        {
            var patient =  await _repository.GetByIdAsync(id);
            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {id} not found.");
            }
            return patient;
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in DriverService.");
            throw new Exception("An error occurred while retrieving the driver.");
        }
    }
    public async Task<(IEnumerable<Patient> Data, int TotalCount)> GetPagedAsync(int page, int size)
    {
        try
        {
            return await _repository.GetPagedAsync(page, size);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in PatientService while paging.");
            throw new Exception("An error occurred while retrieving paged patients.");
        }
    }
    public async Task<Patient> CreateAsync(Patient patient)
    {
       
        try
        {
            //patient.Id = Guid.NewGuid();
            await _repository.AddAsync(patient);
            return patient;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating patient.");
            throw new Exception("An error occurred while creating the patient.");
        }
    }

    public async Task<bool> UpdateAsync(Guid id, Patient patient)
    {
        try
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            patient.Id = id;
            await _repository.UpdateAsync(patient);
            return true;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating patient with ID {id}.");
            throw new Exception("An error occurred while updating patient.");
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
       
        try
        {
            var existing = await _repository.GetByIdAsync(id);
           
            if (existing == null)
            {
                throw new NotFoundException($"Driver with ID {id} not found.");
            }
            await _repository.DeleteAsync(id);
            return true;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting patien with ID {id}.");
            throw new Exception("An error occurred while deleting the patient.");
        }
    }
    
    public async Task<MedicalRecord?> GetMedicalRecord(Guid patientId)
    {
        try
        {
            // Forward user's token to MiniAPI
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.Split(" ").Last());

            var response = await _httpClient.GetAsync($"/medicalrecords/{patientId}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<MedicalRecord>();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while fetching medical record for the  patien with ID {patientId}.");
            throw new Exception("An error occurred while fetching medical record for the patient.");
        }
       
    }
}

