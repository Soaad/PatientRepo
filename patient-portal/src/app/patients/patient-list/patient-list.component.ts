import { Component } from '@angular/core';
import { Patient } from 'src/app/models/patient.model';
import { PatientService } from '../patient.service';

@Component({
  selector: 'app-patient-list',
  templateUrl: './patient-list.component.html',
  styleUrls: ['./patient-list.component.scss']
})
export class PatientListComponent {
patients: Patient[] = [];
constructor(private patientService: PatientService) {}
ngOnInit() {
  this.patientService.getAllPatients().subscribe(data => this.patients = data);
}
}
