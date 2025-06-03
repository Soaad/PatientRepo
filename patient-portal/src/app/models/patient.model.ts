export interface Patient {
  id: string;
  fullName: string;
  dateOfBirth: string;
  encryptedEmail: string;
  encryptedPhone: string;
  address: string;
}



export interface MedicalRecord {
  patientId: string;
  diagnosis: string;
  notes: string;
  lastUpdated: string;
}