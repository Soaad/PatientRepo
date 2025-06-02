using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using MedicalRecords.Application;
using MedicalRecords.Domain.Contracts;
using MedicalRecords.Domain.Models;

namespace MedicalRecords.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
    {
        private readonly ILogger <PatientRepository>_logger;
        private readonly IDbConnection _dbConnection;
        private readonly IEncryptionService _encryption;

        public PatientRepository(IDbConnection dbConnection, ILogger<PatientRepository> logger, IEncryptionService encryption)
    {
        _dbConnection = dbConnection;
        _logger = logger;
        _encryption = encryption;
        DatabaseInitializer.Initialize(_dbConnection);
    }

    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        try
        {
            var patients = _dbConnection.Query<Patient>("SELECT * FROM Patients ORDER BY FullName").ToList();

            // Decrypt sensitive data
            foreach (var patient in patients)
            {
                patient.EncryptedEmail = _encryption.Decrypt(patient.EncryptedEmail);
                patient.EncryptedPhone = _encryption.Decrypt(patient.EncryptedPhone);
            }

            return patients;
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex, "Database error occurred while retrieving patients.");
            throw new Exception("Database error. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            throw;
        }
    }

    public async Task<Patient?> GetByIdAsync(Guid id)
    {
        try
        {
            var query = "SELECT * FROM Patients WHERE Id = @Id";
            var patient = _dbConnection.QueryFirstOrDefault<Patient>(query, new { Id = id });

            if (patient != null)
            {
                patient.EncryptedEmail = _encryption.Decrypt(patient.EncryptedEmail);
                patient.EncryptedPhone = _encryption.Decrypt(patient.EncryptedPhone);
            }

            return patient;
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex, "Database error occurred while retrieving patient.");
            throw new Exception("Database error. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            throw;
        }
    }
    
    public async Task<(IEnumerable<Patient> Data, int TotalCount)> GetPagedAsync(int page, int size)
    {
        try
        {
            var offset = (page - 1) * size;

            var dataQuery = @"SELECT * FROM Patients
                                    ORDER BY FullName
                                    LIMIT @Size OFFSET @Offset;";
            var countQuery = "SELECT COUNT(*) FROM Patients;";

            var patients = _dbConnection.Query<Patient>(dataQuery, new { Size = size, Offset = offset }).ToList();
            int totalCount = _dbConnection.ExecuteScalar<int>(countQuery);

            foreach (var patient in patients)
            {
                patient.EncryptedEmail = _encryption.Decrypt(patient.EncryptedEmail);
                patient.EncryptedPhone = _encryption.Decrypt(patient.EncryptedPhone);
            }

            return (patients, totalCount);
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex, "Database error occurred while paging patients.");
            throw new Exception("Database error. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            throw;
        }
    }

    public async Task AddAsync(Patient patient)
    {
        try
        {
            var encryptedEmail = _encryption.Encrypt(patient.EncryptedEmail);
            var encryptedPhone = _encryption.Encrypt(patient.EncryptedPhone);

            _dbConnection.Execute(
                "INSERT INTO Patients (Id, FullName, DateOfBirth, EncryptedEmail, EncryptedPhone, Address) " +
                "VALUES (@Id, @FullName, @DateOfBirth, @EncryptedEmail, @EncryptedPhone, @Address)",
                new
                {
                    patient.Id,
                    patient.FullName,
                    patient.DateOfBirth,
                    EncryptedEmail = encryptedEmail,
                    EncryptedPhone = encryptedPhone,
                    patient.Address
                });
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex, "Database error occurred while adding patient.");
            throw new Exception("Database error. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            throw;
        }
    }

    public async Task UpdateAsync(Patient patient)
    {
        try
        {
            var encryptedEmail = _encryption.Encrypt(patient.EncryptedEmail);
            var encryptedPhone = _encryption.Encrypt(patient.EncryptedPhone);

            _dbConnection.Execute(
                "UPDATE Patients SET FullName = @FullName, DateOfBirth = @DateOfBirth, " +
                "EncryptedEmail = @EncryptedEmail, EncryptedPhone = @EncryptedPhone, Address = @Address " +
                "WHERE Id = @Id",
                new
                {
                    patient.Id,
                    patient.FullName,
                    patient.DateOfBirth,
                    EncryptedEmail = encryptedEmail,
                    EncryptedPhone = encryptedPhone,
                    patient.Address
                });
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex, "Database error occurred while updating patient.");
            throw new Exception("Database error. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            _dbConnection.Execute("DELETE FROM Patients WHERE Id = @Id", new { Id = id });
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex, "Database error occurred while deleting patient.");
            throw new Exception("Database error. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            throw;
        }
    }
}
