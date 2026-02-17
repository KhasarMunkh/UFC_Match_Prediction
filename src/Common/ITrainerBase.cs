using Microsoft.ML.Data;

namespace MLexperiment.Common
{
    public interface ITrainerBase
    {
        string Name { get; }
        string ModelPath { get; }
        void Fit(string trainingFileName);
        BinaryClassificationMetrics Evaluate();
        void PrintModelMetrics(BinaryClassificationMetrics modelMetrics);
        void Save();
    }
}