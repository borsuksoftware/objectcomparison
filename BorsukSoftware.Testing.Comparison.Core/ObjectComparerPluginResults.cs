using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison
{
	/// <summary>
	/// Results object for instances of <see cref="IObjectComparerPlugin"/>
	/// </summary>
	public struct ObjectComparerPluginResults
	{
		/// <summary>
		/// Get / set the nature of the comparison result
		/// </summary>
		/// <value>The result type</value>
		public ComparisonResultType ComparisonResultType { get; set; }

		/// <summary>
		/// Get / set the optional payload
		/// </summary>
		/// <value>The optional payload</value>
		public object Payload { get; set; }
	}
}
