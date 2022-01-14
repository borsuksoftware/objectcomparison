using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	public class DecimalComparerPluginTests
	{
		public static IEnumerable<object[]> TestOutput_BothToleranceModes_Data
		{
			get
			{
				yield return new object[] { true, 0.1m, 0.2m, null, 0.2m, 0.2m };
				yield return new object[] { true, 0.1m, 0.2m, null, 0.1m, null };
				yield return new object[] { true, 0.1m, 0.2m, 100000m, 100001m, null };
			}
		}

		[MemberData(nameof(TestOutput_BothToleranceModes_Data))]
		[Theory]
		public void TestOutput_BothToleranceModes(bool treatNullsAsZero,
			decimal absoluteTolerance,
			decimal relativeTolerance,
			decimal? expected,
			decimal? actual,
			decimal? difference)
		{
			var plugin = new DecimalComparerPlugin
			{
				AbsoluteTolerance = absoluteTolerance,
				RelativeTolerance = relativeTolerance,
				ToleranceModes = FloatingPointToleranceModes.AbsoluteTolerance | FloatingPointToleranceModes.RelativeTolerance,
				TreatMissingValuesAsZero = treatNullsAsZero
			};

			this.PerformTest(plugin, expected, actual, difference);
			this.PerformTest(plugin, actual, expected, difference.HasValue ? (decimal?)(0 - difference.Value) : null);
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
						0.1m,
						null,
						0.1m,
						null
					};

					yield return new object[]
					{
						true,
						0.1m,
						null,
						-0.1m,
						null
					};

					yield return new object[]
					{
						true,
						0.1m,
						null,
						0.2m,
						0.2m
					};
				}

				// Handle general values
				{
					yield return new object[]
					{
						true,
						0.1m,
						0.2m,
						0.1m,
						null
					};

					yield return new object[]
					{
						true,
						0.1m,
						-.01m,
						-0.1m,
						null
					};

					yield return new object[]
					{
						true,
						0.1m,
						-0.1m,
						0.2m,
						0.2m - -0.1m
					};
				}
			}
		}

		[MemberData(nameof(TestOutput_AbsoluteTolerances_Data))]
		[Theory]
		public void TestOutput_AbsoluteTolerances(bool treatNullsAsZero, decimal absoluteTolerance, decimal? expected, decimal? actual, decimal? difference)
		{
			var plugin = new DecimalComparerPlugin
			{
				AbsoluteTolerance = absoluteTolerance,
				ToleranceModes = FloatingPointToleranceModes.AbsoluteTolerance,
				TreatMissingValuesAsZero = treatNullsAsZero
			};

			this.PerformTest(plugin, expected, actual, difference);
			this.PerformTest(plugin, actual, expected, difference.HasValue ? (decimal?)(0 - difference.Value) : null);
		}


		public static IEnumerable<object[]> TestOutput_RelativeTolerances_Data
		{
			get
			{
				yield return new object[] { true, 0.01m, null, 0.01m, 0.01m};
				yield return new object[] { true, 0.01m, 0.0m, 0.01m, 0.01m};
				yield return new object[] { true, 0.01m, 0.1m, 0.11m, 0.11m - 0.1m};
				yield return new object[] { true, 0.1m, 0.1m, 0.11m, null};
			}
		}

		[MemberData(nameof(TestOutput_RelativeTolerances_Data))]
		[Theory]
		public void TestOutput_RelativeTolerances(bool treatNullsAsZero, decimal relativeTolerance, decimal? expected, decimal? actual, decimal? difference)
		{
			var plugin = new DecimalComparerPlugin
			{
				RelativeTolerance = relativeTolerance,
				ToleranceModes = FloatingPointToleranceModes.RelativeTolerance,
				TreatMissingValuesAsZero = treatNullsAsZero
			};

			this.PerformTest(plugin, expected, actual, difference);
			this.PerformTest(plugin, actual, expected, difference.HasValue ? (decimal?)(0 - difference.Value) : null);
		}

		private void PerformTest(DecimalComparerPlugin plugin, decimal? expected, decimal? actual, decimal? difference)
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
