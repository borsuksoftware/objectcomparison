using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
    /// <summary>
    /// Comparer plugin for <see cref="Guid"/> objects
    /// </summary>
    public class GuidComparerPlugin : IObjectComparerPlugin
    {
        public ObjectComparerPluginResults TryCompare(
            string key,
            object expected,
            object actual)
        {
            // Check to see if we can compare as Guids
            if ((actual == null && expected == null) ||
                (actual != null && !(actual is Guid)) ||
                (expected != null && !(expected is Guid)))
                return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

            if (expected == null || actual == null)
                return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

            Guid expectedGuid = (Guid)expected;
            Guid actualGuid = (Guid)actual;

            if (expectedGuid == actualGuid)
                return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

            return new ObjectComparerPluginResults
            {
                ComparisonResultType = ComparisonResultType.Different,
                Payload = null
            };
        }
    }
}
