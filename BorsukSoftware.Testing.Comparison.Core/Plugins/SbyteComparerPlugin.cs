using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	/// <summary>
	/// Comparer plugin for <see cref="sbyte"/> objects
	/// </summary>
	/// <remarks>The output type for differences here is int due to this being the default type of subtracting 2 sbytes in .net</remarks>
	public class SByteComparerPlugin : IObjectComparerPlugin
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
			// Check to see if we can compare as sbytes
			if ((actual == null && expected == null) ||
				(actual != null && !(actual is sbyte)) ||
				(expected != null && !(expected is sbyte)))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			if (!this.TreatMissingValuesAsZero && (expected == null || actual == null))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			sbyte expectedSByte = expected == null ? (sbyte)0 : (sbyte)expected;
            sbyte actualSByte = actual == null ? (sbyte)0 : (sbyte)actual;

			if (expectedSByte == actualSByte)
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

			return new ObjectComparerPluginResults
			{
				ComparisonResultType = ComparisonResultType.Different,
				Payload = actualSByte - expectedSByte
			};
		}
	}
}
