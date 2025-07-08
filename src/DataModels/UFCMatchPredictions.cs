// Represents the data model for UFC match predictions. (Models output data)
namespace MLexperiment.DataModels
{
    /// <summary>
    /// Represents the prediction for a UFC match outcome.
    /// The PredictedLabel property indicates whether the red fighter is predicted to win (true)
    /// </summary>
    public class UFCMatchPrediction
    {
        public bool PredictedLabel { get; set; }
    }
}