using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3 {
	interface ICommandHandler {
		string CommandName { get; }
		int CommandCalled(TerminalEnvironment terminalEnvironment, string command_name, string[] arguments);
	}
}
