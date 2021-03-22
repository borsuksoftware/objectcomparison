using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	/// <summary>
	/// Standard plugin for handling <see cref="double"/> instances
	/// </summary>
	/// <remarks>This plugin supports both absolute and relative tolerances</remarks>
	public class DoubleComparerPlugin : IObjectComparerPlugin
	{
		public bool TreatMissingValuesAsZero { get; set; } = false;

		public FloatingPointToleranceModes ToleranceModes { get; set; }

		public double AbsoluteTolerance { get; set; } = 0;

		public double RelativeTolerance { get; set; } = 0;

		public ObjectComparerPluginResults TryCompare(string key, object expected, object actual)
		{
			if ((expected != null && !(expected is double)) ||
				(actual != null && !(actual is double)))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			if (!this.TreatMissingValuesAsZero)
			{
				if (actual == null || expected == null)
					return new ObjectComparerPluginResults
					{
						ComparisonResultType = ComparisonResultType.Different,
						Payload = (actual == null ? 0 : (double)actual) - (expected == null ? 0 : (double)expected)
					};
			}

			// At this point, we know that we can assume the values for actual and expected can be treated as if they're doubles (or zeros)

			double expectedAsDouble = expected == null ? 0 : (double)expected;
			double actualAsDouble = actual == null ? 0 : (double)actual;

			var difference = actualAsDouble - expectedAsDouble;
			if (difference == 0)
			{
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };
			}

			if ((this.ToleranceModes & FloatingPointToleranceModes.AbsoluteTolerance) != 0 && 
				Math.Abs(difference) <= this.AbsoluteTolerance)
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

			if(( this.ToleranceModes & FloatingPointToleranceModes.RelativeTolerance) != 0 &&
				actualAsDouble != 0.0 &&
				expectedAsDouble != 0.0)
			{
				var smallerValue = Math.Min(Math.Abs(actualAsDouble), Math.Abs(expectedAsDouble));
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
