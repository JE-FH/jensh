using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.Helpers {
	class TablePrinter {
		int spacing;

		public TablePrinter(int _spacing) {
			spacing = _spacing;
		}

		public void PrintTable(PrintableTableDescription tableDesc, IPrintableTableRow[] rows) {
			
			List<int> columnsLengths = new();

			for (int i = 0; i < tableDesc.ColumnHeaders.Length; i++) {
				columnsLengths.Add(tableDesc.ColumnHeaders[i].RealLength);
			}

			var columnCount = columnsLengths.Count;
			
			foreach(IPrintableTableRow row in rows) {
				EscapeCodeString[] rowColumns = row.GetStylizedColumns();

				if (rowColumns.Length != columnCount) {
					throw new Exception("Inconsistent column count");
				}
				for (int i = 0; i < rowColumns.Length; i++) {
					columnsLengths[i] = Math.Max(rowColumns[i].RealLength, columnsLengths[i]);
				}
			}

			for (int i = 0; i < tableDesc.ColumnHeaders.Length; i++) {
				if (i > 0) {
					Console.Write(new string(' ', spacing));
				}
				Console.Write(tableDesc.ColumnHeaders[i].Data);
				Console.Write(new string(' ', columnsLengths[i] - tableDesc.ColumnHeaders[i].RealLength));
			}
			
			Console.WriteLine();

			for (int i = 0; i < tableDesc.ColumnHeaders.Length; i++) {
				if (i > 0) {
					Console.Write(new string(' ', spacing));
				}
				Console.Write(new string('-', columnsLengths[i]));
			}
			
			Console.WriteLine();

			foreach (IPrintableTableRow row in rows) {
				EscapeCodeString[] rowColumns = row.GetStylizedColumns();
				for (int i = 0; i < rowColumns.Length; i++) {
					if (i > 0) {
						Console.Write(new string(' ', spacing));
					}
					Console.Write(rowColumns[i].Data);
					Console.Write(new string(' ', columnsLengths[i] - rowColumns[i].RealLength));
				}
				Console.WriteLine();
			}

		}
	}
}
