using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison
{
	/// <summary>
	/// Interface to access all object comparison functionality
	/// </summary>
	/// <remarks>Note that a plugin is expected to return a single payload value for a comparison, i.e. one cannot 
	/// provide an iterator for a given address. However, if one wishes to return a complex difference payload, then 
	/// simply return an object which captures the desired information and consume it subsequently.</remarks>
	public interface IObjectComparer
	{
		/// <summary>
		/// Compare the supplied values
		/// </summary>
		/// <param name="expectedValues">The values which one is expecting</param>
		/// <param name="actualValues">The values which one actually received</param>
		/// <returns>An enumeration over the differences</returns>
		IEnumerable<KeyValuePair<string, ComparisonResults>> CompareValues(IEnumerable<KeyValuePair<string, object>> expectedValues, IEnumerable<KeyValuePair<string, object>> actualValues);
	}
}
