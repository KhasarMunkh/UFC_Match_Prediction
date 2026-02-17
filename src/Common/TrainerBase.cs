// abstract TrainerBase class. 
// This class is in the Common folder and its main goal is to standardize the way this whole process is done. 
// It is in this class where we process data and perform feature engineering. 
// This class is also in charge of training machine learning algorithm. 
// The classes that implement this abstract class are located in the Trainers folder.

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using MLexperiment.DataModels;
namespace MLexperiment.Common
{
    public abstract class TrainerBase<TParameters> : ITrainerBase where TParameters : class
    {
        public string Name { get; protected set; } = string.Empty;
        protected static string ModelPath => Path.Combine(AppContext.BaseDirectory, "classification.mdl");
        protected readonly MLContext _mlContext;
        protected DataOperationsCatalog.TrainTestData? _dataSplit;
        protected ITrainerEstimator<BinaryPredictionTransformer<TParameters>, TParameters> _model = null!;
        protected ITransformer _trainedModel = null!;

        protected TrainerBase()
        {
            _mlContext = new MLContext(seed: 42);
        }

        /// <summary>
        /// Fits the model using the training dataset.
        /// </summary>
        public void Fit(string trainingFileName)
        {
            if (!File.Exists(trainingFileName))
            {
                throw new FileNotFoundException($"Training file '{trainingFileName}' not found.");
            }

            _dataSplit = LoadAndPrepareData(trainingFileName);
            var dataProcessPipeline = BuildDataProcessingPipeline();

            // Create the complete training pipeline by appending the ML algorithm
            var trainingPipeline = dataProcessPipeline.Append(_model);

            // Train the model
            _trainedModel = trainingPipeline.Fit(_dataSplit.Value.TrainSet);
        }
        // /// <summary>
        /// Evaluates the model using the test dataset and returns the metrics.
        /// </summary>
        public BinaryClassificationMetrics Evaluate()
        {
            if (_dataSplit == null || _trainedModel == null)
            {
                throw new InvalidOperationException("Model must be trained before evaluation. Call Fit() first.");
            }

            var testSetTransform = _trainedModel.Transform(_dataSplit.Value.TestSet);

            return _mlContext.BinaryClassification.EvaluateNonCalibrated(testSetTransform);
        }
        public void Save()
        {
            if (_dataSplit == null || _trainedModel == null)
            {
                throw new InvalidOperationException("Model must be trained before saving. Call Fit() first.");
            }

            _mlContext.Model.Save(_trainedModel, _dataSplit.Value.TrainSet.Schema, ModelPath);
        }
        private IEstimator<ITransformer> BuildDataProcessingPipeline()
        {
            // Turn the label column into a boolean type
            // This is necessary to ensure the model can interpret the winner correctly
            //  build an in-memory lookup (could also come from a small CSV)

            // Define the data processing pipeline for UFC match prediction
            var dataProcessPipeline = _mlContext.Transforms.Categorical.OneHotEncoding("Weight_Class_Encoded", "Weight_Class")
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding("Gender_Encoded", "Gender"))
                .Append(_mlContext.Transforms.Conversion.MapValue("Label",
                    new Dictionary<string, bool>
                    {
                        { "Red", true },  // R_fighter is the winner
                        { "Blue", false }  // Red is not the winner
                    }, "Winner"))
                // Convert boolean to float for processing
                .Append(_mlContext.Transforms.Conversion.ConvertType("Is_Title_Bout_Float", "Is_Title_Bout", DataKind.Single))
                // Concatenate all numeric features into a single Features vector
                .Append(_mlContext.Transforms.Concatenate("Features",
                    "Weight_Class_Encoded",
                    "Gender_Encoded",
                    "Is_Title_Bout_Float",
                    "Wins_Total_Diff",
                    "Losses_Total_Diff",
                    "Age_Diff",
                    "Height_Diff",
                    "Weight_Diff",
                    "Reach_Diff",
                    "TD_Def_Diff",
                    "Sub_Diff",
                    "TD_Diff"))
                // Normalize features to improve training performance  
                .Append(_mlContext.Transforms.NormalizeMinMax("Features", "Features"))
                // Cache the processed data for faster training iterations
                .AppendCacheCheckpoint(_mlContext);
            return dataProcessPipeline;

        }
        private DataOperationsCatalog.TrainTestData LoadAndPrepareData(string trainingFileName)
        {
            // Load the data from the specified file
            IDataView dataView = _mlContext.Data.LoadFromTextFile<UFCMatchData>(trainingFileName, separatorChar: ',', hasHeader: true);
            // Split the data into training and test sets
            return _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.3);
        }
        
        public void PrintModelMetrics(BinaryClassificationMetrics modelMetrics)
        {
            Console.WriteLine($"F1 Score: {modelMetrics.F1Score:P2}" +
                              $"\nAccuracy : {modelMetrics.Accuracy:P2}" +
                              $"\nPositive Precision: {modelMetrics.PositivePrecision:P2}" +
                              $"\nNegative Precision: {modelMetrics.NegativePrecision:P2}" +
                              $"\nPositive Recall: {modelMetrics.PositiveRecall:P2}" +
                              $"\nNegative Recall: {modelMetrics.NegativeRecall:P2}" +
                              $"\nArea Under Precision Recall Curve: {modelMetrics.AreaUnderPrecisionRecallCurve:F4}");
        }
    }
}