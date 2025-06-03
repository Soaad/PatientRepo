import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MedicalRecordsRoutingModule } from './medical-records-routing.module';
import { FormsModule } from '@angular/forms';
import { MedicalRecordComponent } from './medical-record/medical-record.component';


@NgModule({
  declarations: [MedicalRecordComponent],
  imports: [
    CommonModule,
    FormsModule,
    MedicalRecordsRoutingModule
  ]
})
export class MedicalRecordsModule { }
