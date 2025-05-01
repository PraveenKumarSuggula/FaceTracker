// src/app/components/dashboard/admin-dashboard/admin-dashboard.component.ts
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent {
  constructor(private auth: AuthService, private router: Router) {}

  startMeeting() {
    this.router.navigate(['/meeting-room']);
  }

  logout() {
    this.auth.logout();
  }
}
