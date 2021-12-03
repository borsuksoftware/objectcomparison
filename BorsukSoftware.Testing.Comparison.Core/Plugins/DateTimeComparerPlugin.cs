using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
    public class DateTimeComparerPlugin : IObjectComparerPlugin
    {
        public bool IgnoreTime { get; set; }

        public ObjectComparerPluginResults TryCompare(string key, object expected, object actual)
        {
            if ((expected != null && !(expected is DateTime)) ||
                (actual != null && !(actual is DateTime)))
                return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

            // Uncertain as to what payload makes sense here....
            if ((expected == null && actual != null) ||
                (actual == null && expected != null))
            {

                if (actual == null)
                    return new ObjectComparerPluginResults
                    {
                        ComparisonResultType = ComparisonResultType.Different,
                        Payload = new TimeSpan(-((DateTime)expected).Ticks),
                    };

                if (expected == null)
                    return new ObjectComparerPluginResults
                    {
                        ComparisonResultType = ComparisonResultType.Different,
                        Payload = new TimeSpan(((DateTime)actual).Ticks)
                    };
            }

            var expectedDateTime = (DateTime)expected;
            var actualDateTime = (DateTime)actual;

            if (this.IgnoreTime)
            {
                expectedDateTime = expectedDateTime.Date;
                actualDateTime = actualDateTime.Date;
            }

            if (expectedDateTime == actualDateTime)
                return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

            return new ObjectComparerPluginResults
            {
                ComparisonResultType = ComparisonResultType.Different,
                Payload = actualDateTime - expectedDateTime
            };
        }
    }
}