using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	public class SimpleStringComparerPluginTests
	{
		[InlineData(null, null)]
		[InlineData("", null)]
		[InlineData(null, "")]
		[InlineData("", "")]
		[Theory]
		public void CheckNullHandling(string expected, string actual)
		{
			var plugin = new SimpleStringComparerPlugin()
			{
				TreatNullAndEmptyStringsAsEqual = true
			};

			var output = plugin.TryCompare("pair", expected, actual);
			Assert.Equal(ComparisonResultType.Equal, output.ComparisonResultType);
		}

		[InlineData(true, "bob", "BOB", ComparisonResultType.Equal)]
		[InlineData(false, "BOB", "BOB", ComparisonResultType.Equal)]
		[InlineData(false, "bob", "BOB", ComparisonResultType.Different)]
		[InlineData(true, "BOB", "bob", ComparisonResultType.Equal)]
		[InlineData(false, "BOB", "bob", ComparisonResultType.Different)]
		[InlineData(false, null, "BOB", ComparisonResultType.Different)]
		[InlineData(false, "BOB", null, ComparisonResultType.Different)]
		[Theory]
		public void CheckValues(bool ignoreCase, string expected, string actual, ComparisonResultType expectedResult)
		{
			var plugin = new SimpleStringComparerPlugin()
			{
				IgnoreCase = ignoreCase
			};

			var output = plugin.TryCompare("key", expected, actual);

			Assert.Equal(expectedResult, output.ComparisonResultType);


		}
	}
}
