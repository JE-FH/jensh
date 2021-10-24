using ModernTerminal3.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace ModernTerminal3.Commands {
	class FileSystemEntryData : IPrintableTableRow {
		string attributes;
		string size;
		string lastChange;
		bool isDir;
		string name;

		public FileSystemEntryData(string _attributes, string _size, string _lastChange, bool _isDir, string _name) {
			attributes = _attributes;
			size = _size;
			lastChange = _lastChange;
			isDir = _isDir;
			name = _name;
		}

		public int[] GetRealLengths() {
			return new int[] { attributes.Length, size.Length, lastChange.Length, name.Length };
		}

		EscapeCodeString[] IPrintableTableRow.GetStylizedColumns() {
			EscapeCodeString stylizedName;
			if (isDir) {
				stylizedName = new EscapeCodeString(name.Length, $"\x1B[34;1m{name}\x1B[0m");
			} else {
				stylizedName = new EscapeCodeString(name);
			}
			return new EscapeCodeString[] { 
				new EscapeCodeString(attributes), 
				new EscapeCodeString(size),
				new EscapeCodeString(lastChange),
				stylizedName
			};
		}
	}

	class ListDirCommand : ICommandHandler {
		public string CommandName => "ls";

		public int CommandCalled(string command_name, string[] arguments) {
			var dirInfo = new DirectoryInfo(Environment.CurrentDirectory);
			List<FileSystemEntryData> entries = new();
			
			foreach (var entryInfo in dirInfo.EnumerateDirectories()) {
				entries.Add(new FileSystemEntryData(
					"",
					"",
					FormatDate(entryInfo.LastWriteTime),
					true,
					entryInfo.Name
				));
			}
			
			foreach (var entryInfo in dirInfo.EnumerateFiles()) {
				entries.Add(new FileSystemEntryData(
					"",
					entryInfo.Length.ToString(),
					FormatDate(entryInfo.LastWriteTime),
					false,
					entryInfo.Name
				));
			}

			PrintableTableDescription printableTableDescription = new PrintableTableDescription(new EscapeCodeString[] {
				new EscapeCodeString("Attributes"),
				new EscapeCodeString("Size"),
				new EscapeCodeString("Last modified"),
				new EscapeCodeString("Name"),
			});

			TablePrinter tablePrinter = new(2);
			tablePrinter.PrintTable(printableTableDescription, entries.ToArray());
			return 0;
		}

		private string FormatDate(DateTime dateTime) {
			return DateTime.Now.ToString("dd MMM yyyy");
		}
	}
}
