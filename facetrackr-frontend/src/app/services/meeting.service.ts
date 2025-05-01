import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class MeetingService {

  constructor(private http: HttpClient, private auth: AuthService) {}

  private hubConnection!: signalR.HubConnection;

  private baseUrl = 'https://localhost:44322/api/';

  startConnection(token: string) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:44322/videoHub', {
        accessTokenFactory: () => token
      })
      .build();

    this.hubConnection.start().catch(err => console.error('SignalR Error:', err));
  }

  stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  generateMeetingId() {
    return this.http.get(`${this.baseUrl}meeting/generate`, this.auth.getAuthHeaders());
  }

  startMeeting(meetingId: string) {
    return this.http.post(`${this.baseUrl}meeting/start`, { meetingId }, this.auth.getAuthHeaders());
  }

  endMeeting(meetingId: string) {
    return this.http.post(`${this.baseUrl}meeting/end`, { meetingId }, this.auth.getAuthHeaders());
  }

  checkMeetingStatus(meetingId: string) {
    return this.http.get(`${this.baseUrl}meeting/status/${meetingId}`, this.auth.getAuthHeaders());
  }

  downloadAttendanceReportByMeeting(meetingId: string) {
    return this.http.get(`${this.baseUrl}attendance/report-by-meeting/${meetingId}`, {
      headers: this.auth.getAuthHeaders().headers,
      responseType: 'blob'
    });
  }
}
