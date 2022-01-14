using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	public class FilterPluginTests
	{
		[Fact]
		public void ExcludeWorks()
		{
			var plugin = new FilterPlugin(
				new FunctionBasedComparerPlugin((s, o, o2) => throw new Exception()),
				(s, o, o2) => false);

			var output = plugin.TryCompare("pair", 1, 2);

			Assert.Equal(ComparisonResultType.UnableToCompare, output.ComparisonResultType);
		}

		[Fact]
		public void IncludeWorks()
		{
			var plugin = new FilterPlugin(
				new FunctionBasedComparerPlugin((s, o, o2) => new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal, Payload = "Bob" }),
				(s, o, o2) => true);

			var output = plugin.TryCompare("pair", 1, 2);

			Assert.Equal(ComparisonResultType.Equal, output.ComparisonResultType);
			Assert.Equal("Bob", output.Payload);
		}
	}
}
