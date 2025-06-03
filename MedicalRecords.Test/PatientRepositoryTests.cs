using System.Data;
using Dapper;
using MedicalRecords.Application;
using MedicalRecords.Domain.Models;
using MedicalRecords.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Moq;

namespace MedicalRecords.Test;

public class PatientRepositoryTests: IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly PatientRepository _repository;
        private readonly Mock<ILogger<PatientRepository>> _loggerMock;
        private readonly Mock<IEncryptionService> _encryptionMock;

        public PatientRepositoryTests()
        {
            // Set up in-memory SQLite database
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Create Patients table
            _connection.Execute(@"
                CREATE TABLE Patients (
                    Id BLOB PRIMARY KEY,
                    FullName TEXT NOT NULL,
                    DateOfBirth TEXT NOT NULL,
                    EncryptedEmail TEXT NOT NULL,
                    EncryptedPhone TEXT NOT NULL,
                    Address TEXT NOT NULL
                );
            ");

            _loggerMock = new Mock<ILogger<PatientRepository>>();
            _encryptionMock = new Mock<IEncryptionService>();

            // Set up encryption mock to return the same value for simplicity
            _encryptionMock.Setup(e => e.Encrypt(It.IsAny<string>())).Returns<string>(s => s);
            _encryptionMock.Setup(e => e.Decrypt(It.IsAny<string>())).Returns<string>(s => s);

            _repository = new PatientRepository(_connection, _loggerMock.Object, _encryptionMock.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldInsertPatient()
        {
            // Arrange
            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                FullName = "John Doe",
                DateOfBirth = new DateTime(1990, 1, 1),
                EncryptedEmail = "john.doe@example.com",
                EncryptedPhone = "1234567890",
                Address = "123 Main St"
            };

            // Act
            await _repository.AddAsync(patient);

            // Assert
            var result = await _repository.GetByIdAsync(patient.Id);
            Assert.NotNull(result);
            Assert.Equal("John Doe", result.FullName);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllPatients()
        {
            // Arrange
            var patient1 = new Patient
            {
                Id = Guid.NewGuid(),
                FullName = "Alice Smith",
                DateOfBirth = new DateTime(1985, 5, 20),
                EncryptedEmail = "alice.smith@example.com",
                EncryptedPhone = "9876543210",
                Address = "456 Elm St"
            };

            var patient2 = new Patient
            {
                Id = Guid.NewGuid(),
                FullName = "Bob Johnson",
                DateOfBirth = new DateTime(1978, 3, 15),
                EncryptedEmail = "bob.johnson@example.com",
                EncryptedPhone = "5551234567",
                Address = "789 Oak St"
            };

            await _repository.AddAsync(patient1);
            await _repository.AddAsync(patient2);

            // Act
            var patients = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(patients);
            Assert.Equal(2, patients.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingPatient()
        {
            // Arrange
            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                FullName = "Charlie Brown",
                DateOfBirth = new DateTime(1992, 7, 10),
                EncryptedEmail = "charlie.brown@example.com",
                EncryptedPhone = "1112223333",
                Address = "321 Pine St"
            };

            await _repository.AddAsync(patient);

            // Modify patient details
            patient.FullName = "Charles Brown";
            patient.Address = "654 Maple St";

            // Act
            await _repository.UpdateAsync(patient);

            // Assert
            var updatedPatient = await _repository.GetByIdAsync(patient.Id);
            Assert.Equal("Charles Brown", updatedPatient.FullName);
            Assert.Equal("654 Maple St", updatedPatient.Address);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemovePatient()
        {
            // Arrange
            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                FullName = "Diana Prince",
                DateOfBirth = new DateTime(1988, 12, 25),
                EncryptedEmail = "diana.prince@example.com",
                EncryptedPhone = "4445556666",
                Address = "987 Cedar St"
            };

            await _repository.AddAsync(patient);

            // Act
            await _repository.DeleteAsync(patient.Id);

            // Assert
            var deletedPatient = await _repository.GetByIdAsync(patient.Id);
            Assert.Null(deletedPatient);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
