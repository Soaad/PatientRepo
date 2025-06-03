

import { Component } from '@angular/core';
import { Patient } from 'src/app/models/patient.model';
import { PatientService } from '../patient.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-patient-list',
  templateUrl: './patient-list.component.html',
  styleUrls: ['./patient-list.component.scss']
})
export class PatientListComponent {
  patients: Patient[] = [];
  newPatient: Patient = { id: "00000000-0000-0000-0000-000000000000", fullName: '', dateOfBirth: '' ,encryptedEmail:'',encryptedPhone:'',address:''}; // Adjust fields as needed
  editPatient: Patient | null = null;

  constructor(private patientService: PatientService,private router: Router) {}

  ngOnInit() {
    this.loadPatients();
  }

  loadPatients() {
    this.patientService.getAllPatients().subscribe(data => this.patients = data);
  }

  addPatient() {
    debugger
    this.patientService.createPatient(this.newPatient).subscribe(() => {
      this.newPatient = { id: '', fullName: '', dateOfBirth: '' ,encryptedEmail:'',encryptedPhone:'',address:''}; // Reset the form
      this.loadPatients();
    });
  }

  startEdit(patient: Patient) {
  // this.editPatient = { ...patient };
   const date = patient.dateOfBirth ? patient.dateOfBirth.slice(0, 10) : '';
  this.editPatient = { ...patient, dateOfBirth: date };
  }

  updatePatient() {
    debugger
    if (this.editPatient) {
      this.patientService.updatePatient(this.editPatient.id, this.editPatient).subscribe(() => {
        this.editPatient = null;
        this.loadPatients();
      });
    }
  }

  cancelEdit() {
    this.editPatient = null;
  }

  deletePatient(id: string) {
    if (confirm('Are you sure you want to delete this patient?')) {
      this.patientService.deletePatient(id).subscribe(() => this.loadPatients());
    }
  }

   openMedicalRecord(patientId: string) {
    this.router.navigate(['/medical-records', patientId]);
  }
}


