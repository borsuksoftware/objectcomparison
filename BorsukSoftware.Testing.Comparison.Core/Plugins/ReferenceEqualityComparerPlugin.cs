using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	/// <summary>
	/// Helper plugin to check equality based off reference equality
	/// </summary>
	/// <remarks>This plugin can be useful under some specialised circumstances and is included for completion.</remarks>
	public class ReferenceEqualityComparerPlugin : IObjectComparerPlugin
	{
		/// <summary>
		/// Get / set the desired behaviour in case objects are not the same underlying object
		/// </summary>
		public ComparisonResultType NotSameObjectComparisonResultType { get; set; } = ComparisonResultType.UnableToCompare;

		public ObjectComparerPluginResults TryCompare(string key, object expected, object actual)
		{
			if (expected == null && actual == null)
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

			if (expected != null && actual != null &&
				ReferenceEquals(expected, actual))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

			return new ObjectComparerPluginResults { ComparisonResultType = this.NotSameObjectComparisonResultType };
		}
	}
}
