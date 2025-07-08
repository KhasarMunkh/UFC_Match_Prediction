using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Trainers.FastTree;
using MLexperiment.Common;

namespace MLexperiment.Trainers
{
    /// <summary>
    /// Trainer for Random Forest classification.
    /// </summary>
    public class RandomForestTrainer : TrainerBase<FastForestBinaryModelParameters>
    {
        public RandomForestTrainer(int numberOfLeaves, int numberOfTrees) : base()
        {
            Name = $"Random Forest ({numberOfLeaves} leaves, {numberOfTrees} trees)";
            _model = _mlContext.BinaryClassification.Trainers.FastForest(numberOfLeaves: numberOfLeaves, numberOfTrees: numberOfTrees);
        }
    }
}