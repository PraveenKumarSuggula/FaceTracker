// src/app/services/feedback.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class FeedbackService {
  private apiUrl = 'https://localhost:44322/api/feedback';

  constructor(private http: HttpClient, private auth: AuthService) {}

  submitFeedback(comment: string) {
    const body = { comment };
    return this.http.post(`${this.apiUrl}/submit`, body, this.auth.getAuthHeaders());
  }
}
