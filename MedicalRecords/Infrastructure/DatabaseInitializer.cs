using System.Data;
using Dapper;

namespace MedicalRecords.Infrastructure;

public class DatabaseInitializer
{
    public static void Initialize(IDbConnection connection)
    {
        connection.Open(); // Ensure connection is open
        const string createTableQuery = @"
        CREATE TABLE IF NOT EXISTS Patients (
            Id TEXT PRIMARY KEY,
            FullName TEXT NOT NULL,
            DateOfBirth TEXT NOT NULL,
            EncryptedEmail TEXT NOT NULL,
            EncryptedPhone TEXT NOT NULL,
            Address TEXT
        );
    ";

        connection.Execute(createTableQuery);
    }
}