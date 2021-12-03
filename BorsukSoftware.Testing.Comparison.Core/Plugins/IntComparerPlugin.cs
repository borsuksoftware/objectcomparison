using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	/// <summary>
	/// Comparer Which can compare numeric values for all signed integer types, i.e. <see cref="sbyte"/>, <see cref="Int16"/>, <see cref="Int32"/> and <see cref="Int64"/>
	/// </summary>
	public class IntComparerPlugin : IObjectComparerPlugin
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
					o is sbyte ||
					o is Int16 ||
					o is Int32 ||
					o is Int64;
			}

			long ConvertToComparisonType( object o )
			{
				return o is long l ? l :
					o is int i ? (long)i :
					o is short s ? (long)s :
					o is sbyte sb ? (long)sb :
					0L;
			}

			if ((expected == null && actual == null) ||
				(expected != null && !IsIntType(expected)) ||
				(actual != null && !IsIntType(actual)))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			if( !this.TreatMissingValuesAsZero && (actual == null || expected == null))
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
