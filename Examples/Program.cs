using System;
using System.Collections.Generic;
using System.Linq;

namespace Examples
{
	/// <summary>
	/// Examples program to demonstrate possible usages for the comparison library
	/// </summary>
	/// <remarks>This uses the BorsukSoftware.ObjectFlattener.Core library to convert the objects from the
	/// .net types into a format suitable for comparison</remarks>
	class Program
	{
		enum Modes
		{
			Simple,
			Finance
		};

		public static void Main(string[] args)
		{
			Modes? mode = null;
			if (args.Length >= 2 && args[0].ToLower() == "--mode")
			{
				if (!Enum.TryParse<Modes>(args[1], true, out var modeI))
				{
					var defaultColour = Console.ForegroundColor;
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"Invalid mode - {args[1]}");
					Console.ForegroundColor = defaultColour;
					return;
				}

				mode = modeI;
			}

			if (!mode.HasValue)
			{
				Console.WriteLine(@"Object Comparison Examples
Select mode:

  1 - Simple
  2 - Nested finance example
  Esc - Cancel
");

				while (!mode.HasValue)
				{
					var readChar = System.Console.ReadKey(true);

					switch (readChar.KeyChar)
					{
						case (char)27:
							return;

						case '1':
							mode = Modes.Simple;
							break;

						case '2':
							mode = Modes.Finance;
							break;
					}
				}
			}

			switch (mode.Value)
			{
				case Modes.Finance:
					FinanceMode();
					break;

				case Modes.Simple:
					SimpleMode();
					break;
			}
		}

		private static void SimpleMode()
		{
			Console.WriteLine("Simple mode");
			var @object = new
			{
				property1 = 2.3,
				property2 = 2.3f,
				property3 = "str",
				property4 = 1,
				property5 = 1L,
				property6 = (short)1,
				nestedProp = new
				{
					nes1 = 2.3d,
					nes2 = 5.6,
					sd = "AOF"
				}
			};

			var @object2 = new
			{
				property1 = 2.31,
				property2 = 2.33f,
				property3 = "str",
				property4 = 1,
				property5 = 1L,
				property6 = (short)1,
				nestedProp = new
				{
					nes1 = 2.3f,
					nes2 = 5.6,
					sd = "aof"
				}
			};

			var extractor = new BorsukSoftware.ObjectFlattener.ObjectFlattener();
			extractor.Plugins.Add(new BorsukSoftware.ObjectFlattener.Plugins.StandardPlugin()
			{
				ProcessFields = true,
				ProcessProperties = true
			});

			var comparer = new BorsukSoftware.Testing.Comparison.ObjectComparer
			{
				ObjectComparerNoAvailablePluginBehaviour = BorsukSoftware.Testing.Comparison.ObjectComparerNoAvailablePluginBehaviour.ReportAsDifference
			};

			comparer.ComparisonPlugins.Add(new BorsukSoftware.Testing.Comparison.Plugins.DoubleComparerPlugin());
			comparer.ComparisonPlugins.Add(new BorsukSoftware.Testing.Comparison.Plugins.FloatComparerPlugin());
			comparer.ComparisonPlugins.Add(new BorsukSoftware.Testing.Comparison.Plugins.Int16ComparerPlugin());
			comparer.ComparisonPlugins.Add(new BorsukSoftware.Testing.Comparison.Plugins.Int32ComparerPlugin());
			comparer.ComparisonPlugins.Add(new BorsukSoftware.Testing.Comparison.Plugins.Int64ComparerPlugin());
			comparer.ComparisonPlugins.Add(new BorsukSoftware.Testing.Comparison.Plugins.SimpleStringComparerPlugin() { IgnoreCase = true });

			var output = comparer.CompareValues(extractor.FlattenObject(null, @object), extractor.FlattenObject(null, object2));

			foreach (var pair in output)
			{
				Console.WriteLine($" {pair.Key} - {pair.Value.ExpectedValue} ({pair.Value.ExpectedValue?.GetType()?.Name}) vs. {pair.Value.ActualValue} ({pair.Value.ActualValue?.GetType()?.Name})=> {pair.Value.ComparisonPayload}");
			}
		}

		private static void FinanceMode()
		{
			Console.WriteLine("Finance mode");

			// Spoof up some data - typically this would be read in from your actual system / expected set of results etc.
			DataStucture[] data = new DataStucture[2];
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = new DataStucture();

				data[i].InstrumentLevelResults = new InstrumentResults[200];
				for (int j = 0; j < data[i].InstrumentLevelResults.Length; ++j)
				{
					data[i].InstrumentLevelResults[j] = new InstrumentResults()
					{
						BaseCurrency = "GBP",
						Identifier = $"Trade #{j}",
						LocalValue = 200 + (20 * j),
						BaseCurrencyValue = 200 + (20 * j)
					};
				}
			}

			var r = new Random();
			var itemsToUpdate = r.Next(20);
			for (int i = 0; i < itemsToUpdate; ++i)
			{
				var idx = r.Next(data[1].InstrumentLevelResults.Length);
				data[1].InstrumentLevelResults[idx].LocalValue += 2 * r.NextDouble();

				// Randomly adjust a second value for demonstration purposes
				if (r.NextDouble() < 0.2)
				{
					data[1].InstrumentLevelResults[idx].BaseCurrencyValue += 2 * r.NextDouble();
				}
			}

			var objectFlattener = new BorsukSoftware.ObjectFlattener.ObjectFlattener();
			objectFlattener.Plugins.Add(new BorsukSoftware.ObjectFlattener.Plugins.StandardPlugin());

			var objectComparer = new BorsukSoftware.Testing.Comparison.ObjectComparer();
			objectComparer.ComparisonPlugins.Add(new BorsukSoftware.Testing.Comparison.Plugins.SimpleStringComparerPlugin());
			objectComparer.ComparisonPlugins.Add(new BorsukSoftware.Testing.Comparison.Plugins.DoubleComparerPlugin());

			// Perform the actual comparison...
			var comparisons = Enumerable.Range(0, data[0].InstrumentLevelResults.Length).
				Select(i => new { i, expected = data[0].InstrumentLevelResults[i], actual = data[1].InstrumentLevelResults[i] }).
				Select(tuple => new
				{
					tuple.actual,
					tuple.expected,
					comparisons = objectComparer.CompareValues(
					   objectFlattener.FlattenObject(null, tuple.expected),
					   objectFlattener.FlattenObject(null, tuple.actual)).ToList()
				}).
				ToList();

			// Create a friendly output format for display
			var matching = comparisons.Where(t => t.comparisons.Count == 0).ToList();
			var differences = comparisons.Where(t => t.comparisons.Count != 0).ToList();

			var output = new
			{
				summary = new
				{
					matching = matching.Count,
					differences = differences.Count
				},
				differences = differences.Select(t => new
				{
					identifier = t.actual.Identifier,
					differences = t.comparisons.Select(t2 => new
					{
						key = t2.Key,
						expected = t2.Value.ExpectedValue,
						actual = t2.Value.ActualValue,
						difference = t2.Value.ComparisonPayload
					})
				}),
			};

			var text = System.Text.Json.JsonSerializer.Serialize(output, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
			Console.WriteLine(text);
		}
	}
}
