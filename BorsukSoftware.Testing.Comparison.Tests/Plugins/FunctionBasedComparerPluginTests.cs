using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	public class FunctionBasedComparerPluginTests
	{
		[Fact]
		public void FunctionBasedPlugin_Dictionary()
		{
			var comparer = new ObjectComparer()
			{
				ObjectComparerNoAvailablePluginBehaviour = ObjectComparerNoAvailablePluginBehaviour.Throw
			};

			comparer.ComparisonPlugins.Add(new Plugins.FunctionBasedComparerPlugin((key, expected, actual) => {
				if (expected is IDictionary<string, object> expectedDictionary &&
					actual is IDictionary<string, object> actualDictionary)
				{
					return new ObjectComparerPluginResults
					{
						ComparisonResultType = ComparisonResultType.Equal
					};
				}

				return new ObjectComparerPluginResults
				{
					ComparisonResultType = ComparisonResultType.UnableToCompare
				};
			}));



		}
	}
}
