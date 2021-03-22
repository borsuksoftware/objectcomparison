using System;
using System.Collections.Generic;
using System.Linq;

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
	}
}
