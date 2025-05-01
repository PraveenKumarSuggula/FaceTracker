import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { StudentDashboardComponent } from './components/dashboard/student-dashboard/student-dashboard.component';
import { TeacherDashboardComponent } from './components/dashboard/teacher-dashboard/teacher-dashboard.component';
import { AdminDashboardComponent } from './components/dashboard/admin-dashboard/admin-dashboard.component';
import { MeetingRoomComponent } from './components/meeting-room/meeting-room.component';
import { AuthGuard } from './guards/auth.guard';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  
  { path: 'dashboard/student', component: StudentDashboardComponent, canActivate: [AuthGuard], data: { roles: ['Student'] } },
  { path: 'dashboard/teacher', component: TeacherDashboardComponent, canActivate: [AuthGuard], data: { roles: ['Teacher'] } },
  { path: 'dashboard/admin', component: AdminDashboardComponent, canActivate: [AuthGuard], data: { roles: ['Admin'] } },

  { path: 'meeting-room', component: MeetingRoomComponent, canActivate: [AuthGuard], data: { roles: ['Teacher', 'Student', 'Admin'] } },

  { path: '**', redirectTo: '/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
