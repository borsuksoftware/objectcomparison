namespace BorsukSoftware.Testing.Comparison
{
    /// <summary>
    /// Enum detailing how an instance of <see cref="ObjectComparer"/> should behave if it comes across a value
    /// within one of the sets for which no key is available in the other set
    /// </summary>
    public enum ObjectComparerMismatchedKeysBehaviour
    {
		/// <summary>
		/// The missing value will be treated as if it had excisted but was null
		/// </summary>
		TreatMissingValueAsNull,

		/// <summary>
		/// The key will be marked as a difference, regardless of the supplied value and the actual comparison plugins
		/// </summary>
		ReportAsDifference,

		/// <summary>
		/// Any mismatched keys will be excluded from the comparison
		/// </summary>
		Ignore,

		/// <summary>
		/// Any mismatched keys will result in an exception being thrown
		/// </summary>
		Throw,
    }
}
