import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-student-dashboard',
  templateUrl: './student-dashboard.component.html',
  styleUrls: ['./student-dashboard.component.scss']
})
export class StudentDashboardComponent {
  meetingId = '';

  constructor(private auth: AuthService, private router: Router) {}

  joinMeeting() {
    if (!this.meetingId) {
      alert('Please enter a meeting ID.');
      return;
    }
    this.router.navigate(['/meeting-room'], { queryParams: { id: this.meetingId } });
  }

  logout() {
    this.auth.logout();
  }
}
