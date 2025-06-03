import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MedicalRecord } from '../models/medicalrecord.model';

@Injectable({
  providedIn: 'root'
})
export class MedicalRecordService {
  private baseUrl = 'http://localhost:5169/medicalrecords';

  constructor(private http: HttpClient) {}

  getMedicalRecords(patientId: string): Observable<MedicalRecord[]> {
    debugger
    return this.http.get<MedicalRecord[]>(`${this.baseUrl}/${patientId}`);
  }

  addMedicalRecord(record: MedicalRecord): Observable<MedicalRecord> {
    debugger
    return this.http.post<MedicalRecord>(this.baseUrl, record);
  }
}