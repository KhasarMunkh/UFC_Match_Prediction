using Microsoft.ML;
using MLexperiment.DataModels;

namespace MLexperiment.Predictors
{
    public class Predictor
    {
        private readonly MLContext _mlContext;
        private readonly string _modelPath;
        private ITransformer _model = null!;

        public Predictor(string modelPath)
        {
            if (string.IsNullOrWhiteSpace(modelPath))
            {
                throw new ArgumentException("Model path must not be empty.", nameof(modelPath));
            }

            _mlContext = new MLContext(seed: 42);
            _modelPath = modelPath;
        }

        /// <summary>
        /// Returns a prediction on new data using the loaded model.
        /// </summary>
        public UFCMatchPrediction Predict(UFCMatchData newSample)
        {
            LoadModel();

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<UFCMatchData, UFCMatchPrediction>(_model);
            return predictionEngine.Predict(newSample);
        }

        private void LoadModel()
        {
            if (!File.Exists(_modelPath))
            {
                throw new FileNotFoundException($"Model file '{_modelPath}' not found.");
            }

            using (var stream = new FileStream(_modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _model = _mlContext.Model.Load(stream, out _);
            }

            if (_model == null)
            {
                throw new InvalidOperationException($"Failed to load model from '{_modelPath}'");
            }
        }
    }
}