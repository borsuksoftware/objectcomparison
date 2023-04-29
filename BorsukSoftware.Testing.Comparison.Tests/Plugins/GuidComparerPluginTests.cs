using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
    public class GuidComparerPluginTests
    {

        public static IEnumerable<object[]> Standard_Data
        {
            get
            {
                var guidA = Guid.NewGuid();
                var guidB = Guid.NewGuid();
                Assert.NotEqual(guidA, guidB);

                yield return new object[] {
                    guidA,
                    guidA,
                    ComparisonResultType.Equal
                };

                yield return new object[] {
                    guidA,
                    guidB,
                    ComparisonResultType.Different
                };

                yield return new object[] {
                    null,
                    guidB,
                    ComparisonResultType.UnableToCompare
                };

                yield return new object[] {
                    guidA,
                    null,
                    ComparisonResultType.UnableToCompare
                };

                yield return new object[] {
                    guidA,
                    "bob",
                    ComparisonResultType.UnableToCompare
                };

                yield return new object[] {
                    "bob",
                    guidB,
                    ComparisonResultType.UnableToCompare
                };
            }
        }


        [MemberData(nameof(Standard_Data))]
        [Theory]
        public void Standard(object expected, object actual, ComparisonResultType expectedResultType)
        {
            var plugin = new GuidComparerPlugin();

            var result = plugin.TryCompare("key", expected, actual);

            Assert.Equal(expectedResultType, result.ComparisonResultType);
        }
    }
}
