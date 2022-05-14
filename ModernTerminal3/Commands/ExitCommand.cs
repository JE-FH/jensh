using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.Commands {
	public class ExitCommand : ICommandHandler {
		public string CommandName => "exit";

		public int CommandCalled(TerminalEnvironment terminalEnvironment, string command_name, string[] arguments) {
			if (arguments.Length > 0) {
				if (int.TryParse(arguments[0], NumberStyles.None, CultureInfo.InvariantCulture, out int res)) {
					Environment.Exit(res);
				} else {
					terminalEnvironment.ErrStream.WriteLine("Expected argument 1 to be a valid integer");
					return 1;
				}
			}

			Environment.Exit(0);
			return 0;
		}
	}
}
