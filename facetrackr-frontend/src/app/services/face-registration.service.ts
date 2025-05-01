// src/app/services/faceregistration.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class FaceRegistrationService {
  private apiUrl = 'https://localhost:44322/api/faceregistration';

  constructor(private http: HttpClient, private auth: AuthService) {}

  registerFace(userId: number, faceBase64: string) {
    const body = { userId, faceBase64 };
    return this.http.post(`${this.apiUrl}/register`, body, this.auth.getAuthHeaders());
  }
}
