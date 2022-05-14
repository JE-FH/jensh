using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.Commands {
	class MakeDirectoryCommand : ICommandHandler {
		public string CommandName => "mkdir";

		public MakeDirectoryCommand() { }

		public int CommandCalled(TerminalEnvironment terminalEnvironment, string command_name, string[] arguments) {
			if (arguments.Length != 1) {
				terminalEnvironment.OutStream.WriteLine("Expected atleast 1 argumnent");
				return 1;
			}

			if (Path.IsPathRooted(arguments[0])) {
				Directory.CreateDirectory(arguments[0]);
			} else {
				Directory.CreateDirectory(Path.Join(Environment.CurrentDirectory, arguments[0]));
			}
			return 0;
		}
	}
}
