using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
    public class BooleanComparerPluginTests
    {

        public static IEnumerable<object[]> Standard_Data
        {
            get
            {
                yield return new object[] {
                    true,
                    true,
                    ComparisonResultType.Equal
                };

                yield return new object[] {
                    false,
                    false,
                    ComparisonResultType.Equal
                };

                yield return new object[] {
                    true,
                    false,
                    ComparisonResultType.Different
                };

                yield return new object[] {
                    false,
                    true,
                    ComparisonResultType.Different
                };

                yield return new object[] {
                    null,
                    true,
                    ComparisonResultType.UnableToCompare
                };

                yield return new object[] {
                    null,
                    false,
                    ComparisonResultType.UnableToCompare
                };

                yield return new object[] {
                    true,
                    null,
                    ComparisonResultType.UnableToCompare
                };

                yield return new object[] {
                    false,
                    null,
                    ComparisonResultType.UnableToCompare
                };

                yield return new object[] {
                    true,
                    "bob",
                    ComparisonResultType.UnableToCompare
                };

                yield return new object[] {
                    "bob",
                    true,
                    ComparisonResultType.UnableToCompare
                };
            }
        }


        [MemberData(nameof(Standard_Data))]
        [Theory]
        public void Standard(object expected, object actual, ComparisonResultType expectedResultType)
        {
            var plugin = new BooleanComparerPlugin();

            var result = plugin.TryCompare("key", expected, actual);

            Assert.Equal(expectedResultType, result.ComparisonResultType);
        }
    }
}
