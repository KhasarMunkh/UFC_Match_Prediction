using Microsoft.ML;
using MLexperiment.DataModels;

public class Predictor
{
    protected static string ModelPath => Path.Combine(AppContext.BaseDirectory, "classification.mdl");
    private readonly MLContext _mlContext;

    private ITransformer _model;

    public Predictor()
    {
        _mlContext = new MLContext(seed: 42);
    }

    //Returns prediction on new data 
    public UFCMatchPrediction Predict(UFCMatchData newSample)
    {
        LoadModel();
        
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<UFCMatchData, UFCMatchPrediction>(_model);
        return predictionEngine.Predict(newSample);
    }
    private void LoadModel()
    {
        if (!File.Exists(ModelPath))
        {
            throw new FileNotFoundException($"Model file '{ModelPath}' not found.");
        }

        using (var stream = new FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            _model = _mlContext.Model.Load(stream, out _);
        }

        if (_model == null)
        {
            throw new Exception($"Failed to load model from '{ModelPath}'");
        }
    }
}