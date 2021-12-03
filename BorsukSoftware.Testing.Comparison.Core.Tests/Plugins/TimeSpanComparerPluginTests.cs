using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
    public class TimeSpanComparerPluginTests
    {
        public static IEnumerable<object[]> Standard_Data
        {
            get
            {
                yield return new object[] {
                    new TimeSpan(12,30,01),
                    new TimeSpan(12,30,01),
                    null
                };

                yield return new object[] {
                    new TimeSpan(12,30,01),
                    null,
                    new TimeSpan(-12,-30,-01)
                };

                yield return new object[] {
                    null,
                    new TimeSpan(12,30,01),
                    new TimeSpan(12,30,01)
                };

                yield return new object[] {
                    new TimeSpan(13,05,12),
                    new TimeSpan(13,05,13),
                    new TimeSpan(0,0,1)
                };
            }
        }

        [MemberData(nameof(Standard_Data))]
        [Theory]
        public void Standard(TimeSpan? expected, TimeSpan? actual, TimeSpan? difference)
        {
            var plugin = new TimeSpanComparerPlugin();
            var result = plugin.TryCompare("key", expected, actual);

            if (difference == null)
            {
                Xunit.Assert.Equal(ComparisonResultType.Equal, result.ComparisonResultType);
            }
            else
            {
                Xunit.Assert.Equal(ComparisonResultType.Different, result.ComparisonResultType);

                Xunit.Assert.IsType<TimeSpan>(result.Payload);
                var typedPayload = (TimeSpan)result.Payload;

                Assert.Equal(difference, typedPayload);
            }
        }
    }
}
