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
        #region Data Model

        /// <summary>
        /// Get the set of plugins which do the actual comparison.
        /// </summary>
        /// <remarks>The comparer will iterate through this collection and find the first plugin which can handle the given combination. TLDR - order matters</remarks>
        public IList<IObjectComparerPlugin> ComparisonPlugins { get; private set; } = new List<IObjectComparerPlugin>();

        /// <summary>
        /// Get / set how the object comparer should behave when no suitable plugin can be found for a given comparison
        /// </summary>
        public ObjectComparerNoAvailablePluginBehaviour ObjectComparerNoAvailablePluginBehaviour { get; set; } = ObjectComparerNoAvailablePluginBehaviour.ReportAsDifference;

        /// <summary>
        /// Get / set how the object comparer should behave when it comes across a key which exists in 1 set but not the other
        /// </summary>
        public ObjectComparerMismatchedKeysBehaviour ObjectComparerMismatchedKeysBehaviour { get; set; } = ObjectComparerMismatchedKeysBehaviour.TreatMissingValueAsNull;

        #endregion

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


            var allValuesToProcess = new Dictionary<string, (object expected, object actual)>();
            foreach (var pair in actualValues)
            {
                if (!actualValuesKeys.Add(pair.Key))
                    throw new InvalidOperationException($"Duplicate key '{pair.Key}' found in actual values");

                if (!expectedValuesDictionary.TryGetValue(pair.Key, out var expectedValue))
                {
                    switch (this.ObjectComparerMismatchedKeysBehaviour)
                    {
                        case ObjectComparerMismatchedKeysBehaviour.TreatMissingValueAsNull:
                            expectedValue = null;
                            break;

                        case ObjectComparerMismatchedKeysBehaviour.ReportAsDifference:
                            actualValuesExtra.Add(pair.Key, pair.Value);
                            continue;

                        case ObjectComparerMismatchedKeysBehaviour.Ignore:
                            continue;

                        case ObjectComparerMismatchedKeysBehaviour.Throw:
                            throw new NotSupportedException($"Received value for '{pair.Key}' in actual values, but no such key was found in the set of expected values");

                        default:
                            throw new NotSupportedException($"Invalid mismatched keys behaviour value specified - {this.ObjectComparerMismatchedKeysBehaviour}");
                    }
                }

                expectedValuesDictionary.Remove(pair.Key);

                allValuesToProcess.Add(pair.Key, (expectedValue, pair.Value));
            }

            IEnumerable<KeyValuePair<string, (object expected, object actual)>> valuesToCompare = allValuesToProcess;
            IEnumerable<KeyValuePair<string, ComparisonResults>> additionalDifferencesToReport = actualValuesExtra.Select(pair => new KeyValuePair<string, ComparisonResults>(pair.Key, new ComparisonResults { ActualValue = pair.Value }));

            if (expectedValuesDictionary.Count > 0)
            {
                switch (this.ObjectComparerMismatchedKeysBehaviour)
                {
                    case ObjectComparerMismatchedKeysBehaviour.Ignore:
                        break;

                    case ObjectComparerMismatchedKeysBehaviour.ReportAsDifference:
                        additionalDifferencesToReport = additionalDifferencesToReport.Concat(expectedValuesDictionary.Select(pair => new KeyValuePair<string, ComparisonResults>(pair.Key, new ComparisonResults() { ActualValue = null, ExpectedValue = pair.Value })));
                        break;

                    case ObjectComparerMismatchedKeysBehaviour.TreatMissingValueAsNull:
                        {
                            // Any additional values can just be appended onto the set to be processed
                            valuesToCompare = valuesToCompare.Concat(
                                expectedValuesDictionary.Select(pair => new KeyValuePair<string, (object expected, object actual)>(pair.Key, (expected: pair.Value, actual: null))));
                        }
                        break;

                    case ObjectComparerMismatchedKeysBehaviour.Throw:
                        var firstValue = expectedValuesDictionary.First();
                        throw new NotSupportedException($"Received value for '{firstValue.Key}' in expected values, but no such key was found in th4e set of actual values");

                    default:
                        throw new NotSupportedException($"Invalid mismatched keys behaviour value specified - {this.ObjectComparerMismatchedKeysBehaviour}");
                }
            }

            foreach (var valueToProcess in valuesToCompare)
            {
                // Do the actual comparison here...
                if ((valueToProcess.Value.actual == null && valueToProcess.Value.expected == null) ||
                    (valueToProcess.Value.actual != null && valueToProcess.Value.expected != null && ReferenceEquals(valueToProcess.Value.actual, valueToProcess.Value.expected)))
                    continue;

                // At this point... defer to the plugins
                bool processed = false;
                foreach (var plugin in this.ComparisonPlugins)
                {
                    var pluginComparisonResult = plugin.TryCompare(valueToProcess.Key, valueToProcess.Value.expected, valueToProcess.Value.actual);
                    switch (pluginComparisonResult.ComparisonResultType)
                    {
                        case ComparisonResultType.UnableToCompare:
                            continue;

                        case ComparisonResultType.Equal:
                            processed = true;
                            break;

                        case ComparisonResultType.Different:
                            processed = true;
                            differences.Add(valueToProcess.Key, new ComparisonResults
                            {
                                ActualValue = valueToProcess.Value.actual,
                                ComparisonPayload = pluginComparisonResult.Payload,
                                ExpectedValue = valueToProcess.Value.expected
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
                            throw new InvalidOperationException($"Unable to compare values for '{valueToProcess.Key}'");

                        case ObjectComparerNoAvailablePluginBehaviour.ReportAsDifference:
                            differences.Add(valueToProcess.Key,
                                new ComparisonResults
                                {
                                    ActualValue = valueToProcess.Value.actual,
                                    ExpectedValue = valueToProcess.Value.expected
                                });
                            break;

                        default:
                            throw new NotSupportedException($"Unsupported missing no available plugin behaviour of '{this.ObjectComparerNoAvailablePluginBehaviour}");
                    }
                }
            }

            var output = differences.Concat(additionalDifferencesToReport);

            return output;
        }
    }
}
