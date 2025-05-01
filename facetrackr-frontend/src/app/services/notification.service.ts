// src/app/services/notification.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private apiUrl = 'https://localhost:44322/api/notification';

  constructor(private http: HttpClient, private auth: AuthService) {}

  sendNotification(title: string, message: string) {
    const body = { title, message };
    return this.http.post(`${this.apiUrl}/send`, body, this.auth.getAuthHeaders());
  }
}
