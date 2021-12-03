using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
    public class TimeSpanComparerPlugin : IObjectComparerPlugin
    {
        public ObjectComparerPluginResults TryCompare(string key, object expected, object actual)
        {
            if ((expected != null && !(expected is TimeSpan)) ||
                (actual != null && !(actual is TimeSpan)))
                return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

            // Uncertain as to what payload makes sense here....
            if ((expected == null && actual != null) ||
                (actual == null && expected != null))
            {

                if (actual == null)
                    return new ObjectComparerPluginResults
                    {
                        ComparisonResultType = ComparisonResultType.Different,
                        Payload = TimeSpan.Zero - (TimeSpan)expected,
                    };

                if (expected == null)
                    return new ObjectComparerPluginResults
                    {
                        ComparisonResultType = ComparisonResultType.Different,
                        Payload = (TimeSpan)actual
                    };
            }

            var expectedTimeSpan = (TimeSpan)expected;
            var actualTimeSpan = (TimeSpan)actual;

            if (expectedTimeSpan == actualTimeSpan)
                return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

            return new ObjectComparerPluginResults
            {
                ComparisonResultType = ComparisonResultType.Different,
                Payload = actualTimeSpan - expectedTimeSpan
            };
        }
    }
}