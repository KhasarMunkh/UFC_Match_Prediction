using Microsoft.ML.Data;

namespace MLexperiment.Common
{
    public interface ITrainerBase
    {
        string Name { get; }
        void Fit(string trainingFileName);
        BinaryClassificationMetrics Evaluate();
        void PrintModelMetrics(BinaryClassificationMetrics modelMetrics);
        void Save();
    }
}