using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using Moq;
using Xunit;

namespace BorsukSoftware.Testing.Comparison
{
    public class ObjectComparerTests
    {
        #region Input Validation

        [Fact]
        public void NullActualValues()
        {
            var comparer = new ObjectComparer();
            Assert.Throws<ArgumentNullException>(() => comparer.CompareValues(Array.Empty<KeyValuePair<string, object>>(), null));
        }

        [Fact]
        public void NullExpectedValues()
        {
            var comparer = new ObjectComparer();
            Assert.Throws<ArgumentNullException>(() => comparer.CompareValues(null, Array.Empty<KeyValuePair<string, object>>()));
        }

        #endregion

        #region Duplicate testing

        [Fact]
        public void DuplicateActualValues()
        {
            var comparer = new ObjectComparer();

            var actualValues = new[]
            {
                new KeyValuePair<string,object>( "key1", 2),
                new KeyValuePair<string, object>( "key1", 3 )
            };

            var expectedValues = Array.Empty<KeyValuePair<string, object>>();

            Assert.Throws<InvalidOperationException>(() => comparer.CompareValues(expectedValues, actualValues));
        }

        [Fact]
        public void DuplicateExpectedValues()
        {
            var comparer = new ObjectComparer();

            var expectedValues = new[]
            {
                new KeyValuePair<string,object>( "key1", 2),
                new KeyValuePair<string, object>( "key1", 3 )
            };

            var actualValues = Array.Empty<KeyValuePair<string, object>>();

            Assert.Throws<InvalidOperationException>(() => comparer.CompareValues(expectedValues, actualValues));
        }

        #endregion

        [Fact]
        public void NullValuesMatch()
        {
            var comparer = new ObjectComparer()
            {
                ObjectComparerNoAvailablePluginBehaviour = ObjectComparerNoAvailablePluginBehaviour.Throw
            };

            var response = comparer.CompareValues(
                new[] { new KeyValuePair<string, object>("key", null) },
                new[] { new KeyValuePair<string, object>("key", null) });

            Assert.NotNull(response);
            Assert.Empty(response);
        }

        #region No plugin behaviour tests

        [Fact]
        public void NoPlugin_Throw()
        {
            var comparer = new ObjectComparer()
            {
                ObjectComparerNoAvailablePluginBehaviour = ObjectComparerNoAvailablePluginBehaviour.Throw
            };

            Assert.Throws<InvalidOperationException>(() => comparer.CompareValues(
                new[] { new KeyValuePair<string, object>("key", 1) },
                new[] { new KeyValuePair<string, object>("key", 1) }).
                ToList());
        }

        [Fact]
        public void NoPlugin_ReportAsDifference()
        {
            var comparer = new ObjectComparer()
            {
                ObjectComparerNoAvailablePluginBehaviour = ObjectComparerNoAvailablePluginBehaviour.ReportAsDifference
            };

            var output = comparer.CompareValues(
                new[] { new KeyValuePair<string, object>("key", 2.3d) },
                new[] { new KeyValuePair<string, object>("key", 2.4f) });

            Assert.NotNull(output);
            var outputAsList = output.ToList();
            Assert.Single(outputAsList);

            var first = output.First();

            Assert.Equal("key", first.Key);
            Assert.Equal(2.3d, first.Value.ExpectedValue);
            Assert.Equal(2.4f, first.Value.ActualValue);
            Assert.Null(first.Value.ComparisonPayload);
        }

        [Fact]
        public void NoPlugin_Ignore()
        {
            var comparer = new ObjectComparer()
            {
                ObjectComparerNoAvailablePluginBehaviour = ObjectComparerNoAvailablePluginBehaviour.Ignore
            };

            var output = comparer.CompareValues(
                new[] { new KeyValuePair<string, object>("key", 2.3d) },
                new[] { new KeyValuePair<string, object>("key", 2.4f) });

            Assert.NotNull(output);
            var outputAsList = output.ToList();
            Assert.Empty(outputAsList);
        }
        #endregion

        #region Mismatched Key behaviour


        public static IEnumerable<object[]> MismatchedKeyBehaviour_Data
        {
            get
            {
                yield return new object[]
                {
                    new [] { new KeyValuePair<string,object>( "val1", 2) },
                    new [] { new KeyValuePair<string,object>( "val2", 3) },
                };

                yield return new object[]
                {
                    null,
                    new [] { new KeyValuePair<string,object>( "val2", 3) },
                };

                yield return new object[]
                {
                    new [] { new KeyValuePair<string,object>( "val1", 2) },
                    null,
                };
            }
        }

