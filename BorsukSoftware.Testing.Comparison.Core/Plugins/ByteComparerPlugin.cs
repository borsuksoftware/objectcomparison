using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
    /// <summary>
    /// Comparer plugin for <see cref="byte"/> objects
    /// </summary>
    /// <remarks>The output type for differences here is int due to this being the default type of subtracting 2 bytes in .net</remarks>
    public class ByteComparerPlugin : IObjectComparerPlugin
    {
        /// <summary>
        /// Get / set whether or not to treat missing values as if they were present and zero
        /// </summary>
        public bool TreatMissingValuesAsZero { get; set; } = false;

        public ObjectComparerPluginResults TryCompare(
            string key,
            object expected,
            object actual)
        {
            // Check to see if we can compare as bytes
            if ((actual == null && expected == null) ||
                (actual != null && !(actual is byte)) ||
                (expected != null && !(expected is byte)))
                return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

            if (!this.TreatMissingValuesAsZero && (expected == null || actual == null))
                return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

            byte expectedByte = expected == null ? (byte)0 : (byte)expected;
            byte actualByte = actual == null ? (byte)0 : (byte)actual;

            if (expectedByte == actualByte)
                return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

            return new ObjectComparerPluginResults
            {
                ComparisonResultType = ComparisonResultType.Different,
                Payload = actualByte - expectedByte
            };
        }
    }
}
