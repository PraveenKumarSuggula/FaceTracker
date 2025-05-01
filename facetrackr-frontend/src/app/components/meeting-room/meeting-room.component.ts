import { Component, OnInit, OnDestroy } from '@angular/core';
import { FaceService } from '../../services/face.service';
import { AuthService } from '../../services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { MeetingService } from 'src/app/services/meeting.service';
import { AttendanceService } from 'src/app/services/attendance.service';

@Component({
  selector: 'app-meeting-room',
  templateUrl: './meeting-room.component.html',
  styleUrls: ['./meeting-room.component.scss']
})
export class MeetingRoomComponent implements OnInit, OnDestroy {
  videoElement!: HTMLVideoElement;
  intervalId: any;
  role: string | null = '';
  userId: number | null = null;
  meetingId = '';
  isStudentJoined = false;
  isMeetingStarted = false;
  meetingActive = false;
  faceRegistered = false;
  matchedName = '';
  lastConfidence: number = 0;

  constructor(
    private attendanceService: AttendanceService,
    private faceService: FaceService,
    private auth: AuthService,
    private route: ActivatedRoute,
    private router: Router,
    private meeting: MeetingService
  ) {}

  ngOnInit() {
    this.role = this.auth.getRole();
    const payload = this.auth.getPayload();
    this.userId = payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ?? null;
    this.route.queryParams.subscribe(params => {
      this.meetingId = params['id'] || '';
    });
    this.setupVideo();
    // if (this.role === 'Student') {
    //   setInterval(() => {
    //     this.meeting.checkMeetingStatus(this.meetingId).subscribe((res: any) => {
    //       if (!res.isActive && this.isStudentJoined) {
    //         alert("Meeting ended by teacher.");
    //         this.exitMeeting();
    //       }
    //     });
    //   }, 100000); // check every 1000s
    // }
  }

  async setupVideo() {
    this.videoElement = document.getElementById('video') as HTMLVideoElement;
    const canvas = document.getElementById('overlay') as HTMLCanvasElement;
    const stream = await navigator.mediaDevices.getUserMedia({ video: true });
    this.videoElement.srcObject = stream;
    this.videoElement.play();
  
    await this.faceService.loadModels();
    await this.faceService.loadLabeledFaces();
  
    setInterval(async () => {
      const confidence = await this.faceService.detectAndDraw(this.videoElement, canvas);
      if (confidence > 0) this.lastConfidence = confidence;
    }, 1000);
  }
  

  async registerFace() {
    const base64 = await this.faceService.captureFace(this.videoElement);
    if (this.userId) {
      await this.faceService.registerFace(this.userId, base64);
      this.faceRegistered = true;
      alert('Face registered successfully!');
    }
  }

  async startMeeting() {
    this.isMeetingStarted = true;
    this.meetingActive = true;
  
    if (!this.meetingId) {
      // Only teacher generates the ID
      const res: any = await this.meeting.generateMeetingId().toPromise();
      this.meetingId = res.meetingId;
    }
  
    // Inform backend to mark meeting as started
    await this.meeting.startMeeting(this.meetingId).toPromise();
    //this.startAttendance();
  }  

  async joinMeeting() {
    if (!this.faceRegistered) {
      alert('Please register your face before joining!');
      return;
    }
    this.isStudentJoined = true;
    this.startAttendance();
  }

  startAttendance() {
    this.intervalId = setInterval(async () => {
      const base64 = await this.faceService.captureFace(this.videoElement);
      const res: any = await this.attendanceService.markAttendance(base64, this.userId!, this.meetingId);
      this.lastConfidence = Math.round(100 - res.confidence); // Lower = better
      this.matchedName = `${res.name} (ID: ${res.label})`;
    }, 10000); //10s
  }

  endMeeting() {
    clearInterval(this.intervalId);
    this.isMeetingStarted = false;
    this.meetingActive = false;
    alert('Meeting ended. Attendance report will be generated.');
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

  exitMeeting() {
    if (this.role === 'Teacher') {
    this.downloadReport()
    }
    this.router.navigate([`/dashboard/${this.role?.toLowerCase()}`]);
  }

  ngOnDestroy() {
    clearInterval(this.intervalId);
  }
}
