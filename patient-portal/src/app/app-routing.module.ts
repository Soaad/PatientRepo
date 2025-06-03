import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { MedicalRecordComponent } from './medical-records/medical-record/medical-record.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'patients', loadChildren: () => import('./patients/patients.module').then(m => m.PatientsModule) },
  { path: 'medical-records/:patientId', component: MedicalRecordComponent },
    { path: 'medical-records', loadChildren: () => import('./medical-records/medical-records.module').then(m => m.MedicalRecordsModule) },

  { path: '', redirectTo: 'login', pathMatch: 'full' }
];


@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
