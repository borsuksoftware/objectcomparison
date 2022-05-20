using System;
using System.Collections.Generic;
using System.Linq;

namespace BorsukSoftware.Testing.Comparison.Extensions.Collections
{
    /// <summary>
    /// Class to compare sets of objects based off generating an arbitrary key as a function of the object to be compared
    /// </summary>
    /// <remarks>This code works on the assumption that the ordering isn't important, if this is not the case, then the key generation func
    /// does get given the index within the collection of the object which it's operating on. This index can be passed into the resulting
    /// key dictionary to get position sensitive results</remarks>
    public class ObjectSetComparerAdvanced
    {
        /// <summary>
        /// Compare the given sets of objects using the given criteria
        /// </summary>
        /// <remarks><see cref="IEnumerable{T}"/> is used as the input type here to simplify the job of the callers</remarks>
        /// <typeparam name="TObjectType">The type of the object to be compared (may be object)</typeparam>
        /// <typeparam name="TComparisonResultPayload">The type of the object containing the payload for the results</typeparam>
        /// <param name="keyGenerationFunc">The function to provide the keys to use for a given object</param>
        /// <param name="comparisonFunc">The function to perform the actual comparison</param>
        /// <param name="expectedObjects">The set of expected objects (iterated through once)</param>
        /// <param name="actualObjects">The set of actual objects (iterated through once)</param>
        /// <returns>The results of comparing the object sets</returns>
        public ComparisonResults<TObjectType, TComparisonResultPayload> CompareObjectSets<TObjectType, TComparisonResultPayload>(
            Func<int, TObjectType, IReadOnlyDictionary<string, object>> keyGenerationFunc,
            Func<TObjectType, TObjectType, (bool Passed, TComparisonResultPayload Payload)> comparisonFunc,
            IEnumerable<TObjectType> expectedObjects,
            IEnumerable<TObjectType> actualObjects)
        {
            var allObjectsByKey = new Dictionary<IReadOnlyDictionary<string, object>, ValuesTuple<TObjectType>>(new DictionaryEqualityComparer());

            int idx = 0;
            foreach (TObjectType obj in expectedObjects)
            {
                var keyDict = keyGenerationFunc(idx++, obj);
                if (!allObjectsByKey.TryGetValue(keyDict, out var existingTuple))
                {
                    existingTuple = new ValuesTuple<TObjectType>();
                    allObjectsByKey[keyDict] = existingTuple;
                }

                existingTuple.ExpectedValues.Add(obj);
            }

            idx = 0;
            foreach (TObjectType obj in actualObjects)
            {
                var keyDict = keyGenerationFunc(idx++, obj);
                if (!allObjectsByKey.TryGetValue(keyDict, out var existingTuple))
                {
                    existingTuple = new ValuesTuple<TObjectType>();
                    allObjectsByKey[keyDict] = existingTuple;
                }

                existingTuple.ActualValues.Add(obj);
            }

            var matchingObjects = new Dictionary<IReadOnlyDictionary<string, object>, (TObjectType expected, TObjectType actual)>();
            var differences = new Dictionary<IReadOnlyDictionary<string, object>, (TObjectType expected, TObjectType actual, TComparisonResultPayload payload)>();
            var missingKeys = new Dictionary<IReadOnlyDictionary<string, object>, TObjectType>();
            var additionalKeys = new Dictionary<IReadOnlyDictionary<string, object>,TObjectType>();
            var incomparable = new Dictionary<IReadOnlyDictionary<string, object>, (IReadOnlyCollection<TObjectType> expectedObjects, IReadOnlyCollection<TObjectType> actualObjects)>();

            foreach (var pair in allObjectsByKey)
            {
                if( pair.Value.ActualValues.Count > 1 || pair.Value.ExpectedValues.Count>1)
                {
                    incomparable.Add(pair.Key, (pair.Value.ExpectedValues, pair.Value.ActualValues));
                    continue;
                }

                if (pair.Value.ActualValues.Count > 0 && pair.Value.ExpectedValues.Count == 0)
                {
                    additionalKeys.Add(pair.Key, pair.Value.ActualValues[0]);
                    continue;
                }

                if (pair.Value.ActualValues.Count == 0 && pair.Value.ExpectedValues.Count > 0)
                {
                    missingKeys.Add(pair.Key, pair.Value.ExpectedValues[0]);
                    continue;
                }

                // Handle one-to-one map
                if (pair.Value.ActualValues.Count == 1 && pair.Value.ExpectedValues.Count == 1)
                {
                    var expected = pair.Value.ExpectedValues[0];
                    var actual = pair.Value.ActualValues[0];
                    var resultsTuple = comparisonFunc(expected, actual);

                    if (resultsTuple.Passed)
                    {
                        matchingObjects.Add(pair.Key, (expected, actual));
                    }
                    else
                    {
                        differences.Add(pair.Key, (expected, actual, resultsTuple.Payload));
                    }
                    continue;
                }
            }

            return new ComparisonResults<TObjectType, TComparisonResultPayload>
            {
                MatchingObjects = matchingObjects,
                AdditionalKeys = additionalKeys,
                MissingKeys = missingKeys,
                Differences = differences,
                IncomparableKeys = incomparable
            };
        }

        /// <summary>
        /// Results wrapper class
        /// </summary>
        /// <typeparam name="TObjectType">The type of objects which were compared</typeparam>
        /// <typeparam name="TPayload">The payload type</typeparam>
        public class ComparisonResults<TObjectType, TPayload>
        {
            /// <summary>
            /// Get the set of objects which matched
            /// </summary>
            public IReadOnlyDictionary<IReadOnlyDictionary<string, object>, (TObjectType Expected, TObjectType Actual)> MatchingObjects { get; set; }

            /// <summary>
            /// Get the set of objects which were in the expected set but not in the actual set
            /// </summary>
            public IReadOnlyDictionary<IReadOnlyDictionary<string, object>, TObjectType> MissingKeys { get; set; }

            /// <summary>
            /// Get the set of objects which were in the actual set but not in the expected set
            /// </summary>
            public IReadOnlyDictionary<IReadOnlyDictionary<string, object>, TObjectType> AdditionalKeys { get; set; }

            /// <summary>
            /// Get the set of objects which were different
            /// </summary>
            public IReadOnlyDictionary<IReadOnlyDictionary<string, object>, (TObjectType Expected, TObjectType Actual, TPayload Payload)> Differences { get; set; }

            /// <summary>
            /// Get the set of objects which couldn't be compared because of clashing keys
            /// </summary>
            public IReadOnlyDictionary<IReadOnlyDictionary<string, object>, (IReadOnlyCollection<TObjectType> ExpectedObjects, IReadOnlyCollection<TObjectType> ActualObjects)> IncomparableKeys { get; set; }
        }

        private class ValuesTuple<T>
        {
            public List<T> ExpectedValues = new List<T>();
            public List<T> ActualValues = new List<T>();
        }
    }
}
