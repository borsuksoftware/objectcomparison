using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	/// <summary>
	/// Comparer Which can compare numeric values for all unsigned integer types, i.e. <see cref="UInt16"/>, <see cref="UInt32"/> and <see cref="UInt64"/>
	/// </summary>
	/// <remarks>This plugin will try to do its comparison using ulongs and therefore the output type will be ulong as well, which means that differences which
	/// a human would think to be negative will have wrapped round and be rather large numbers
	/// </remarks>
	public class UIntComparerPlugin : IObjectComparerPlugin
	{
		/// <summary>
		/// Get / set whether or not to treat missing values as if they were present and zero
		/// </summary>
		public bool TreatMissingValuesAsZero { get; set; } = false;

		public ObjectComparerPluginResults TryCompare(string key, object expected, object actual)
		{
			bool IsIntType(object o)
			{
				return
					o is UInt16 ||
					o is UInt32 ||
					o is UInt64;
			}

			ulong ConvertToComparisonType(object o)
			{
				return o is ulong l ? l :
					o is uint i ? (ulong)i :
					o is ushort s ? (ulong)s :
					0L;
			}

			if ((expected == null && actual == null) ||
				(expected != null && !IsIntType(expected)) ||
				(actual != null && !IsIntType(actual)))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			if (!this.TreatMissingValuesAsZero && (actual == null || expected == null))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			var expectedNumericValue = ConvertToComparisonType(expected);
			var actualNumericValue = ConvertToComparisonType(actual);

			if (expectedNumericValue == actualNumericValue)
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

			return new ObjectComparerPluginResults
			{
				ComparisonResultType = ComparisonResultType.Different,
				Payload = actualNumericValue - expectedNumericValue
			};
		}
	}
}
