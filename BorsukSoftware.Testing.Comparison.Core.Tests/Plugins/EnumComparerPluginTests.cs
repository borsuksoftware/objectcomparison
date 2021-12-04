using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
    public class EnumComparerPluginTests
    {
        [InlineData(PlatformID.Win32NT, PlatformID.Win32NT, true)]
        [InlineData(PlatformID.Win32NT, PlatformID.Unix, false)]

        [Theory]
        public void Standard(object expected, object actual, bool expectSuccess)
        {
            var plugin = new EnumComparerPlugin();

            var response = plugin.TryCompare("bob", expected, actual);
            if (expectSuccess)
            {
                Assert.Equal(ComparisonResultType.Equal, response.ComparisonResultType);
            }
            else
            {
                Assert.Equal(ComparisonResultType.Different, response.ComparisonResultType);
            }
        }

        [InlineData((int)PlatformID.Win32NT, PlatformID.Win32NT)]
        [Theory]
        public void DifferentTypes(object expected, object actual)
        {
            var plugin = new EnumComparerPlugin();

            var response = plugin.TryCompare("bob", expected, actual);
            Assert.Equal(ComparisonResultType.UnableToCompare, response.ComparisonResultType);
        }
    }
}
