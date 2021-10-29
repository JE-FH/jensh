using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3 {
	interface ITerminalReaderInfoProvider {
		(string newData, int newOffset) TabComplete(string currentInput, int offset);
		
		string GetLastCommand(int offset);

	}
}
