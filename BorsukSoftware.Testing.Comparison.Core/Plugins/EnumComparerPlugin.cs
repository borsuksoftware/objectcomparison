namespace BorsukSoftware.Testing.Comparison.Plugins
{
    /// <summary>
    /// Comparer plugin for enums
    /// </summary>
    /// <remarks>This plugin doesn't support the case where there's the comparison between an enum and its underlying integer type. 
    /// This is by design as we would view them as different values</remarks>
    public class EnumComparerPlugin : IObjectComparerPlugin
    {
        public ObjectComparerPluginResults TryCompare(
            string key,
            object expected,
            object actual)
        {
            if (expected == null || !expected.GetType().IsEnum ||
                actual == null || !actual.GetType().IsEnum ||
                actual.GetType() != expected.GetType())
                return new BorsukSoftware.Testing.Comparison.ObjectComparerPluginResults
                {
                    ComparisonResultType = BorsukSoftware.Testing.Comparison.ComparisonResultType.UnableToCompare,
                };

            if (actual.Equals(expected))
                return new BorsukSoftware.Testing.Comparison.ObjectComparerPluginResults
                {
                    ComparisonResultType = BorsukSoftware.Testing.Comparison.ComparisonResultType.Equal
                };

            return new BorsukSoftware.Testing.Comparison.ObjectComparerPluginResults
            {
                ComparisonResultType = BorsukSoftware.Testing.Comparison.ComparisonResultType.Different
            };
        }
    }
}
