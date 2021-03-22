using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	public class ReferenceEqualityComparerPluginTests
	{
		[Fact]
		public void Same()
		{
			var plugin = new ReferenceEqualityComparerPlugin();

			var o = new { prop = 2 };

			var output = plugin.TryCompare("key", o, o);
			Assert.Equal(ComparisonResultType.Equal, output.ComparisonResultType);
		}

		[Fact]
		public void NotSame_Different()
		{
			var plugin = new ReferenceEqualityComparerPlugin()
			{
				NotSameObjectComparisonResultType = ComparisonResultType.Different
			};

			var o = new { prop = 2 };

			var output = plugin.TryCompare("key", o, null);
			Assert.Equal(ComparisonResultType.Different, output.ComparisonResultType);
		}

		[Fact]
		public void NotSame_Skip()
		{
			var plugin = new ReferenceEqualityComparerPlugin()
			{
				NotSameObjectComparisonResultType = ComparisonResultType.UnableToCompare
			};

			var o = new { prop = 2 };

			var output = plugin.TryCompare("key", o, null);
			Assert.Equal(ComparisonResultType.UnableToCompare, output.ComparisonResultType);
		}
	}
}
