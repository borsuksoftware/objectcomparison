using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison
{
	/// <summary>
	/// Interface for the components which perform the actual comparison
	/// </summary>
	public interface IObjectComparerPlugin
	{
		/// <summary>
		/// Try to compare the supplied values
		/// </summary>
		/// <param name="key">The address of the value to be processed</param>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <returns>A results object containing information about differences etc.</returns>
		ObjectComparerPluginResults TryCompare(string key, object expected, object actual);
	}
}
