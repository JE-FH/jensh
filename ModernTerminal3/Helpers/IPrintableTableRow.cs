using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.Helpers {
	interface IPrintableTableRow {
		EscapeCodeString[] GetStylizedColumns();
	}

	class PrintableTableDescription {
		public EscapeCodeString[] ColumnHeaders { get; set; }

		public PrintableTableDescription(EscapeCodeString[] columnHeaders) {
			ColumnHeaders = columnHeaders;
		}
	}
}
