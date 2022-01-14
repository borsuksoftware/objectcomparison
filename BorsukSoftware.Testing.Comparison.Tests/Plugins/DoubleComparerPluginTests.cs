using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	public class DoubleComparerPluginTests
	{
		[InlineData(true, 0.1, 0.2, null, 0.2, 0.2)]
		[InlineData(true, 0.1, 0.2, null, 0.1, null)] // Within absolute tolerance
		[InlineData(true, 0.1, 0.2, 100000, 100001, null)] // Within relative tolerance
		[Theory]
		public void TestOutput_BothToleranceModes(bool treatNullsAsZero,
			double absoluteTolerance,
			double relativeTolerance,
			double? expected,
			double? actual,
			double? difference)
		{
			var plugin = new DoubleComparerPlugin
			{
				AbsoluteTolerance = absoluteTolerance,
				RelativeTolerance = relativeTolerance,
				ToleranceModes = FloatingPointToleranceModes.AbsoluteTolerance | FloatingPointToleranceModes.RelativeTolerance,
				TreatMissingValuesAsZero = treatNullsAsZero
			};

			this.PerformTest(plugin, expected, actual, difference);
			this.PerformTest(plugin, actual, expected, difference.HasValue ? (double?)(0 - difference.Value) : null);
		}

		public static IEnumerable<object[]> TestOutput_AbsoluteTolerances_Data
		{
			get
			{
				// Handle null values
				{
					yield return new object[]
					{
						true,
						0.1,
						null,
						0.1,
						null
					};

					yield return new object[]
					{
						true,
						0.1,
						null,
						-0.1,
						null
					};

					yield return new object[]
					{
						true,
						0.1,
						null,
						0.2,
						0.2
					};
				}

				// Handle general values
				{
					yield return new object[]
					{
						true,
						0.1,
						0.2,
						0.1,
						null
					};

					yield return new object[]
					{
						true,
						0.1,
						-.01,
						-0.1,
						null
					};

					yield return new object[]
					{
						true,
						0.1,
						-0.1,
						0.2,
						0.2 - -0.1
					};
				}
			}
		}

		[MemberData(nameof(TestOutput_AbsoluteTolerances_Data))]
		[Theory]
		public void TestOutput_AbsoluteTolerances(bool treatNullsAsZero, double absoluteTolerance, double? expected, double? actual, double? difference)
		{
			var plugin = new DoubleComparerPlugin
			{
				AbsoluteTolerance = absoluteTolerance,
				ToleranceModes = FloatingPointToleranceModes.AbsoluteTolerance,
				TreatMissingValuesAsZero = treatNullsAsZero
			};

			this.PerformTest(plugin, expected, actual, difference);
			this.PerformTest(plugin, actual, expected, difference.HasValue ? (double?)(0 - difference.Value) : null);
		}


		[InlineData(true, 0.01, null, 0.01, 0.01)] // Zero values cannot be equal on a relative basis
		[InlineData(true, 0.01, 0.0, 0.01, 0.01)] // Zero values cannot be equal on a relative basis
		[InlineData(true, 0.01, 0.1, 0.11, 0.11 - 0.1)] // Value shouldn't match
		[InlineData(true, 0.1, 0.1, 0.11, null)] // Value is 10% away so should match

		[Theory]
		public void TestOutput_RelativeTolerances(bool treatNullsAsZero, double relativeTolerance, double? expected, double? actual, double? difference)
		{
			var plugin = new DoubleComparerPlugin
			{
				RelativeTolerance = relativeTolerance,
				ToleranceModes = FloatingPointToleranceModes.RelativeTolerance,
				TreatMissingValuesAsZero = treatNullsAsZero
			};

			this.PerformTest(plugin, expected, actual, difference);
			this.PerformTest(plugin, actual, expected, difference.HasValue ? (double?)(0 - difference.Value) : null);
		}

		private void PerformTest(DoubleComparerPlugin plugin, double? expected, double? actual, double? difference)
		{
			var output = plugin.TryCompare("key",
				expected.HasValue ? (object)expected.Value : null,
				actual.HasValue ? (object)actual.Value : null);

			if (difference.HasValue)
			{
				Assert.Equal(ComparisonResultType.Different, output.ComparisonResultType);
				Assert.Equal(difference.Value, output.Payload);
			}
			else
				Assert.Equal(ComparisonResultType.Equal, output.ComparisonResultType);
		}
	}
}
