using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison
{
	/// <summary>
	/// Wrapper results object for a given key
	/// </summary>
	public struct ComparisonResults
	{
		/// <summary>
		/// Get / set the expected value for the comparison
		/// </summary>
		public object ExpectedValue { get; set; }

		/// <summary>
		/// Get / set the actual value for the comparison
		/// </summary>
		public object ActualValue { get; set; }

		/// <summary>
		/// Get / set the comparison result payload
		/// </summary>
		/// <remarks>The type of this will vary dramatically based off what is being compared / the requirements etc.</remarks>
		public object ComparisonPayload { get; set; }
	}
}
