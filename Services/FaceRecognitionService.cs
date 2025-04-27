using System;
using OpenCvSharp;
using OpenCvSharp.Face;

namespace facetrackr_backend.Services
{
    public class FaceRecognitionService
    {
        private readonly string _faceDataPath = "FaceData/";
        private readonly LBPHFaceRecognizer _recognizer;

        public FaceRecognitionService()
        {
            _recognizer = LBPHFaceRecognizer.Create();
        }

        public void Train(int userId, Mat faceImage)
        {
            var labels = new List<int> { userId };
            var images = new List<Mat> { faceImage };
            _recognizer.Train(images, labels);
            _recognizer.Write(Path.Combine(_faceDataPath, "model.yml"));
        }

        public int Recognize(Mat faceImage)
        {
            _recognizer.Read(Path.Combine(_faceDataPath, "model.yml"));
            _recognizer.Predict(faceImage, out int label, out double confidence);
            return label;  // Return userId or -1 if not recognized
        }
    }

}
