import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MedicalRecordComponent } from './medical-record/medical-record.component';

const routes: Routes = [
  { path: ':patientId', component: MedicalRecordComponent }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MedicalRecordsRoutingModule { }
