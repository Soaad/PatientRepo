import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Patient, MedicalRecord } from '../models/patient.model';
import { AuthService } from '../core/auth.service';

@Injectable({
  providedIn: 'root'
})
export class PatientService {

  private baseUrl = 'http://localhost:5169/api/Patients';
  private medicalUrl = 'http://localhost:5050/medicalrecords';

  constructor(private http: HttpClient, private authService: AuthService) {}

  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({ Authorization: `Bearer ${token}` });
  }

  getAllPatients(): Observable<Patient[]> {
    return this.http.get<Patient[]>(this.baseUrl, { headers: this.getAuthHeaders() });
  }

  getPatient(id: string): Observable<Patient> {
    return this.http.get<Patient>(`${this.baseUrl}/${id}`, { headers: this.getAuthHeaders() });
  }

  getMedicalRecord(patientId: string): Observable<MedicalRecord> {
    return this.http.get<MedicalRecord>(`${this.medicalUrl}/${patientId}`, { headers: this.getAuthHeaders() });
  }

  createPatient(patient: Patient): Observable<any> {
    return this.http.post(this.baseUrl, patient, { headers: this.getAuthHeaders() });
  }
}
