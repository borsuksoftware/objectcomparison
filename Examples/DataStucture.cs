using System;
using System.Collections.Generic;
using System.Text;

namespace Examples
{
	/* 
	 * Note that this is explicitly a data structure to demonstrate comparisons, not a recommendation on how to store
	 * financial data!
	 */

	public class InstrumentResults
	{
		public string Identifier { get; set; }

		public double? LocalValue { get; set; }

		public double? BaseCurrencyValue { get; set; }

		public string BaseCurrency { get; set; }
	}

	public class DataStucture
	{
		public InstrumentResults[] InstrumentLevelResults { get; set; }
	}
}
