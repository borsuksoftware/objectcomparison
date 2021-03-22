using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	/// <summary>
	/// Comparer plugin for <see cref="uint"/> objects
	/// </summary>
	public class UInt32ComparerPlugin : IObjectComparerPlugin
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
			// Check to see if we can compare as unsigned integers
			if ((actual == null && expected == null) ||
				(actual != null && !(actual is uint)) ||
				(expected != null && !(expected is uint)))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			if (!this.TreatMissingValuesAsZero && (expected == null || actual == null))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			uint expectedUint = expected == null ? 0 : (uint)expected;
			uint actualUint = actual == null ? 0 : (uint)actual;

			if (expectedUint == actualUint)
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

			return new ObjectComparerPluginResults
			{
				ComparisonResultType = ComparisonResultType.Different,
				Payload = actualUint - expectedUint
			};
		}
	}
}
