using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using Xunit;

namespace BorsukSoftware.Testing.Comparison.Extensions.Collections
{
    public class ObjectSetComparerAdvancedTests
    {
        [Fact]
        public void Standard()
        {
            var objects1 = Enumerable.Range(0, 100).
                Select(i => new ResultsWrapper(i, i, i)).
                Append(new ResultsWrapper(99, 99, 99)).
                Append(new ResultsWrapper(100, 100, 100)).
                ToList();

            var objects2 = Enumerable.Range(0, 100).
                Select(i => new ResultsWrapper(i, i, i)).
                Append(new ResultsWrapper(101, 101, 101)).
                ToList();

            objects1[23] = new ResultsWrapper(23, 23, 24);

            // Expected values...
            // 23 - different
            // 99 - duplicate
            // 100 - missing
            // 101 - additional
            //
            // => 98 entries should match

            var flattener = new ObjectFlattener.ObjectFlattener();
            flattener.NoAvailablePluginBehaviour = ObjectFlattener.NoAvailablePluginBehaviour.Throw;
            flattener.Plugins.Add(new ObjectFlattener.Plugins.DictionaryPlugin<string>(s => s));
            flattener.Plugins.Add(new ObjectFlattener.Plugins.StandardPlugin());

            var objectComparer = new Testing.Comparison.ObjectComparer();
            objectComparer.ObjectComparerNoAvailablePluginBehaviour = ObjectComparerNoAvailablePluginBehaviour.Throw;
            objectComparer.ObjectComparerMismatchedKeysBehaviour = ObjectComparerMismatchedKeysBehaviour.Throw;
            objectComparer.ComparisonPlugins.Add(new Testing.Comparison.Plugins.DoubleComparerPlugin());

            var comparer = new ObjectSetComparerAdvanced();
            var results = comparer.CompareObjectSets((_, r) =>
           {
               var keys = new Dictionary<string, object>();
               keys["idx"] = r.ID;
               return keys;
           },
            (expected, actual) =>
            {
                var valueSet1Results = objectComparer.CompareValues(
                    flattener.FlattenObject(null, expected.ValueSet1),
                    flattener.FlattenObject(null, actual.ValueSet1)).
                    ToList();

                var valueSet2Results = objectComparer.CompareValues(
                    flattener.FlattenObject(null, expected.ValueSet2),
                    flattener.FlattenObject(null, actual.ValueSet2)).
                    ToList();

                var passed = valueSet1Results.Count == 0 && valueSet2Results.Count == 0;
                return (passed, new { valueSet1Differences = valueSet1Results, valueSet2Differences = valueSet2Results });
            },
                objects1,
                objects2);

            Assert.Equal(98, results.MatchingObjects.Count);

            // Compare matching items to verify
            var expectedMatchingItems = Enumerable.Zip(objects1.Take(99), objects2.Take(99)).
                Select((t, idx) =>
                {
                    var keyDict = new Dictionary<string, object>();
                    keyDict["idx"] = idx;

                    var retValue = new KeyValuePair<IReadOnlyDictionary<string, object>, (ResultsWrapper Expected, ResultsWrapper Actual)>(
                        keyDict,
                        (Expected: t.First, Actual: t.Second));

                    return retValue;
                }).Where(t => (int)t.Key["idx"] != 23);

            results.MatchingObjects.OrderBy(t => (int)t.Key["idx"]).
                Should().
                BeEquivalentTo(expectedMatchingItems);

            // Handle the single expected difference at #23
            Assert.Single(results.Differences);
            var singleDifference = results.Differences.Single();
            singleDifference.Key.Should().BeEquivalentTo(new[] { new KeyValuePair<string, object>("idx", 23) });
            Assert.Same(objects1[23], singleDifference.Value.Expected);
            Assert.Same(objects2[23], singleDifference.Value.Actual);
            Assert.Empty(singleDifference.Value.Payload.valueSet1Differences);
            Assert.NotEmpty(singleDifference.Value.Payload.valueSet2Differences);

            // Check the clashing item - 99
            Assert.Single(results.IncomparableKeys);
            var incomparableKVP = results.IncomparableKeys.Single();
            Assert.Single(incomparableKVP.Key);
            Assert.Equal(99, (int)incomparableKVP.Key["idx"]);
            Assert.Equal(2, incomparableKVP.Value.ExpectedObjects.Count);
            Assert.Single(incomparableKVP.Value.ActualObjects);

            // Check the missing item - 100
            Assert.Single(results.MissingKeys);
            var missingKVP = results.MissingKeys.Single();
            Assert.Same(objects1[101], missingKVP.Value); // 101 is intentional... as idx 100 is the duplicate

            // Check the additional item - 101
            Assert.Single(results.AdditionalKeys);
            var additionalKVP = results.AdditionalKeys.Single();
            Assert.Same(objects2[100], additionalKVP.Value); // 100 is intentional... 
        }

        private class ResultsWrapper
        {
            public int ID { get; }

            public IReadOnlyDictionary<string, double> ValueSet1 { get; }
            public IReadOnlyDictionary<string, double> ValueSet2 { get; }

            public ResultsWrapper(int id, int index1, int index2)
            {
                var dict1 = new Dictionary<string, double>();
                var dict2 = new Dictionary<string, double>();

                this.ID = id;
                this.ValueSet1 = dict1;
                this.ValueSet2 = dict2;

                dict1["GBP"] = index1 * 2.0;
                dict1["USD"] = index1 * 2.1;
                dict1["PLN"] = index1 * 2.2;
                dict1["JPY"] = index1 * 2.3;
                dict1["EUR"] = index1 * 2.4;

                dict2["GBP"] = index2 * 4.0;
                dict2["USD"] = index2 * 4.1;
                dict2["PLN"] = index2 * 4.2;
                dict2["JPY"] = index2 * 4.3;
                dict2["EUR"] = index2 * 4.4;
            }
        }
    }
}
