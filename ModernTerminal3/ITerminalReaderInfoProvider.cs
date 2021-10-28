using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3 {
	interface ITerminalReaderInfoProvider {
		string TabComplete(string currentInput);
		
		string GetLastCommand(int offset);

	}
}
