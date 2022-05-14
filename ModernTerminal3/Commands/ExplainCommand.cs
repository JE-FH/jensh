using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.Commands {
	class ExplainCommand : ICommandHandler {
		public string CommandName => "explain";

		public ExplainCommand() { }

		public int CommandCalled(TerminalEnvironment terminalEnvironment, string command_name, string[] arguments) {
			terminalEnvironment.OutStream.WriteLine($"Got {arguments.Length} arguments");
			foreach (var argument in arguments) {
				terminalEnvironment.OutStream.WriteLine(argument);
			}
			return 0;
		}
	}
}
