using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
    public class DateTimeComparerPluginTests
    {

        public static IEnumerable<object[]> Standard_Data
        {
            get
            {
                var standardPlugin = new DateTimeComparerPlugin
                {
                    IgnoreTime = false
                };

                var ignoreTimePlugin = new DateTimeComparerPlugin
                {
                    IgnoreTime = true
                };

                yield return new object[] {
                    standardPlugin,
                    new DateTime(2020,06,12),
                    new DateTime(2020,06,12),
                    null
                };

                yield return new object[] {
                    ignoreTimePlugin,
                    new DateTime(2020,06,12),
                    new DateTime(2020,06,12,12,12,1),
                    null
                };

                yield return new object[] {
                    standardPlugin,
                    new DateTime(2020,06,12),
                    new DateTime(2020,06,12,12,12,1),
                    new TimeSpan(12,12,1)
                };

                yield return new object[] {
                    standardPlugin,
                    new DateTime(2020,06,12),
                    new DateTime(2020,06,13),
                    new TimeSpan(24,0,0)
                };

            }
        }


        [MemberData(nameof(Standard_Data))]
        [Theory]
        public void Standard(DateTimeComparerPlugin plugin, DateTime? expected, DateTime? actual, TimeSpan? difference)
        {
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
