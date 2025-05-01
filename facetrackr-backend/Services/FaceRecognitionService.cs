using OpenCvSharp;
using OpenCvSharp.Face;

namespace facetrackr_backend.Services
{
    public class FaceRecognitionService
    {
        private readonly string _faceDataPath = Path.Combine(Directory.GetCurrentDirectory(), "FaceData");
        private readonly string _modelPath;
        private readonly LBPHFaceRecognizer _recognizer;

        public FaceRecognitionService()
        {
            _modelPath = Path.Combine(_faceDataPath, "lbph_model.xml");
            _recognizer = LBPHFaceRecognizer.Create();

            if (File.Exists(_modelPath))
            {
                _recognizer.Read(_modelPath);
            }
        }

        public Rect[] DetectFaces(Mat mat)
        {
            var cascadePath = Path.Combine(Directory.GetCurrentDirectory(), "Services", "haarcascade_frontalface_default.xml");
            var cascade = new CascadeClassifier(cascadePath);
            return cascade.DetectMultiScale(mat, 1.1, 4);
        }

        public Mat ConvertBase64ToMat(string base64Image)
        {
            var base64Data = base64Image.Contains(",")
                ? base64Image.Substring(base64Image.IndexOf(",") + 1)
                : base64Image;

            byte[] imageBytes = Convert.FromBase64String(base64Data);
            return Cv2.ImDecode(imageBytes, ImreadModes.Grayscale);
        }

        public void RegisterFace(Mat faceMat, int userId)
        {
            var faces = DetectFaces(faceMat);
            if (faces.Length == 0) return;

            var cropped = new Mat(faceMat, faces[0]);
            Cv2.Resize(cropped, cropped, new Size(200, 200));
            Cv2.EqualizeHist(cropped, cropped); // optional: normalize lighting

            string folderPath = Path.Combine(_faceDataPath, "RegisteredFaces", userId.ToString());
            Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, $"{DateTime.UtcNow:yyyyMMdd_HHmmss}.jpg");
            Cv2.ImWrite(filePath, cropped);

            TrainRecognizer();
        }

        public (bool isSimilar, int label, double minConfidence) RecognizeRelaxed(Mat faceImage, int expectedUserId)
        {
            string studentFolder = Path.Combine(_faceDataPath, "RegisteredFaces", expectedUserId.ToString());

            if (!Directory.Exists(studentFolder)) return (false, -1, 999);

            Cv2.Resize(faceImage, faceImage, new Size(200, 200));
            Cv2.EqualizeHist(faceImage, faceImage);

            double minConfidence = double.MaxValue;
            foreach (var imgPath in Directory.GetFiles(studentFolder, "*.jpg"))
            {
                var registeredImg = Cv2.ImRead(imgPath, ImreadModes.Grayscale);
                Cv2.Resize(registeredImg, registeredImg, new Size(200, 200));
                Cv2.EqualizeHist(registeredImg, registeredImg);

                _recognizer.Predict(faceImage, out int predictedLabel, out double confidence);

                if (confidence < minConfidence)
                {
                    minConfidence = confidence;
                }

                if (confidence < 120) // relaxed threshold
                {
                    return (true, expectedUserId, confidence); // consider similar enough
                }
            }

            return (false, expectedUserId, minConfidence); // not similar enough
        }


        public string SaveFaceImage(Mat faceMat, int userId, string meetingId, string status)
        {
            string folderPath = Path.Combine(_faceDataPath, "CapturedFaces", meetingId, userId.ToString());
            Directory.CreateDirectory(folderPath);

            string fileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{status.ToLower()}.jpg";
            string fullPath = Path.Combine(folderPath, fileName);
            Cv2.ImWrite(fullPath, faceMat);

            return fullPath;
        }

        public void TrainRecognizer()
        {
            var images = new List<Mat>();
            var labels = new List<int>();

            string baseDir = Path.Combine(_faceDataPath, "RegisteredFaces");
            foreach (var userDir in Directory.GetDirectories(baseDir))
            {
                int label = int.Parse(Path.GetFileName(userDir));
                foreach (var imgPath in Directory.GetFiles(userDir, "*.jpg"))
                {
                    var img = Cv2.ImRead(imgPath, ImreadModes.Grayscale);
                    Cv2.Resize(img, img, new Size(200, 200));
                    Cv2.EqualizeHist(img, img);
                    images.Add(img);
                    labels.Add(label);
                }
            }

            if (images.Count > 0)
            {
                _recognizer.Train(images, labels);
                _recognizer.Write(_modelPath);
            }
        }
    }
}
