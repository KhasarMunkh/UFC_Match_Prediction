using MLexperiment.Common;
using MLexperiment.DataModels;
using MLexperiment.Trainers;

namespace MLexperiment
{
    class Program
    {
        static void Main(string[] args)
        {
            var newSample = new UFCMatchData
            {
                Event_Name = "UFC 100",
                R_fighter = "Fighter A",
                R_Stance = "Orthodox",
                B_fighter = "Fighter B",
                B_Stance = "Southpaw",
                Weight_Class = "Welterweight",
                Is_Title_Bout = false,
                Gender = "Male",
                Wins_Total_Diff = 5,
                Losses_Total_Diff = 2,
                Age_Diff = 3,
                Height_Diff = 2,
                Reach_Diff = 1,
                Weight_Diff = 3,
                TD_Def_Diff = 0.5f,
                Sub_Diff = 1,
                TD_Diff = 0.2f
            };
            var trainers = new List<ITrainerBase>
            {
                new RandomForestTrainer(32, 200),
                new RandomForestTrainer(64, 400),
                new RandomForestTrainer(128, 800),
                new RandomForestTrainer(30, 800)
            };

            trainers.ForEach(trainer => TrainEvaluatePredict(trainer, newSample, args));
        }

        static void TrainEvaluatePredict(ITrainerBase trainer, UFCMatchData newSample, string[] args)
        {
            Console.WriteLine("***************************************");
            Console.WriteLine($"Training with {trainer.Name}...");
            Console.WriteLine("***************************************");

            var datasetPath = args.Length > 0
                ? args[0]
                : Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Data", "large_dataset.csv");

            trainer.Fit(datasetPath);

            var modelMetrics = trainer.Evaluate();
            trainer.PrintModelMetrics(modelMetrics);
            trainer.Save();

            var predictor = new Predictor();
            var prediction = predictor.Predict(newSample);
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Prediction for {newSample.R_fighter} vs {newSample.B_fighter}: " +
                              $"{(prediction.PredictedLabel ? newSample.R_fighter : newSample.B_fighter)} wins"); 
            Console.WriteLine("----------------------------------------");
        }
    }
}