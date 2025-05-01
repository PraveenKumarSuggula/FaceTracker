import { Injectable } from '@angular/core';
import * as faceapi from 'face-api.js';
import { AuthService } from './auth.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class FaceService {
  private labeledDescriptors: faceapi.LabeledFaceDescriptors[] = [];
  private faceMatcher: faceapi.FaceMatcher | null = null;
  private backendUrl = 'https://localhost:44322/api';

  constructor(private http: HttpClient, private auth: AuthService) {}

  async loadModels() {
    const url = 'https://facetrackr-models.vercel.app';
    await Promise.all([
      faceapi.nets.ssdMobilenetv1.loadFromUri(`${url}/ssd_mobilenetv1`),
      faceapi.nets.faceLandmark68Net.loadFromUri(`${url}/face_landmark_68`),
      faceapi.nets.faceRecognitionNet.loadFromUri(`${url}/face_recognition`),
    ]);
  }

  async loadLabeledFaces(): Promise<void> {
    const token = this.auth.getToken();
    const registered = await fetch(`${this.backendUrl}/faceregistration/all`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    }).then(res => res.json());
  
    this.labeledDescriptors = registered.map((user: any) => {
      const descriptors = user.descriptors.map((desc: number[]) => new Float32Array(desc));
      return new faceapi.LabeledFaceDescriptors(user.name, descriptors);
    });
  
    this.faceMatcher = new faceapi.FaceMatcher(this.labeledDescriptors);
  }  

  async detectAndDraw(
    video: HTMLVideoElement,
    canvas: HTMLCanvasElement
  ): Promise<number> {
    const result = await faceapi
      .detectSingleFace(video)
      .withFaceLandmarks()
      .withFaceDescriptor();
  
    if (!result || !this.faceMatcher) return 0;
  
    const dims = faceapi.matchDimensions(canvas, video, true);
    const resized = faceapi.resizeResults(result, dims);
    const match = this.faceMatcher.findBestMatch(result.descriptor);
  
    const box = resized.detection.box;
    const context = canvas.getContext('2d')!;
    context.clearRect(0, 0, canvas.width, canvas.height);
    context.strokeStyle = '#00ff00';
    context.lineWidth = 2;
    context.strokeRect(box.x, box.y, box.width, box.height);
  
    const confidence = Math.max(0, (1 - match.distance)) * 100;
    const labelText = match.label === 'unknown'
      ? `Unknown (${confidence.toFixed(1)}%)`
      : `${match.label} (${confidence.toFixed(1)}%)`;
  
    context.fillStyle = '#00ff00';
    context.font = '16px Arial';
    context.fillText(labelText, box.x, box.y - 10);
  
    return Math.round(confidence);
  }    

  async captureFace(video: HTMLVideoElement): Promise<string> {
    const canvas = document.createElement('canvas');
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    const ctx = canvas.getContext('2d')!;
    ctx.drawImage(video, 0, 0, canvas.width, canvas.height);
    return canvas.toDataURL('image/jpeg').split(',')[1]; // return base64 only
  }

  async registerFace(userId: number, faceBase64: string): Promise<any> {
    const payload = { userId, faceBase64 };
    return this.http
      .post(
        `${this.backendUrl}/faceregistration/register`,
        payload,
        this.auth.getAuthHeaders()
      )
      .toPromise();
  }
}
