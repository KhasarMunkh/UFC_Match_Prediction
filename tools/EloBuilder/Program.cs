using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace EloBuilder
{
    class Program
    {
        private const double DefaultElo = 1500.0;
        private const double KFactor = 32.0;

        static int Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Console.WriteLine("Usage: EloBuilder <input.csv> [output.csv]");
                Console.WriteLine();
                Console.WriteLine("Reads a UFC dataset CSV and appends pre-fight Elo columns:");
                Console.WriteLine("  r_elo       - Red corner Elo before the fight");
                Console.WriteLine("  b_elo       - Blue corner Elo before the fight");
                Console.WriteLine("  elo_diff    - Red Elo minus Blue Elo (R - B)");
                Console.WriteLine();
                Console.WriteLine("If output path is omitted, writes to <input>_with_elo.csv");
                return 1;
            }

            var inputPath = args[0];
            if (!File.Exists(inputPath))
            {
                Console.Error.WriteLine($"Error: Input file '{inputPath}' not found.");
                return 1;
            }

            var outputPath = args.Length >= 2
                ? args[1]
                : Path.Combine(
                    Path.GetDirectoryName(inputPath) ?? ".",
                    Path.GetFileNameWithoutExtension(inputPath) + "_with_elo.csv");

            try
            {
                ProcessDataset(inputPath, outputPath);
                Console.WriteLine($"Done. Wrote {outputPath}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        /// <summary>
        /// Reads the input CSV row-by-row (assumed chronological order),
        /// computes pre-fight Elo ratings, updates them after each fight,
        /// and writes a new CSV with r_elo, b_elo, elo_diff appended.
        /// </summary>
        private static void ProcessDataset(string inputPath, string outputPath)
        {
            var elos = new Dictionary<string, double>();
            var rows = new List<(string[] OriginalFields, double RElo, double BElo, double EloDiff)>();
            string[] headerFields;

            // Read phase: compute Elo for every row
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                BadDataFound = null
            };

            using (var reader = new StreamReader(inputPath))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                csv.Read();
                csv.ReadHeader();
                headerFields = csv.HeaderRecord
                    ?? throw new InvalidOperationException("CSV has no header row.");

                // Find column indices by name
                int rFighterIdx = Array.IndexOf(headerFields, "r_fighter");
                int bFighterIdx = Array.IndexOf(headerFields, "b_fighter");
                int winnerIdx = Array.IndexOf(headerFields, "winner");

                if (rFighterIdx < 0 || bFighterIdx < 0 || winnerIdx < 0)
                {
                    throw new InvalidOperationException(
                        "CSV must contain 'r_fighter', 'b_fighter', and 'winner' columns.");
                }

                while (csv.Read())
                {
                    var fields = new string[headerFields.Length];
                    for (int i = 0; i < headerFields.Length; i++)
                    {
                        fields[i] = csv.GetField(i) ?? string.Empty;
                    }

                    var rFighter = fields[rFighterIdx].Trim();
                    var bFighter = fields[bFighterIdx].Trim();
                    var winner = fields[winnerIdx].Trim();

                    // Get pre-fight Elo (before updating)
                    double rElo = GetElo(elos, rFighter);
                    double bElo = GetElo(elos, bFighter);
                    double eloDiff = rElo - bElo;

                    rows.Add((fields, rElo, bElo, eloDiff));

                    // Update Elo based on fight result
                    UpdateElo(elos, rFighter, bFighter, winner);
                }
            }

            // Write phase: original columns + 3 new Elo columns
            using (var writer = new StreamWriter(outputPath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                // Write header
                foreach (var field in headerFields)
                {
                    csv.WriteField(field);
                }
                csv.WriteField("r_elo");
                csv.WriteField("b_elo");
                csv.WriteField("elo_diff");
                csv.NextRecord();

                // Write data rows
                foreach (var (fields, rElo, bElo, eloDiff) in rows)
                {
                    foreach (var field in fields)
                    {
                        csv.WriteField(field);
                    }
                    csv.WriteField(Math.Round(rElo, 2));
                    csv.WriteField(Math.Round(bElo, 2));
                    csv.WriteField(Math.Round(eloDiff, 2));
                    csv.NextRecord();
                }
            }

            Console.WriteLine($"Processed {rows.Count} fights.");
            Console.WriteLine($"Tracked {elos.Count} unique fighters.");
        }

        private static double GetElo(Dictionary<string, double> elos, string fighter)
        {
            if (!elos.TryGetValue(fighter, out double elo))
            {
                elo = DefaultElo;
                elos[fighter] = elo;
            }
            return elo;
        }

        /// <summary>
        /// Updates Elo ratings for both fighters after a fight.
        /// Uses standard Elo formula: E = 1 / (1 + 10^((Rb - Ra) / 400))
        /// Winner gets score 1.0, loser gets 0.0, draw gets 0.5 each.
        /// </summary>
        private static void UpdateElo(
            Dictionary<string, double> elos,
            string rFighter,
            string bFighter,
            string winner)
        {
            double rElo = elos[rFighter];
            double bElo = elos[bFighter];

            double expectedR = 1.0 / (1.0 + Math.Pow(10.0, (bElo - rElo) / 400.0));
            double expectedB = 1.0 - expectedR;

            // Determine actual scores based on winner
            double actualR;
            double actualB;
            if (winner.Equals("Red", StringComparison.OrdinalIgnoreCase))
            {
                actualR = 1.0;
                actualB = 0.0;
            }
            else if (winner.Equals("Blue", StringComparison.OrdinalIgnoreCase))
            {
                actualR = 0.0;
                actualB = 1.0;
            }
            else
            {
                // Draw or unknown -- treat as draw
                actualR = 0.5;
                actualB = 0.5;
            }

            elos[rFighter] = rElo + KFactor * (actualR - expectedR);
            elos[bFighter] = bElo + KFactor * (actualB - expectedB);
        }
    }
}
