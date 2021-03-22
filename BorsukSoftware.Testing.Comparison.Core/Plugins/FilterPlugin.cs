using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	/// <summary>
	/// Filter plugin to allow a user to easily apply a filter to an existing plugin
	/// </summary>
	public class FilterPlugin : IObjectComparerPlugin
	{
		public IObjectComparerPlugin UnderlyingPlugin { get; private set; }
		public Func<string, object, object, bool> CanHandleFunc { get; private set; }

		public FilterPlugin(IObjectComparerPlugin underlyingPlugin, Func<string, object, object, bool> canHandleFunc)
		{
			if (underlyingPlugin == null)
				throw new ArgumentNullException(nameof(underlyingPlugin));

			if (canHandleFunc == null)
				throw new ArgumentNullException(nameof(canHandleFunc));

			this.UnderlyingPlugin = underlyingPlugin;
			this.CanHandleFunc = canHandleFunc;
		}

		public ObjectComparerPluginResults TryCompare(string key, object expected, object actual)
		{
			if (this.CanHandleFunc(key, expected, actual))
			{
				return this.UnderlyingPlugin.TryCompare(key, expected, actual);
			}

			return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };
		}
	}
}
