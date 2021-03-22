using System;
using System.Collections.Generic;
using System.Numerics;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	/// <summary>
	/// Comparer plugin for <see cref="BigInteger"/> objects
	/// </summary>
	public class BigIntegerComparerPlugin : IObjectComparerPlugin
	{
		/// <summary>
		/// Get / set whether or not to treat missing values as if they were present and zero
		/// </summary>
		public bool TreatMissingValuesAsZero { get; set; } = false;

		public ObjectComparerPluginResults TryCompare(
			string key,
			object expected,
			object actual)
		{
			// Check to see if we can compare as BigIntegers
			if ((actual == null && expected == null) ||
				(actual != null && !(actual is BigInteger)) ||
				(expected != null && !(expected is BigInteger)))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			if (!this.TreatMissingValuesAsZero && (expected == null || actual == null))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			var expectedBigInteger = expected == null ? 0 : (BigInteger)expected;
			var actualBigInteger = actual == null ? 0 : (BigInteger)actual;

			if (expectedBigInteger == actualBigInteger)
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

			return new ObjectComparerPluginResults
			{
				ComparisonResultType = ComparisonResultType.Different,
				Payload = actualBigInteger - expectedBigInteger
			};
		}
	}
}
