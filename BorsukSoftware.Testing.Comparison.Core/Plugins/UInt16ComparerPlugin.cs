using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	/// <summary>
	/// Comparer plugin for <see cref="ushort"/> objects
	/// </summary>
	/// <remarks>The output type for differences here is int due to this being the default type of subtracting 2 ushorts in .net</remarks>
	public class UInt16ComparerPlugin : IObjectComparerPlugin
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
			// Check to see if we can compare as unsigned shorts
			if ((actual == null && expected == null) ||
				(actual != null && !(actual is ushort)) ||
				(expected != null && !(expected is ushort)))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			if (!this.TreatMissingValuesAsZero && (expected == null || actual == null))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			ushort expectedUShort = expected == null ? (ushort)0 : (ushort)expected;
			ushort actualUShort = actual == null ? (ushort)0 : (ushort)actual;

			if (expectedUShort == actualUShort)
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

			return new ObjectComparerPluginResults
			{
				ComparisonResultType = ComparisonResultType.Different,
				Payload = actualUShort - expectedUShort
			};
		}
	}
}
