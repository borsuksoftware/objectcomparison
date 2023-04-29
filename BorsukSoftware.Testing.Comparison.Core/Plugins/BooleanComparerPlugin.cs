using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
    /// <summary>
    /// Comparer plugin for <see cref="bool"/> objects
    /// </summary>
    /// <remarks>Note that there is no payload</remarks>
    public class BooleanComparerPlugin : IObjectComparerPlugin
    {
        public ObjectComparerPluginResults TryCompare(
            string key,
            object expected,
            object actual)
        {
            // Check to see if we can compare as bools
            if ((actual == null && expected == null) ||
                (actual != null && !(actual is bool)) ||
                (expected != null && !(expected is bool)))
                return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

            if (actual == null || expected == null)
                return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

            bool expectedBool = (bool)expected ;
            bool actualBool = (bool)actual;

            if (expectedBool == actualBool)
                return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

            return new ObjectComparerPluginResults
            {
                ComparisonResultType = ComparisonResultType.Different,
                Payload = null
            };
        }
    }
}
