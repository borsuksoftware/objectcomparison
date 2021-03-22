using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	/// <summary>
	/// Plugin to compare string instances, allowing for nulls and to ignore case
	/// </summary>
	/// <remarks>This plugin doesn't allow for more advanced string comparison, e.g. handling backspaces / delete characters
	/// nor comparisons ignoring things like carriage returns etc. If those features are required, then an additional plugin 
	/// should be created.</remarks>
	public class SimpleStringComparerPlugin : IObjectComparerPlugin
	{
		#region Settings

		public bool TreatNullAndEmptyStringsAsEqual { get; set; } = true;

		public bool IgnoreCase { get; set; } = false;

		#endregion

		#region IObjectComparerPlugin Members

		public ObjectComparerPluginResults TryCompare(string key, object expected, object actual)
		{
			if ((expected != null && !(expected is string) ||
				(actual != null && !(actual is string))))
			{
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };
			}

			var expectedString = expected as string;
			var actualString = actual as string;

			if (this.TreatNullAndEmptyStringsAsEqual &&
				string.IsNullOrEmpty(expectedString) &&
				string.IsNullOrEmpty(actualString))
			{
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };
			}

			var underlingSC = this.IgnoreCase ?
				StringComparer.CurrentCultureIgnoreCase :
				StringComparer.CurrentCulture;

			var stringComparisonResult = underlingSC.Compare(actualString, expectedString);
			if (stringComparisonResult == 0)
			{
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };
			}

			// Note, no payload
			return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Different };
		}

		#endregion
	}
}