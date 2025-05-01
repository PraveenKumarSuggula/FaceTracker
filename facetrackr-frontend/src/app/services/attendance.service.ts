// src/app/services/attendance.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class AttendanceService {
  private apiUrl = 'https://localhost:44322/api/attendance';

  constructor(private http: HttpClient, private auth: AuthService) {}

  async markAttendance(base64: string, userId: number, meetingId: string): Promise<any> {
    const payload = { faceBase64: base64, userId, meetingId };
    return this.http.post(`${this.apiUrl}/mark`, payload, this.auth.getAuthHeaders()).toPromise();
  }
  
}
