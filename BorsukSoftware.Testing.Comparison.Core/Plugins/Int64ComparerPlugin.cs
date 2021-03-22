using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	/// <summary>
	/// Comparer plugin for <see cref="long"/> objects
	/// </summary>
	public class Int64ComparerPlugin : IObjectComparerPlugin
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
			// Check to see if we can compare as longs
			if ((actual == null && expected == null) ||
				(actual != null && !(actual is long)) ||
				(expected != null && !(expected is long)))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			if (!this.TreatMissingValuesAsZero && (expected == null || actual == null))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			long expectedLong = expected == null ? 0 : (long)expected;
			long actualLong = actual == null ? 0 : (long)actual;

			if (expectedLong == actualLong)
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

			return new ObjectComparerPluginResults
			{
				ComparisonResultType = ComparisonResultType.Different,
				Payload = actualLong - expectedLong
			};
		}
	}
}