        [MemberData(nameof(MismatchedKeyBehaviour_Data))]
        [Theory]
        public void MismatchedKeyBehaviourIgnore(IEnumerable<KeyValuePair<string, object>> additionalExpectedValues, IEnumerable<KeyValuePair<string, object>> additionActualValues)
        {
            var comparer = new ObjectComparer
            {
                ObjectComparerMismatchedKeysBehaviour = ObjectComparerMismatchedKeysBehaviour.Ignore,
                ObjectComparerNoAvailablePluginBehaviour = ObjectComparerNoAvailablePluginBehaviour.Throw
            };

            var matchingValues = Enumerable.Range(1, 20).Select(i => new KeyValuePair<string, object>(i.ToString(), i));

            var expectedValues = matchingValues.Concat(additionalExpectedValues ?? Enumerable.Empty<KeyValuePair<string, object>>());
            var actualValues = matchingValues.Concat(additionActualValues ?? Enumerable.Empty<KeyValuePair<string, object>>());

            var mockPlugin = new Mock<IObjectComparerPlugin>(MockBehavior.Strict);
            foreach (var matchingValue in matchingValues)
                mockPlugin.Setup(x => x.TryCompare(matchingValue.Key, It.IsAny<object>(), It.IsAny<object>())).
                    Returns(new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal }).
                    Verifiable();

            comparer.ComparisonPlugins.Add(mockPlugin.Object);

            var results = comparer.CompareValues(expectedValues, actualValues);

            Assert.NotNull(results);
            var resultsAsList = results.ToList();
            Assert.Empty(resultsAsList);

            mockPlugin.Verify();
        }

        [MemberData(nameof(MismatchedKeyBehaviour_Data))]
        [Theory]
        public void MismatchedKeyBehaviourReport(IEnumerable<KeyValuePair<string, object>> additionalExpectedValues, IEnumerable<KeyValuePair<string, object>> additionalActualValues)
        {
            var comparer = new ObjectComparer
            {
                ObjectComparerMismatchedKeysBehaviour = ObjectComparerMismatchedKeysBehaviour.ReportAsDifference,
                ObjectComparerNoAvailablePluginBehaviour = ObjectComparerNoAvailablePluginBehaviour.Throw
            };

            var matchingValues = Enumerable.Range(1, 20).Select(i => new KeyValuePair<string, object>(i.ToString(), i));

            var expectedValues = matchingValues.Concat(additionalExpectedValues ?? Enumerable.Empty<KeyValuePair<string, object>>());
            var actualValues = matchingValues.Concat(additionalActualValues ?? Enumerable.Empty<KeyValuePair<string, object>>());

            var mockPlugin = new Mock<IObjectComparerPlugin>(MockBehavior.Strict);
            foreach (var matchingValue in matchingValues)
                mockPlugin.Setup(x => x.TryCompare(matchingValue.Key, It.IsAny<object>(), It.IsAny<object>())).
                    Returns(new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal });

            comparer.ComparisonPlugins.Add(mockPlugin.Object);

            var results = comparer.CompareValues(expectedValues, actualValues);

            Assert.NotNull(results);
            var resultsAsList = results.ToList();
            Assert.NotEmpty(resultsAsList);

            IEnumerable<KeyValuePair<string, ComparisonResults>> expectedDifferences = Enumerable.Empty<KeyValuePair<string, ComparisonResults>>();
            if (additionalExpectedValues != null)
                expectedDifferences = expectedDifferences.Concat(additionalExpectedValues.Select(pair => new KeyValuePair<string, ComparisonResults>(pair.Key, new ComparisonResults { ExpectedValue = pair.Value })));

            if (additionalActualValues != null)
                expectedDifferences = expectedDifferences.Concat(additionalActualValues.Select(pair => new KeyValuePair<string, ComparisonResults>(pair.Key, new ComparisonResults { ActualValue = pair.Value })));

