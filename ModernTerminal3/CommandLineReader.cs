using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3 {
	class CommandLineReader {
		public CommandLineReader() {

		}

		public string ReadLine() {
			return Console.ReadLine();

			int oldInMode = NativeConsoleOperation.InConsoleMode;
			int oldOutMode = NativeConsoleOperation.OutConsoleMode;

			NativeConsoleOperation.OutConsoleMode = ConsoleOutMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING;
			NativeConsoleOperation.InConsoleMode = 0;


			while (true) {
				ConsoleKeyInfo keyInfo = Console.ReadKey();
			}

			NativeConsoleOperation.InConsoleMode = oldInMode;
			NativeConsoleOperation.OutConsoleMode = oldOutMode;
		}
	}
}
