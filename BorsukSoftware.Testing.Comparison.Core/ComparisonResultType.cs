using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison
{
	/// <summary>
	/// Enum detailing the result from an instance of <see cref="IObjectComparerPlugin"/>
	/// </summary>
	public enum ComparisonResultType
	{
		/// <summary>
		/// The plugin wasn't able to compare the values for whatever reason, e.g. it has been filtered out, the plugin can't deal with the value types etc.
		/// </summary>
		UnableToCompare = 0,

		/// <summary>
		/// The values were equal 
		/// </summary>
		Equal = 1,

		/// <summary>
		/// The values were different
		/// </summary>
		Different = 2,
	}
}
