using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Plugins
{
	/// <summary>
	/// Standard plugin for handling <see cref="float"/> instances
	/// </summary>
	/// <remarks>This plugin supports both absolute and relative tolerances</remarks>
	public class FloatComparerPlugin : IObjectComparerPlugin
	{
		public bool TreatMissingValuesAsZero { get; set; } = false;

		public FloatingPointToleranceModes ToleranceModes { get; set; }

		public float AbsoluteTolerance { get; set; } = 0;

		public float RelativeTolerance { get; set; } = 0;

		public ObjectComparerPluginResults TryCompare(string key, object expected, object actual)
		{
			if ((expected != null && !(expected is float)) ||
				(actual != null && !(actual is float)))
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.UnableToCompare };

			if (!this.TreatMissingValuesAsZero)
			{
				if (actual == null || expected == null)
					return new ObjectComparerPluginResults
					{
						ComparisonResultType = ComparisonResultType.Different,
						Payload = (actual == null ? 0 : (float)actual) - (expected == null ? 0 : (float)expected)
					};
			}

			// At this point, we know that we can assume the values for actual and expected can be treated as if they're floats (or zeros)

			float expectedAsFloat = expected == null ? 0 : (float)expected;
			float actualAsFloat = actual == null ? 0 : (float)actual;

			var difference = actualAsFloat - expectedAsFloat;
			if (difference == 0)
			{
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };
			}

			if ((this.ToleranceModes & FloatingPointToleranceModes.AbsoluteTolerance) != 0 && 
				Math.Abs(difference) <= this.AbsoluteTolerance)
				return new ObjectComparerPluginResults { ComparisonResultType = ComparisonResultType.Equal };

			if(( this.ToleranceModes & FloatingPointToleranceModes.RelativeTolerance) != 0 &&
				actualAsFloat != 0.0 &&
				expectedAsFloat != 0.0)
			{
				var smallerValue = Math.Min(Math.Abs(actualAsFloat), Math.Abs(expectedAsFloat));
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