            expectedDifferences.OrderBy(pair => pair.Key).Should().
                BeEquivalentTo(resultsAsList.OrderBy(pair => pair.Key));
        }

        [MemberData(nameof(MismatchedKeyBehaviour_Data))]
        [Theory]
        public void MismatchedKeyBehaviourThrow(IEnumerable<KeyValuePair<string, object>> additionalExpectedValues, IEnumerable<KeyValuePair<string, object>> additionalActualValues)
        {
            var comparer = new ObjectComparer
            {
                ObjectComparerMismatchedKeysBehaviour = ObjectComparerMismatchedKeysBehaviour.Throw,
                ObjectComparerNoAvailablePluginBehaviour = ObjectComparerNoAvailablePluginBehaviour.Throw
            };

            var matchingValues = Enumerable.Range(1, 20).Select(i => new KeyValuePair<string, object>(i.ToString(), i));

            var expectedValues = matchingValues.Concat(additionalExpectedValues ?? Enumerable.Empty<KeyValuePair<string, object>>());
            var actualValues = matchingValues.Concat(additionalActualValues ?? Enumerable.Empty<KeyValuePair<string, object>>());

            var mockPlugin = new Mock<IObjectComparerPlugin>(MockBehavior.Strict);
            foreach (var matchingValue in matchingValues)
                mockPlugin.Setup(x => x.TryCompare(matchingValue.Key, It.IsAny<object>(), It.IsAny<object>())).
                    Returns(new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal });

            comparer.ComparisonPlugins.Add(mockPlugin.Object);

            Assert.Throws<NotSupportedException>(() => { comparer.CompareValues(expectedValues, actualValues); });
        }

        [MemberData(nameof(MismatchedKeyBehaviour_Data))]
        [Theory]
        public void MismatchedKeyBehaviourTreatMissingValueAsNull(IEnumerable<KeyValuePair<string, object>> additionalExpectedValues, IEnumerable<KeyValuePair<string, object>> additionalActualValues)
        {
            var comparer = new ObjectComparer
            {
                ObjectComparerMismatchedKeysBehaviour = ObjectComparerMismatchedKeysBehaviour.TreatMissingValueAsNull,
                ObjectComparerNoAvailablePluginBehaviour = ObjectComparerNoAvailablePluginBehaviour.Throw
            };

            var matchingValues = Enumerable.Range(1, 20).Select(i => new KeyValuePair<string, object>(i.ToString(), i));

            var expectedValues = matchingValues.Concat(additionalExpectedValues ?? Enumerable.Empty<KeyValuePair<string, object>>());
            var actualValues = matchingValues.Concat(additionalActualValues ?? Enumerable.Empty<KeyValuePair<string, object>>());

            var mockPlugin = new Mock<IObjectComparerPlugin>(MockBehavior.Strict);
            foreach (var matchingValue in matchingValues)
                mockPlugin.Setup(x => x.TryCompare(matchingValue.Key, It.IsAny<object>(), It.IsAny<object>())).
                    Returns(new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal });

            if (additionalExpectedValues != null)
                foreach (var pair in additionalExpectedValues)
                    mockPlugin.
                        Setup(x => x.TryCompare(pair.Key, pair.Value, null)).
                        Returns(new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Different }).
                        Verifiable();

            if (additionalActualValues != null)
                foreach (var pair in additionalActualValues)
                    mockPlugin.
                        Setup(x => x.TryCompare(pair.Key, null, pair.Value)).
                        Returns(new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Different }).
                        Verifiable();

            comparer.ComparisonPlugins.Add(mockPlugin.Object);

            var results = comparer.CompareValues(expectedValues, actualValues);
            Assert.NotNull(results);
            var resultsAsList = results.ToList();
            Assert.NotEmpty(results);

            IEnumerable<KeyValuePair<string, ComparisonResults>> expectedDifferences = Enumerable.Empty<KeyValuePair<string, ComparisonResults>>();
            if (additionalExpectedValues != null)
                expectedDifferences = expectedDifferences.Concat(additionalExpectedValues.Select(pair => new KeyValuePair<string, ComparisonResults>(pair.Key, new ComparisonResults { ExpectedValue = pair.Value })));

            if (additionalActualValues != null)
                expectedDifferences = expectedDifferences.Concat(additionalActualValues.Select(pair => new KeyValuePair<string, ComparisonResults>(pair.Key, new ComparisonResults { ActualValue = pair.Value })));

            expectedDifferences.OrderBy(pair => pair.Key).Should().
                BeEquivalentTo(resultsAsList.OrderBy(pair => pair.Key));

            mockPlugin.Verify();
        }

        #endregion
    }
}
