using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	/// <summary>
	/// Enum detailing what tolerance modes should be used when comparing numbers
	/// </summary>
	[Flags]
	public enum FloatingPointToleranceModes
	{
		/// <summary>
		/// No tolerance, only exact matches
		/// </summary>
		None = 0,

		/// <summary>
		/// Allow values to match if they're within a certain fixed value
		/// </summary>
		AbsoluteTolerance = 1,

		/// <summary>
		/// Allow values to match if they're within a certain percentage of each other. This is calculated as a percentage of the smaller (in absolute
		/// terms) number
		/// </summary>
		RelativeTolerance = 2,

		/// <summary>
		/// Activate all tolerance modes
		/// </summary>
		All = AbsoluteTolerance | RelativeTolerance
	}
}
