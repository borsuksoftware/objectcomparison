using System;
using System.Collections.Generic;
using System.Linq;

namespace BorsukSoftware.Testing.Comparison.Extensions.Collections
{
    /// <summary>
    /// Class to compare sets of objects based off using the object flattener framework and an arbitrary key function
    /// </summary>
    /// <remarks>
    /// <para>Comparisons are performed by flattening each object using the initially supplied <see cref="BorsukSoftware.ObjectFlattener.IObjectFlattener"/>
    /// and <see cref="BorsukSoftware.Testing.Comparison.IObjectComparer"/> instances. If a more complicated approach is needed, then look at
    /// <see cref="ObjectSetComparerAdvanced"/></para>
    /// 
    /// <para>This code works on the assumption that the ordering isn't important, if this is not the case, then the key generation func
    /// does get given the index within the collection of the object which it's operating on. This index can be passed into the resulting
    /// key dictionary to get position sensitive results.</para></remarks>
    public class ObjectSetComparerStandard
    {
        #region Data Model

        /// <summary>
        /// Get the underlying object flattener which will act upon the objects being compared
        /// </summary>
        public BorsukSoftware.ObjectFlattener.IObjectFlattener ObjectFlattener { get; }

        /// <summary>
        /// Get the underlying object comparer which will perform the actual object by object comparison
        /// </summary>
        public IObjectComparer UnderlyingComparer { get; }

        #endregion

        #region Construction Logic

        /// <summary>
        /// Construct a fresh instance
        /// </summary>
        /// <param name="objectFlattener">The underlying object flattener to use to convert objects to a comparable state</param>
        /// <param name="underlyingComparer">The underlying object comparer to use to compare the objects</param>
        public ObjectSetComparerStandard(
            BorsukSoftware.ObjectFlattener.IObjectFlattener objectFlattener,
            IObjectComparer underlyingComparer)
        {
            if (objectFlattener == null)
                throw new ArgumentNullException(nameof(objectFlattener));

            if (underlyingComparer == null)
                throw new ArgumentNullException(nameof(underlyingComparer));

            this.ObjectFlattener = objectFlattener;
            this.UnderlyingComparer = underlyingComparer;
        }

        #endregion

        /// <summary>
        /// Compare the given sets of objects
        /// </summary>
        /// <remarks><see cref="IEnumerable{T}"/> is used as the input type here to simplify the job of the callers</remarks>
        /// <typeparam name="T">The type of the object to be compared (may be object)</typeparam>
        /// <param name="keyGenerationFunc">The function to provide the keys to use for a given object</param>
        /// <param name="expectedObjects">The set of expected objects (iterated through once)</param>
        /// <param name="actualObjects">The set of actual objects (iterated through once)</param>
        /// <returns>The results of comparing the object sets</returns>
        public ComparisonResults<T> CompareObjectSets<T>(
            Func<int, T, IReadOnlyDictionary<string, object>> keyGenerationFunc,
            IEnumerable<T> expectedObjects,
            IEnumerable<T> actualObjects)
        {
            var allObjectsByKey = new Dictionary<IReadOnlyDictionary<string, object>, ValuesTuple<T>>(new DictionaryEqualityComparer());

            int idx = 0;
            foreach (T obj in expectedObjects)
            {
                var keyDict = keyGenerationFunc(idx++, obj);
                if (!allObjectsByKey.TryGetValue(keyDict, out var existingTuple))
                {
                    existingTuple = new ValuesTuple<T>();
                    allObjectsByKey[keyDict] = existingTuple;
                }

                existingTuple.ExpectedValues.Add(obj);
            }

            idx = 0;
            foreach (T obj in actualObjects)
            {
                var keyDict = keyGenerationFunc(idx++, obj);
                if (!allObjectsByKey.TryGetValue(keyDict, out var existingTuple))
                {
                    existingTuple = new ValuesTuple<T>();
                    allObjectsByKey[keyDict] = existingTuple;
                }

                existingTuple.ActualValues.Add(obj);
            }

            var matching = new Dictionary<IReadOnlyDictionary<string, object>, (T Expected, T Actual)>();
            var differences = new Dictionary<IReadOnlyDictionary<string, object>, (T Expected, T Actual, IReadOnlyCollection<KeyValuePair<string, ComparisonResults>> Differences)>();
            var missingKeys = new Dictionary<IReadOnlyDictionary<string, object>, T>();
            var additionalKeys = new Dictionary<IReadOnlyDictionary<string, object>, T>();
            var incomparable = new Dictionary<IReadOnlyDictionary<string, object>, (IReadOnlyCollection<T> expectedObjects, IReadOnlyCollection<T> actualObjects)>();

            foreach (var pair in allObjectsByKey)
            {
                if (pair.Value.ExpectedValues.Count > 1 || pair.Value.ActualValues.Count > 1)
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
                    var expectedValue = pair.Value.ExpectedValues[0];
                    var actualValue = pair.Value.ActualValues[0];

                    var expectedObjectFlattenedValues = this.ObjectFlattener.FlattenObject(null, expectedValue);
                    var actualObjectFlattenedValues = this.ObjectFlattener.FlattenObject(null, actualValue);

                    var objectLevelDifferences = this.UnderlyingComparer.CompareValues(expectedObjectFlattenedValues, actualObjectFlattenedValues).ToList();
                    if (objectLevelDifferences.Count == 0)
                    {
                        matching.Add(pair.Key, (Expected: expectedValue, Actual: actualValue));
                        continue;
                    }

                    differences.Add(pair.Key, (Expected: expectedValue, Actual: actualValue, Differences: objectLevelDifferences));
                    continue;
                }
            }

            return new ComparisonResults<T>
            {
                MatchingObjects = matching,
                AdditionalKeys = additionalKeys,
                MissingKeys = missingKeys,
                Differences = differences,
                IncomparableKeys = incomparable
            };
        }

        public class ComparisonResults<T>
        {
            public IReadOnlyDictionary<IReadOnlyDictionary<string, object>, (T Expected, T Actual)> MatchingObjects { get; set; }
            public IReadOnlyDictionary<IReadOnlyDictionary<string, object>, T> MissingKeys { get; set; }
            public IReadOnlyDictionary<IReadOnlyDictionary<string, object>, T> AdditionalKeys { get; set; }
            public IReadOnlyDictionary<IReadOnlyDictionary<string, object>, (T Expected, T Actual, IReadOnlyCollection<KeyValuePair<string, ComparisonResults>> Differences)> Differences { get; set; }
            public IReadOnlyDictionary<IReadOnlyDictionary<string, object>, (IReadOnlyCollection<T> ExpectedObjects, IReadOnlyCollection<T> ActualObjects)> IncomparableKeys { get; set; }
        }

        private class ValuesTuple<T>
        {
            public List<T> ExpectedValues = new List<T>();
            public List<T> ActualValues = new List<T>();
        }
    }
}
