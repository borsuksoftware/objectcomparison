using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	/// <summary>
	/// Plugin to allow a user to provide functions for object comparison
	/// </summary>
	public class FunctionBasedComparerPlugin : IObjectComparerPlugin
	{
		#region Data Model

		public Func<string, object, object, ObjectComparerPluginResults> TryCompareFunc { get; private set; }

		#endregion

		public FunctionBasedComparerPlugin(Func<string, object, object, ObjectComparerPluginResults> tryCompareFunc)
		{
			if (tryCompareFunc == null)
				throw new ArgumentNullException(nameof(tryCompareFunc));

			this.TryCompareFunc = tryCompareFunc;
		}

		public ObjectComparerPluginResults TryCompare(string key, object expected, object actual)
		{
			return this.TryCompareFunc(key, expected, actual);
		}
	}
}
