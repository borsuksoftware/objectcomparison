using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using Xunit;

namespace BorsukSoftware.Testing.Comparison.Extensions.Collections
{
    public class ObjectSetComparerStandardTests
    {
        [Fact]
        public void Standard()
        {
            var underlyingComparer = new ObjectComparer { ObjectComparerMismatchedKeysBehaviour = ObjectComparerMismatchedKeysBehaviour.Throw, ObjectComparerNoAvailablePluginBehaviour = ObjectComparerNoAvailablePluginBehaviour.Throw };
            // To exclude KeyPart1 / 2 from being compared, they can either be marked as always passing OR they can be excluded from the object flattening process. Usually easier to do so in the comparer
            // Note that this isn't really necessary as if the keys match, then these values will be equal and therefore should pass (unless there's no comparer plugin specified for them of course)
            underlyingComparer.ComparisonPlugins.Add(new Plugins.FunctionBasedComparerPlugin((name, obj1, obj2) =>
                {
                    if (name.EndsWith(nameof(ComparisonObjectType.KeyPart1)) || name.EndsWith(nameof(ComparisonObjectType.KeyPart2)))
                        return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

                    return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };
                }));

            // If it was desired to exclude a particular value from the comparison, say a new field has been added and comparisons wish to be done excluding that, then create
            // a custom plugin for the comparer to mark those values as having matched
            // underlyingComparer.ComparisonPlugins.Add(new Plugins.FunctionBasedComparerPlugin((name, obj1, obj2) =>
            // {
            //     if (name.EndsWith(nameof(ComparisonObjectType.RandomValue)))
            //         return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };
            // 
            //     return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };
            // }));
            // The alternative is to exclude the object from flattening...


            underlyingComparer.ComparisonPlugins.Add(new Plugins.SimpleStringComparerPlugin());
            underlyingComparer.ComparisonPlugins.Add(new Plugins.DoubleComparerPlugin());
            underlyingComparer.ComparisonPlugins.Add(new Plugins.Int32ComparerPlugin());

            var objectFlattener = new BorsukSoftware.ObjectFlattener.ObjectFlattener { NoAvailablePluginBehaviour = ObjectFlattener.NoAvailablePluginBehaviour.Throw };

            // Uncomment this code to not report the value for RandomValue in the output (note, must be at the beginning of the list to avoid the standard plugin handling it)
            // objectFlattener.Plugins.Add(new BorsukSoftware.ObjectFlattener.Plugins.FunctionBasedPlugin((name, obj) => name.EndsWith(nameof(ComparisonObjectType.RandomValue)), (of, name, obj) => Enumerable.Empty<KeyValuePair<string, object>>());

            objectFlattener.Plugins.Add(new BorsukSoftware.ObjectFlattener.Plugins.StandardPlugin());

            var items1 = Enumerable.Range(0, 200).
                Select(i => new ComparisonObjectType { KeyPart1 = i.ToString(), Value1 = 2.3 * i, Value2 = 2.4 * i, Value3 = 2.5 * i }).
                ToList();

            var items2 = Enumerable.Range(0, 200).
                Select(i => new ComparisonObjectType { KeyPart1 = i.ToString(), Value1 = 2.3 * i, Value2 = 2.4 * i, Value3 = 2.5 * i }).
                ToList();

            items2[23].Value3 += 6.7;
            items2[27].KeyPart1 = "28";

            var comparer = new ObjectSetComparerStandard(objectFlattener, underlyingComparer);
            var comparedSets = comparer.CompareObjectSets((idx, obj) =>
                {
                    var dict = new Dictionary<string, object>();
                    dict["KeyPart1"] = obj.KeyPart1;
                    dict["KeyPart2"] = obj.KeyPart2;
                    return dict;
                },
                items1,
                items2);

            Assert.Single(comparedSets.Differences);
            var difference = comparedSets.Differences.Single();
            difference.Key.Should().BeEquivalentTo(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("KeyPart1", "23"), new KeyValuePair<string, object>("KeyPart2", "Macro") });

            Assert.Same(items1[23], difference.Value.Expected);
            Assert.Same(items2[23], difference.Value.Actual);
            difference.Value.Differences.Should().
                BeEquivalentTo(new[] {
                    new KeyValuePair<string, ComparisonResults>( "Value3", new ComparisonResults() { ExpectedValue = items1[23].Value3, ActualValue = items2[23].Value3, ComparisonPayload = items2[23].Value3 - items1[23].Value3 })
                });

