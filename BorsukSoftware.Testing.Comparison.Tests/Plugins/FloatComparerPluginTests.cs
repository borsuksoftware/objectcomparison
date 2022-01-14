using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	public class FloatComparerPluginTests
	{
		[InlineData(true, 0.1f, 0.2f, null, 0.2f, 0.2f)]
		[InlineData(true, 0.1f, 0.2f, null, 0.1f, null)] // Within absolute tolerance
		[InlineData(true, 0.1f, 0.2f, 100000f, 100001f, null)] // Within relative tolerance
		[Theory]
		public void TestOutput_BothToleranceModes(bool treatNullsAsZero,
			float absoluteTolerance,
			float relativeTolerance,
			float? expected,
			float? actual,
			float? difference)
		{
			var plugin = new FloatComparerPlugin
			{
				AbsoluteTolerance = absoluteTolerance,
				RelativeTolerance = relativeTolerance,
				ToleranceModes = FloatingPointToleranceModes.AbsoluteTolerance | FloatingPointToleranceModes.RelativeTolerance,
				TreatMissingValuesAsZero = treatNullsAsZero
			};

			this.PerformTest(plugin, expected, actual, difference);
			this.PerformTest(plugin, actual, expected, difference.HasValue ? (float?)(0 - difference.Value) : null);
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
						0.1f,
						null,
						0.1f,
						null
					};

					yield return new object[]
					{
						true,
						0.1f,
						null,
						-0.1f,
						null
					};

					yield return new object[]
					{
						true,
						0.1f,
						null,
						0.2f,
						0.2f
					};
				}

				// Handle general values
				{
					yield return new object[]
					{
						true,
						0.1f,
						0.2f,
						0.1f,
						null
					};

					yield return new object[]
					{
						true,
						0.1f,
						-.01f,
						-0.1f,
						null
					};

					yield return new object[]
					{
						true,
						0.1f,
						-0.1f,
						0.2f,
						0.2f - -0.1f
					};
				}
			}
		}

		[MemberData(nameof(TestOutput_AbsoluteTolerances_Data))]
		[Theory]
		public void TestOutput_AbsoluteTolerances(bool treatNullsAsZero, float absoluteTolerance, float? expected, float? actual, float? difference)
		{
			var plugin = new FloatComparerPlugin
			{
				AbsoluteTolerance = absoluteTolerance,
				ToleranceModes = FloatingPointToleranceModes.AbsoluteTolerance,
				TreatMissingValuesAsZero = treatNullsAsZero
			};

			this.PerformTest(plugin, expected, actual, difference);
			this.PerformTest(plugin, actual, expected, difference.HasValue ? (float?)(0 - difference.Value) : null);
		}


		[InlineData(true, 0.01f, null, 0.01f, 0.01f)] // Zero values cannot be equal on a relative basis
		[InlineData(true, 0.01f, 0.0f, 0.01f, 0.01f)] // Zero values cannot be equal on a relative basis
		[InlineData(true, 0.01f, 0.1f, 0.11f, 0.11f - 0.1f)] // Value shouldn't match
		[InlineData(true, 0.1f, 0.1f, 0.11f, null)] // Value is 10% away so should match

		[Theory]
		public void TestOutput_RelativeTolerances(bool treatNullsAsZero, float relativeTolerance, float? expected, float? actual, float? difference)
		{
			var plugin = new FloatComparerPlugin
			{
				RelativeTolerance = relativeTolerance,
				ToleranceModes = FloatingPointToleranceModes.RelativeTolerance,
				TreatMissingValuesAsZero = treatNullsAsZero
			};

			this.PerformTest(plugin, expected, actual, difference);
			this.PerformTest(plugin, actual, expected, difference.HasValue ? (float?)(0 - difference.Value) : null);
		}

		private void PerformTest(FloatComparerPlugin plugin, float? expected, float? actual, float? difference)
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
