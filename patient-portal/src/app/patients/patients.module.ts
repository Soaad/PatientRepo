import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PatientsRoutingModule } from './patients-routing.module';
import { PatientListComponent } from './patient-list/patient-list.component';
import { FormsModule } from '@angular/forms';


@NgModule({
  declarations: [
    PatientListComponent,
   // PatientDetailComponent
  ],
  imports: [
    CommonModule,
    FormsModule, 
    PatientsRoutingModule
  ]
})
export class PatientsModule { }
