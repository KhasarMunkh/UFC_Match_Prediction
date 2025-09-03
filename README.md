# MLexperiment: UFC Fight Outcome Prediction with ML.NET

This project uses [ML.NET](https://dotnet.microsoft.com/en-us/apps/machinelearning-ai/ml-dotnet) to predict the outcome of UFC fights using fighter statistics and event data.

## Data Source

- [UFC Complete Dataset (1996-2024) on Kaggle](https://www.kaggle.com/datasets/maksbasher/ufc-complete-dataset-all-events-1996-2024)

## Features

- **Binary classification** using Random Forest (Fast Forest) algorithm
- Data preprocessing and feature engineering
- Modular trainer architecture (`TrainerBase`, `RandomForestTrainer`)
- Prediction API (`Predictor`)
- Extensible data model (`UFCMatchData`)

## Project Structure

```
src/
  Common/           # Core interfaces and abstract trainer
  Data/             # CSV datasets
  DataModels/       # Data model classes
  Predictors/       # Prediction logic
  Trainers/         # ML.NET trainer implementations
  Program.cs        # Main entry point
```

## Example Results
```csharp
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
```
```
***************************************
Training with Random Forest (32 leaves, 200 trees)...
***************************************
F1 Score: 79.76%
Accuracy : 68.95%
Positive Precision: 69.72%
Negative Precision: 63.44%
Positive Recall: 93.19%
Negative Recall: 22.61%
Area Under Precision Recall Curve: 0.8378
----------------------------------------
Prediction for Fighter A vs Fighter B: Fighter A wins
----------------------------------------
***************************************
Training with Random Forest (64 leaves, 400 trees)...
***************************************
F1 Score: 79.92%
Accuracy : 69.47%
Positive Precision: 70.34%
Negative Precision: 63.99%
Positive Recall: 92.52%
Negative Recall: 25.42%
Area Under Precision Recall Curve: 0.8429
----------------------------------------
Prediction for Fighter A vs Fighter B: Fighter A wins
----------------------------------------
***************************************
Training with Random Forest (128 leaves, 800 trees)...
***************************************
F1 Score: 79.49%
Accuracy : 69.21%
Positive Precision: 70.65%
Negative Precision: 61.41%
Positive Recall: 90.85%
Negative Recall: 27.84%
Area Under Precision Recall Curve: 0.8457
----------------------------------------
Prediction for Fighter A vs Fighter B: Fighter A wins
----------------------------------------
***************************************
Training with Random Forest (30 leaves, 800 trees)...
***************************************
F1 Score: 79.74%
Accuracy : 68.86%
Positive Precision: 69.61%
Negative Precision: 63.37%
Positive Recall: 93.32%
Negative Recall: 22.09%
Area Under Precision Recall Curve: 0.8381
----------------------------------------
Prediction for Fighter A vs Fighter B: Fighter A wins
```
## License

See [Kaggle dataset license](https://www.kaggle.com/datasets/maksbasher/ufc-complete-dataset-all-events-1996-2024)
