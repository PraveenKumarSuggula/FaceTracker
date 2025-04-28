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

            // Load model if exists
            if (File.Exists(_modelPath))
            {
                _recognizer.Read(_modelPath);
            }
        }

        public int Recognize(Mat faceImage)
        {
            if (!File.Exists(_modelPath))
                return -1;

            _recognizer.Predict(faceImage, out int label, out double confidence);
            return (confidence < 50) ? label : -1;  // Adjust threshold as needed
        }

        public Mat ConvertBase64ToMat(string base64Image)
        {
            var base64Data = base64Image.Contains(",")
                ? base64Image.Substring(base64Image.IndexOf(",") + 1)
                : base64Image;

            byte[] imageBytes = Convert.FromBase64String(base64Data);
            Mat mat = Cv2.ImDecode(imageBytes, ImreadModes.Grayscale);
            return mat;
        }

        public string SaveFaceImage(Mat faceMat, int userId)
        {
            string folderPath = Path.Combine(_faceDataPath, "CapturedFaces", userId.ToString());
            Directory.CreateDirectory(folderPath);

            string fileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}.jpg";
            string fullPath = Path.Combine(folderPath, fileName);

            Cv2.ImWrite(fullPath, faceMat);
            return fullPath;
        }

        public void RegisterFace(Mat faceMat, int userId)
        {
            string folderPath = Path.Combine(_faceDataPath, "RegisteredFaces", userId.ToString());
            Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, $"{DateTime.UtcNow:yyyyMMdd_HHmmss}.jpg");
            Cv2.ImWrite(filePath, faceMat);

            TrainRecognizer();  // Retrain model after registering new face
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
                    images.Add(Cv2.ImRead(imgPath, ImreadModes.Grayscale));
                    labels.Add(label);
                }
            }

            if (images.Count > 0)
            {
                _recognizer.Train(images, labels);
                _recognizer.Write(_modelPath);  // Save model consistently
            }
        }
    }
}
