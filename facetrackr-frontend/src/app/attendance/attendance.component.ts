import { Component, OnInit, AfterViewInit } from '@angular/core';
import * as faceapi from 'face-api.js';

@Component({
  selector: 'app-attendance',
  templateUrl: './attendance.component.html',
  styleUrls: ['./attendance.component.scss']
})
export class AttendanceComponent implements OnInit, AfterViewInit {
  async ngOnInit() {
    await Promise.all([
      faceapi.nets.ssdMobilenetv1.loadFromUri('https://facetrackr-models.vercel.app/ssd_mobilenetv1'),
      faceapi.nets.faceLandmark68Net.loadFromUri('https://facetrackr-models.vercel.app/face_landmark_68'),
      faceapi.nets.faceRecognitionNet.loadFromUri('https://facetrackr-models.vercel.app/face_recognition')
    ]);
    console.log("Face API models loaded!");
  }

  async ngAfterViewInit() {
    const video = <HTMLVideoElement>document.getElementById('video');
    navigator.mediaDevices.getUserMedia({ video: {} })
      .then(stream => video.srcObject = stream)
      .catch(err => console.error(err));

    video.addEventListener('play', async () => {
      const canvas = faceapi.createCanvasFromMedia(video);
      document.body.append(canvas);
      const displaySize = { width: video.width, height: video.height };
      faceapi.matchDimensions(canvas, displaySize);

      setInterval(async () => {
        const detections = await faceapi.detectAllFaces(video).withFaceLandmarks().withFaceDescriptors();
        canvas.getContext('2d')?.clearRect(0, 0, canvas.width, canvas.height);
        faceapi.draw.drawDetections(canvas, faceapi.resizeResults(detections, displaySize));
      }, 1000);
    });
  }
}
