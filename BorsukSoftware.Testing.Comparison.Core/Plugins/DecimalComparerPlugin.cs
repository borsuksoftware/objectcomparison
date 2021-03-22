using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	/// <summary>
	/// Standard plugin for handling <see cref="decimal"/> instances
	/// </summary>
	public class DecimalComparerPlugin : IObjectComparerPlugin
	{
		public bool TreatMissingValuesAsZero { get; set; } = false;

		public FloatingPointToleranceModes ToleranceModes { get; set; }

		public decimal AbsoluteTolerance { get; set; } = 0;

		public decimal RelativeTolerance { get; set; } = 0;

		public ObjectComparerPluginResults TryCompare(string key, object expected, object actual)
		{
			if ((expected != null && !(expected is decimal)) ||
				(actual != null && !(actual is decimal)))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			if (!this.TreatMissingValuesAsZero)
			{
				if (actual == null || expected == null)
					return new ObjectComparerPluginResults
					{
						ComparisonResultType = ComparisonResultType.Different,
						Payload = (actual == null ? 0 : (decimal)actual) - (expected == null ? 0 : (decimal)expected)
					};
			}

			// At this point, we know that we can assume the values for actual and expected can be treated as if they're decimals (or zeros)

			decimal expectedAsDecimal = expected == null ? 0 : (decimal)expected;
			decimal actualAsDecimal = actual == null ? 0 : (decimal)actual;

			var difference = actualAsDecimal - expectedAsDecimal;
			if (difference == 0)
			{
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };
			}

			if ((this.ToleranceModes & FloatingPointToleranceModes.AbsoluteTolerance) != 0 && 
				Math.Abs(difference) <= this.AbsoluteTolerance)
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

			if(( this.ToleranceModes & FloatingPointToleranceModes.RelativeTolerance) != 0 &&
				actualAsDecimal != 0 &&
				expectedAsDecimal != 0)
			{
				var smallerValue = Math.Min(Math.Abs(actualAsDecimal), Math.Abs(expectedAsDecimal));
				var dif = Math.Abs(difference);

				var relativeDifference = dif / smallerValue;
				if (relativeDifference <= this.RelativeTolerance)
					return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };
			}

			return new ObjectComparerPluginResults
			{
				ComparisonResultType = ComparisonResultType.Different,
				Payload = difference
			};
		}
	}
}
