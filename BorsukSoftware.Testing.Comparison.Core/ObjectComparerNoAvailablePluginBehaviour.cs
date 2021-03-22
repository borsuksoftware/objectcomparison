using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison
{
	/// <summary>
	/// Enum detailing how an instance of <see cref="ObjectComparer"/> should behave if it comes across a value to
	/// compare for which no plugin claims responsibility.
	/// </summary>
	public enum ObjectComparerNoAvailablePluginBehaviour
	{
		Throw = 1,
		Ignore = 2,
		ReportAsDifference = 3,
	}
}
