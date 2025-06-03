import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MedicalRecord } from 'src/app/models/medicalrecord.model';
import { MedicalRecordService } from '../medicalrecordservice';

@Component({
  selector: 'app-medical-record',
  templateUrl: './medical-record.component.html',
  styleUrls: ['./medical-record.component.scss']
})
export class MedicalRecordComponent implements OnInit {
 patientId: string = '';
  records: MedicalRecord[] = [];
  newRecord: MedicalRecord = { patientId: '', diagnosis: '', notes: '', lastUpdated: '' };

  constructor(
    private route: ActivatedRoute,
    private medicalRecordService: MedicalRecordService
  ) {}

  ngOnInit() {
    this.patientId = this.route.snapshot.paramMap.get('patientId') || '';
    this.newRecord.patientId = this.patientId;
    this.loadRecords();
  }

  loadRecords() {
    this.medicalRecordService.getMedicalRecords(this.patientId)
      .subscribe(records => this.records = records);
      console.log(this.records);
  }

  addRecord() {
    if (!this.newRecord.diagnosis || !this.newRecord.notes || !this.newRecord.lastUpdated) return;
    this.medicalRecordService.addMedicalRecord(this.newRecord)
      .subscribe(() => {
        this.newRecord = { patientId: this.patientId, diagnosis: '', notes: '', lastUpdated: '' };
        this.loadRecords();
      });
  }
}