            Assert.Single(comparedSets.MissingKeys);
            var missingKey = comparedSets.MissingKeys.Single();
            missingKey.Key.Should().BeEquivalentTo(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("KeyPart1", "27"), new KeyValuePair<string, object>("KeyPart2", "Macro") });
            Assert.Same(items1[27], missingKey.Value);

            Assert.Empty(comparedSets.AdditionalKeys);

            Assert.Single(comparedSets.IncomparableKeys);
            var incomparableKey = comparedSets.IncomparableKeys.Single();
            incomparableKey.Key.Should().BeEquivalentTo(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("KeyPart1", "28"), new KeyValuePair<string, object>("KeyPart2", "Macro") });

            Assert.Equal(197, comparedSets.MatchingObjects.Count); // TODO
        }

        [Fact]
        public void AdditionalKey()
        {
            var underlyingComparer = new ObjectComparer { ObjectComparerMismatchedKeysBehaviour = ObjectComparerMismatchedKeysBehaviour.Throw, ObjectComparerNoAvailablePluginBehaviour = ObjectComparerNoAvailablePluginBehaviour.Throw };

            underlyingComparer.ComparisonPlugins.Add(new Plugins.SimpleStringComparerPlugin());
            underlyingComparer.ComparisonPlugins.Add(new Plugins.DoubleComparerPlugin());
            underlyingComparer.ComparisonPlugins.Add(new Plugins.Int32ComparerPlugin());

            var objectFlattener = new BorsukSoftware.ObjectFlattener.ObjectFlattener { NoAvailablePluginBehaviour = ObjectFlattener.NoAvailablePluginBehaviour.Throw };
            objectFlattener.Plugins.Add(new BorsukSoftware.ObjectFlattener.Plugins.StandardPlugin());

            var items1 = Enumerable.Range(0, 200).
                Select(i => new ComparisonObjectType { KeyPart1 = i.ToString(), Value1 = 2.3 * i, Value2 = 2.4 * i, Value3 = 2.5 * i }).
                ToList();

            var items2 = Enumerable.Range(0, 201).
                Select(i => new ComparisonObjectType { KeyPart1 = i.ToString(), Value1 = 2.3 * i, Value2 = 2.4 * i, Value3 = 2.5 * i }).
                ToList();

            var comparer = new ObjectSetComparerStandard(objectFlattener, underlyingComparer);
            var comparedSets = comparer.CompareObjectSets((idx, obj) =>
            {
                var dict = new Dictionary<string, object>();
                dict["KeyPart1"] = obj.KeyPart1;
                dict["KeyPart2"] = obj.KeyPart2;
                return dict;
            },
                items1,
                items2);

            Assert.Empty(comparedSets.Differences);
            Assert.Empty(comparedSets.MissingKeys);
            Assert.Empty(comparedSets.IncomparableKeys);
            Assert.Equal(200, comparedSets.MatchingObjects.Count); // TODO

            Assert.Single(comparedSets.AdditionalKeys);
            var additionalKeys = comparedSets.AdditionalKeys.Single();
            additionalKeys.Key.Should().BeEquivalentTo(new[] { new KeyValuePair<string, object>("KeyPart1", "200"), new KeyValuePair<string, object>("KeyPart2", "Macro") });
            Assert.Same(items2[200], additionalKeys.Value);
        }

        [Fact]
        public void MissingKey()
        {
            var underlyingComparer = new ObjectComparer { ObjectComparerMismatchedKeysBehaviour = ObjectComparerMismatchedKeysBehaviour.Throw, ObjectComparerNoAvailablePluginBehaviour = ObjectComparerNoAvailablePluginBehaviour.Throw };

            underlyingComparer.ComparisonPlugins.Add(new Plugins.SimpleStringComparerPlugin());
            underlyingComparer.ComparisonPlugins.Add(new Plugins.DoubleComparerPlugin());
            underlyingComparer.ComparisonPlugins.Add(new Plugins.Int32ComparerPlugin());

            var objectFlattener = new BorsukSoftware.ObjectFlattener.ObjectFlattener { NoAvailablePluginBehaviour = ObjectFlattener.NoAvailablePluginBehaviour.Throw };
            objectFlattener.Plugins.Add(new BorsukSoftware.ObjectFlattener.Plugins.StandardPlugin());

            var items1 = Enumerable.Range(0, 200).
                Select(i => new ComparisonObjectType { KeyPart1 = i.ToString(), Value1 = 2.3 * i, Value2 = 2.4 * i, Value3 = 2.5 * i }).
                ToList();

            var items2 = Enumerable.Range(0, 199).
                Select(i => new ComparisonObjectType { KeyPart1 = i.ToString(), Value1 = 2.3 * i, Value2 = 2.4 * i, Value3 = 2.5 * i }).
                ToList();

            var comparer = new ObjectSetComparerStandard(objectFlattener, underlyingComparer);
            var comparedSets = comparer.CompareObjectSets((idx, obj) =>
            {
                var dict = new Dictionary<string, object>();
                dict["KeyPart1"] = obj.KeyPart1;
                dict["KeyPart2"] = obj.KeyPart2;
                return dict;
            },
                items1,
                items2);

            Assert.Empty(comparedSets.Differences);
            Assert.Empty(comparedSets.AdditionalKeys);
            Assert.Empty(comparedSets.IncomparableKeys);
            Assert.Equal(199, comparedSets.MatchingObjects.Count); // TODO

            Assert.Single(comparedSets.MissingKeys);
            var missingKeys = comparedSets.MissingKeys.Single();
            missingKeys.Key.Should().BeEquivalentTo(new[] { new KeyValuePair<string, object>("KeyPart1", "199"), new KeyValuePair<string, object>("KeyPart2", "Macro") });
            Assert.Same(items1[199], missingKeys.Value);
        }

        [Fact]
        public void DuplicateKey_Expected()
        {
            var underlyingComparer = new ObjectComparer { ObjectComparerMismatchedKeysBehaviour = ObjectComparerMismatchedKeysBehaviour.Throw, ObjectComparerNoAvailablePluginBehaviour = ObjectComparerNoAvailablePluginBehaviour.Throw };

            underlyingComparer.ComparisonPlugins.Add(new Plugins.SimpleStringComparerPlugin());
            underlyingComparer.ComparisonPlugins.Add(new Plugins.DoubleComparerPlugin());
            underlyingComparer.ComparisonPlugins.Add(new Plugins.Int32ComparerPlugin());

            var objectFlattener = new BorsukSoftware.ObjectFlattener.ObjectFlattener { NoAvailablePluginBehaviour = ObjectFlattener.NoAvailablePluginBehaviour.Throw };
            objectFlattener.Plugins.Add(new BorsukSoftware.ObjectFlattener.Plugins.StandardPlugin());

            var items1 = Enumerable.Range(0, 200).Concat(new[] { 0 }).
                Select(i => new ComparisonObjectType { KeyPart1 = i.ToString(), Value1 = 2.3 * i, Value2 = 2.4 * i, Value3 = 2.5 * i }).
                ToList();

            var items2 = Enumerable.Range(0, 200).
                Select(i => new ComparisonObjectType { KeyPart1 = i.ToString(), Value1 = 2.3 * i, Value2 = 2.4 * i, Value3 = 2.5 * i }).
                ToList();

            var comparer = new ObjectSetComparerStandard(objectFlattener, underlyingComparer);
            var comparedSets = comparer.CompareObjectSets((idx, obj) =>
            {
                var dict = new Dictionary<string, object>();
                dict["KeyPart1"] = obj.KeyPart1;
                dict["KeyPart2"] = obj.KeyPart2;
                return dict;
            },
                items1,
                items2);

            Assert.Empty(comparedSets.Differences);
            Assert.Empty(comparedSets.AdditionalKeys);
            Assert.Empty(comparedSets.MissingKeys);
            Assert.Equal(199, comparedSets.MatchingObjects.Count);

            Assert.Single(comparedSets.IncomparableKeys);
            var incomparableItem = comparedSets.IncomparableKeys.Single();
            incomparableItem.Key.Should().BeEquivalentTo(new[] { new KeyValuePair<string, object>("KeyPart1", "0"), new KeyValuePair<string, object>("KeyPart2", "Macro") });
            incomparableItem.Value.ExpectedObjects.Should().BeEquivalentTo(new[] { items1[0], items1[200] });
            incomparableItem.Value.ActualObjects.Should().BeEquivalentTo(new[] { items2[0] });
            Assert.Equal(2, incomparableItem.Value.ExpectedObjects.Count);
            Assert.Equal(1, incomparableItem.Value.ActualObjects.Count);
        }

        [Fact]
        public void DuplicateKey_Additional()
        {
            var underlyingComparer = new ObjectComparer { ObjectComparerMismatchedKeysBehaviour = ObjectComparerMismatchedKeysBehaviour.Throw, ObjectComparerNoAvailablePluginBehaviour = ObjectComparerNoAvailablePluginBehaviour.Throw };

            underlyingComparer.ComparisonPlugins.Add(new Plugins.SimpleStringComparerPlugin());
            underlyingComparer.ComparisonPlugins.Add(new Plugins.DoubleComparerPlugin());
            underlyingComparer.ComparisonPlugins.Add(new Plugins.Int32ComparerPlugin());

            var objectFlattener = new BorsukSoftware.ObjectFlattener.ObjectFlattener { NoAvailablePluginBehaviour = ObjectFlattener.NoAvailablePluginBehaviour.Throw };
            objectFlattener.Plugins.Add(new BorsukSoftware.ObjectFlattener.Plugins.StandardPlugin());

            var items1 = Enumerable.Range(0, 200).
                Select(i => new ComparisonObjectType { KeyPart1 = i.ToString(), Value1 = 2.3 * i, Value2 = 2.4 * i, Value3 = 2.5 * i }).
                ToList();

            var items2 = Enumerable.Range(0, 200).Concat(new[] { 0 }).
                Select(i => new ComparisonObjectType { KeyPart1 = i.ToString(), Value1 = 2.3 * i, Value2 = 2.4 * i, Value3 = 2.5 * i }).
                ToList();

            var comparer = new ObjectSetComparerStandard(objectFlattener, underlyingComparer);
            var comparedSets = comparer.CompareObjectSets((idx, obj) =>
            {
                var dict = new Dictionary<string, object>();
                dict["KeyPart1"] = obj.KeyPart1;
                dict["KeyPart2"] = obj.KeyPart2;
                return dict;
            },
                items1,
                items2);

            Assert.Empty(comparedSets.Differences);
            Assert.Empty(comparedSets.AdditionalKeys);
            Assert.Empty(comparedSets.MissingKeys);
            Assert.Equal(199, comparedSets.MatchingObjects.Count); // TODO

            Assert.Single(comparedSets.IncomparableKeys);
            var incomparableItem = comparedSets.IncomparableKeys.Single();
            incomparableItem.Key.Should().BeEquivalentTo(new[] { new KeyValuePair<string, object>("KeyPart1", "0"), new KeyValuePair<string, object>("KeyPart2", "Macro") });
            incomparableItem.Value.ExpectedObjects.Should().BeEquivalentTo(new[] { items1[0] });
            incomparableItem.Value.ActualObjects.Should().BeEquivalentTo(new[] { items2[0], items2[200] });
            Assert.Equal(1, incomparableItem.Value.ExpectedObjects.Count);
            Assert.Equal(2, incomparableItem.Value.ActualObjects.Count);
        }


        [Fact]
        public void CanUseAdHocTypes()
        {
            // In this test, we want to ensure that we can compare items which may flatten down to the same item, but aren't actually the same .net object
            var underlyingComparer = new ObjectComparer { ObjectComparerMismatchedKeysBehaviour = ObjectComparerMismatchedKeysBehaviour.TreatMissingValueAsNull, ObjectComparerNoAvailablePluginBehaviour = ObjectComparerNoAvailablePluginBehaviour.Throw };

            underlyingComparer.ComparisonPlugins.Add(new Plugins.FunctionBasedComparerPlugin((name, obj1, obj2) =>
            {
                if (name.EndsWith("cricket"))
                    return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

                return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };
            }));

            underlyingComparer.ComparisonPlugins.Add(new Plugins.SimpleStringComparerPlugin());
            underlyingComparer.ComparisonPlugins.Add(new Plugins.DoubleComparerPlugin());
            underlyingComparer.ComparisonPlugins.Add(new Plugins.Int32ComparerPlugin());

            var objectFlattener = new BorsukSoftware.ObjectFlattener.ObjectFlattener { NoAvailablePluginBehaviour = ObjectFlattener.NoAvailablePluginBehaviour.Throw };
            objectFlattener.Plugins.Add(new BorsukSoftware.ObjectFlattener.Plugins.StandardPlugin());

            var items1 = Enumerable.Range(0, 200).
                Select(i => new { james = i, bob = i.ToString() });

            var items2 = Enumerable.Range(0, 200).
                Select(i => new { bob = i.ToString(), james = i, cricket = 2.3 });

            var comparer = new ObjectSetComparerStandard(objectFlattener, underlyingComparer);
            var comparedSets = comparer.CompareObjectSets<object>((idx, obj) =>
            {
                var dict = new Dictionary<string, object>();
                dict[idx.ToString()] = idx;
                return dict;
            },
                items1,
                items2);

            Assert.Empty(comparedSets.Differences);
            Assert.Empty(comparedSets.AdditionalKeys);
            Assert.Empty(comparedSets.MissingKeys);
            Assert.Equal(200, comparedSets.MatchingObjects.Count);
            Assert.Empty(comparedSets.IncomparableKeys);

            comparedSets.MatchingObjects.OrderBy(t => (int)t.Key.First().Value).
                Select( t => t.Value).
                Should().
                BeEquivalentTo(Enumerable.Zip(items1, items2).Select(tuple => (Expected: tuple.First, Actual: tuple.Second)));
        }

        private class ComparisonObjectType
        {
            public string KeyPart1 { get; set; }
            public string KeyPart2 { get; set; } = "Macro";

            public double Value1 { get; set; }
            public double Value2 { get; set; }
            public double Value3 { get; set; }
        }
    }

}
