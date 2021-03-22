using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	/// <summary>
	/// Comparer plugin for <see cref="short"/> objects
	/// </summary>
	/// <remarks>The output type for differences here is int due to this being the default type of subtracting 2 shorts in .net</remarks>
	public class Int16ComparerPlugin : IObjectComparerPlugin
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
			// Check to see if we can compare as shorts
			if ((actual == null && expected == null) ||
				(actual != null && !(actual is short)) ||
				(expected != null && !(expected is short)))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			if (!this.TreatMissingValuesAsZero && (expected == null || actual == null))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			short expectedShort = expected == null ? (short)0 : (short)expected;
			short actualShort = actual == null ? (short)0 : (short)actual;

			if (expectedShort == actualShort)
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

			return new ObjectComparerPluginResults
			{
				ComparisonResultType = ComparisonResultType.Different,
				Payload = actualShort - expectedShort
			};
		}
	}
}
