using System;
using System.Collections.Generic;
using System.Linq;

namespace BorsukSoftware.Testing.Comparison
{
	/// <summary>
	/// Standard implementation of <see cref="IObjectComparer"/>
	/// </summary>
	public class ObjectComparer : IObjectComparer
	{
		/// <summary>
		/// Get the set of plugins which do the actual comparison.
		/// </summary>
		/// <remarks>The comparer will iterate through this collection and find the first plugin which can handle the given combination. TLDR - order matters</remarks>
		public IList<IObjectComparerPlugin> ComparisonPlugins { get; private set; } = new List<IObjectComparerPlugin>();

		public ObjectComparerNoAvailablePluginBehaviour ObjectComparerNoAvailablePluginBehaviour { get; set; } = ObjectComparerNoAvailablePluginBehaviour.ReportAsDifference;

		public IEnumerable<KeyValuePair<string, ComparisonResults>> CompareValues(IEnumerable<KeyValuePair<string, object>> expectedValues, IEnumerable<KeyValuePair<string, object>> actualValues)
		{
			if (expectedValues == null)
				throw new ArgumentNullException(nameof(expectedValues));

			if (actualValues == null)
				throw new ArgumentNullException(nameof(actualValues));

			var expectedValuesDictionary = new Dictionary<string, object>();
			foreach (var pair in expectedValues)
			{
				if (expectedValuesDictionary.ContainsKey(pair.Key))
					throw new InvalidOperationException($"Duplicate key '{pair.Key}' found in expected values");

				expectedValuesDictionary.Add(pair.Key, pair.Value);
			}

			var actualValuesKeys = new HashSet<string>();
			var actualValuesExtra = new Dictionary<string, object>();
			var differences = new Dictionary<string, ComparisonResults>();
			foreach (var pair in actualValues)
			{
				if (!actualValuesKeys.Add(pair.Key))
					throw new InvalidOperationException($"Duplicate key '{pair.Key}' found in actual values");

				if (!expectedValuesDictionary.TryGetValue(pair.Key, out var expectedValue))
				{
					actualValuesExtra.Add(pair.Key, pair.Value);
					continue;
				}

				expectedValuesDictionary.Remove(pair.Key);

				// Do the actual comparison here...
				if ((pair.Value == null && expectedValue == null) ||
					(pair.Value != null && expectedValue != null && ReferenceEquals(pair.Value, expectedValue)))
					continue;

				// At this point... defer to the plugins
				bool processed = false;
				foreach (var plugin in this.ComparisonPlugins)
				{
					var pluginComparisonResult = plugin.TryCompare(pair.Key, expectedValue, pair.Value);
					switch (pluginComparisonResult.ComparisonResultType)
					{
						case ComparisonResultType.UnableToCompare:
							continue;

						case ComparisonResultType.Equal:
							processed = true;
							break;

						case ComparisonResultType.Different:
							processed = true;
							differences.Add(pair.Key, new ComparisonResults
							{
								ActualValue = pair.Value,
								ComparisonPayload = pluginComparisonResult.Payload,
								ExpectedValue = expectedValue
							});
							break;

						default:
							throw new NotSupportedException($"Unknown comparison result type - '{pluginComparisonResult.ComparisonResultType}'");
					}

					if (processed)
						break;
				}

				if (!processed)
				{
					switch (this.ObjectComparerNoAvailablePluginBehaviour)
					{
						case ObjectComparerNoAvailablePluginBehaviour.Ignore:
							continue;

						case ObjectComparerNoAvailablePluginBehaviour.Throw:
							throw new InvalidOperationException($"Unable to compare values for '{pair.Key}'");

						case ObjectComparerNoAvailablePluginBehaviour.ReportAsDifference:
							differences.Add(pair.Key,
								new ComparisonResults
								{
									ActualValue = pair.Value,
									ExpectedValue = expectedValue
								});
							break;

						default:
							throw new NotSupportedException($"Unsupported missing no available plugin behaviour of '{this.ObjectComparerNoAvailablePluginBehaviour}");
					}
				}
			}

			var output = differences.
				Concat(expectedValuesDictionary.Select(pair => new KeyValuePair<string, ComparisonResults>(pair.Key, new ComparisonResults { ExpectedValue = pair.Value }))).
				Concat(actualValuesExtra.Select(pair => new KeyValuePair<string, ComparisonResults>(pair.Key, new ComparisonResults { ActualValue = pair.Value })));

			return output;
		}
	}
}
