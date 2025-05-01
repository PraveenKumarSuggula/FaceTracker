import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { MeetingService } from 'src/app/services/meeting.service';

@Component({
  selector: 'app-teacher-dashboard',
  templateUrl: './teacher-dashboard.component.html',
  styleUrls: ['./teacher-dashboard.component.scss']
})
export class TeacherDashboardComponent {
  meetingId: string = '';

  constructor(
    private auth: AuthService,
    private router: Router,
    private meeting: MeetingService
  ) {}

  startMeeting() {
    this.meeting.generateMeetingId().subscribe((res: any) => {
      this.meetingId = res.meetingId; // store it locally
      this.router.navigate(['/meeting-room'], { queryParams: { id: this.meetingId } });
    });
  }

  downloadReport() {
    if (!this.meetingId) {
      alert('No meeting ID found. Please start a meeting first.');
      return;
    }

    this.meeting.downloadAttendanceReportByMeeting(this.meetingId).subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'Attendance_Report.pdf';
      a.click();
      window.URL.revokeObjectURL(url);
    }, error => {
      alert("Failed to generate report.");
    });
  }

  logout() {
    this.auth.logout();
  }
}
